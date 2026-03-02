using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Генерирует платформы и управляет их движением.
/// Платформы НЕ двигаются сами — они статичны в мире.
/// Движение вниз создаётся иллюзией за счёт движения камеры и игрока.
/// </summary>
public class PlatformSpawner : MonoBehaviour
{
    [Header("Настройки префаба")]
    [SerializeField] private GameObject normalPlatformPrefab;
    [SerializeField] private GameObject movingPlatformPrefab;
    [SerializeField] private GameObject breakablePlatformPrefab;
    [SerializeField] private GameObject boostPlatformPrefab;

    [Header("Настройки генерации")]
    [SerializeField] private float platformSpacing = 1.2f;
    [SerializeField] private float spawnTimerInterval = 0.3f;
    [SerializeField] private float minX = -2f;
    [SerializeField] private float maxX = 2f;

    [Header("Настройки мира")]
    [SerializeField] private float initialPlatformCount = 10f;

    [Header("Вероятности типов платформ")]
    [Range(0f, 1f)] [SerializeField] private float movingPlatformChance = 0.2f;
    [Range(0f, 1f)] [SerializeField] private float breakablePlatformChance = 0.15f;
    [Range(0f, 1f)] [SerializeField] private float boostPlatformChance = 0.1f;

    private float _spawnTimer;
    private float _lastSpawnY;
    private float _highestPlatformY;

    private void Start()
    {
        // Спавним стартовую платформу точно под игроком (всегда по центру)
        SpawnStartingPlatform();

        // Спавним остальные начальные платформы
        _lastSpawnY = platformSpacing;
        _highestPlatformY = platformSpacing;

        for (int i = 1; i < initialPlatformCount; i++)
        {
            SpawnPlatform(_lastSpawnY);
            _lastSpawnY += platformSpacing;
            _highestPlatformY = _lastSpawnY;
        }
    }

    /// <summary>
    /// Спавнит стартовую платформу точно по центру на позиции y=0.
    /// Игрок всегда появляется на ней.
    /// </summary>
    private void SpawnStartingPlatform()
    {
        Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
        GameObject platform = Instantiate(normalPlatformPrefab, spawnPosition, Quaternion.identity);
        platform.transform.SetParent(transform);
    }

    private void Update()
    {
        // Генерируем новые платформы выше самой высокой
        // Это происходит, когда игрок поднимается
        float highestVisibleY = Camera.main.transform.position.y + Camera.main.orthographicSize + 5f;

        if (_highestPlatformY < highestVisibleY)
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= spawnTimerInterval)
            {
                _spawnTimer = 0f;
                SpawnPlatform(_highestPlatformY);
                _highestPlatformY += platformSpacing;
            }
        }

        // Удаляем платформы, ушедшие далеко вниз
        DestroyPlatformsBelowBoundary();
    }

    /// <summary>
    /// Выбирает тип платформы на основе вероятностей.
    /// </summary>
    private GameObject GetRandomPlatformPrefab()
    {
        float random = Random.value;
        
        // Проверяем в порядке от редкого к частому
        if (random < boostPlatformChance)
            return boostPlatformPrefab;
        
        random -= boostPlatformChance;
        
        if (random < breakablePlatformChance)
            return breakablePlatformPrefab;
        
        random -= breakablePlatformChance;
        
        if (random < movingPlatformChance)
            return movingPlatformPrefab;
        
        // По умолчанию обычная платформа
        return normalPlatformPrefab;
    }

    /// <summary>
    /// Спавнит платформу на указанной высоте со случайной X-позицией.
    /// </summary>
    private void SpawnPlatform(float yPosition)
    {
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, yPosition, 0f);

        // Выбираем случайный тип платформы
        GameObject platformPrefab = GetRandomPlatformPrefab();
        
        GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        platform.transform.SetParent(transform);
    }

    /// <summary>
    /// Удаляет платформы, которые опустились ниже границы.
    /// </summary>
    private void DestroyPlatformsBelowBoundary()
    {
        float destroyY = Camera.main.transform.position.y - Camera.main.orthographicSize - 10f;

        List<Transform> childrenToRemove = new List<Transform>();

        foreach (Transform child in transform)
        {
            if (child.position.y < destroyY)
            {
                childrenToRemove.Add(child);
            }
        }

        foreach (Transform child in childrenToRemove)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Возвращает позицию самой высокой платформы.
    /// </summary>
    public float GetHighestPlatformY()
    {
        return _highestPlatformY;
    }
}
