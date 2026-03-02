using UnityEngine;

/// <summary>
/// Скрипт для создания эффекта бесконечного фона.
/// Фон следует за камерой по вертикали с параллакс-эффектом.
/// Использует две копии фона для создания бесконечного эффекта.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("Настройки параллакса")]
    [Tooltip("Коэффициент следования за камерой (0-1). Меньше = медленнее фон")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactor = 0.3f;

    [Header("Настройки фона")]
    [Tooltip("Спрайт фона (облака)")]
    [SerializeField] private Sprite backgroundSprite;

    [Tooltip("Порядок сортировки спрайта")]
    [SerializeField] private int sortingOrder = -10;

    [Tooltip("Масштаб фона")]
    [SerializeField] private float backgroundScale = 1f;

    [Tooltip("Ширина фона в единицах сцены")]
    [SerializeField] private float backgroundWidth = 20f;

    [Tooltip("Высота фона в единицах сцены")]
    [SerializeField] private float backgroundHeight = 20f;

    private Camera _mainCamera;
    private Vector3 _previousCameraPosition;
    private SpriteRenderer _spriteRenderer;
    
    // Две копии фона для бесконечного эффекта
    private GameObject _backgroundTop;
    private GameObject _backgroundBottom;

    private void Start()
    {
        _mainCamera = Camera.main;
        _previousCameraPosition = _mainCamera.transform.position;

        // Создаем две копии фона
        CreateBackgroundCopies();

        // Применяем настройки
        ApplySettings();

        // Позиционируем фон на начальную позицию камеры
        RepositionBackgrounds();
    }

    private void LateUpdate()
    {
        if (_mainCamera == null || _backgroundTop == null || _backgroundBottom == null) return;

        // Вычисляем смещение камеры
        Vector3 cameraMovement = _mainCamera.transform.position - _previousCameraPosition;

        // Двигаем оба фона с учетом коэффициента параллакса (только по вертикали)
        float verticalMovement = cameraMovement.y * parallaxFactor;
        _backgroundTop.transform.position += new Vector3(0, verticalMovement, 0);
        _backgroundBottom.transform.position += new Vector3(0, verticalMovement, 0);

        // Проверяем, нужно ли переместить фон для создания бесконечного эффекта
        CheckAndRepositionBackgrounds();

        // Обновляем предыдущую позицию камеры
        _previousCameraPosition = _mainCamera.transform.position;
    }

    /// <summary>
    /// Создает две копии фона для бесконечного эффекта.
    /// </summary>
    private void CreateBackgroundCopies()
    {
        // Верхний фон
        _backgroundTop = new GameObject("Background_Top");
        _backgroundTop.transform.parent = transform;
        _backgroundTop.transform.localPosition = Vector3.zero;
        
        // Нижний фон
        _backgroundBottom = new GameObject("Background_Bottom");
        _backgroundBottom.transform.parent = transform;
        _backgroundBottom.transform.localPosition = Vector3.zero;

        // Добавляем SpriteRenderer к обеим копиям
        _spriteRenderer = _backgroundTop.AddComponent<SpriteRenderer>();
        _backgroundBottom.AddComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Проверяет, нужно ли переместить фон для создания бесконечного эффекта.
    /// </summary>
    private void CheckAndRepositionBackgrounds()
    {
        float cameraY = _mainCamera.transform.position.y;
        float cameraOrthographicSize = _mainCamera.orthographicSize;
        float cameraBottom = cameraY - cameraOrthographicSize;
        float cameraTop = cameraY + cameraOrthographicSize;

        float topBackgroundBottom = _backgroundTop.transform.position.y - backgroundHeight / 2;
        float bottomBackgroundTop = _backgroundBottom.transform.position.y + backgroundHeight / 2;

        // Если верхний фон ушел слишком низко, перемещаем его вверх
        if (topBackgroundBottom < cameraBottom)
        {
            _backgroundTop.transform.position = new Vector3(
                _backgroundTop.transform.position.x,
                _backgroundBottom.transform.position.y + backgroundHeight,
                _backgroundTop.transform.position.z
            );
        }
        // Если нижний фон ушел слишком высоко, перемещаем его вниз
        else if (bottomBackgroundTop > cameraTop)
        {
            _backgroundBottom.transform.position = new Vector3(
                _backgroundBottom.transform.position.x,
                _backgroundTop.transform.position.y - backgroundHeight,
                _backgroundBottom.transform.position.z
            );
        }
    }

    /// <summary>
    /// Применяет настройки к GameObject.
    /// </summary>
    private void ApplySettings()
    {
        // Проверяем, что копии фона созданы
        if (_backgroundTop == null || _backgroundBottom == null) return;

        if (_spriteRenderer == null)
        {
            _spriteRenderer = _backgroundTop.GetComponent<SpriteRenderer>();
        }

        SpriteRenderer bottomRenderer = _backgroundBottom.GetComponent<SpriteRenderer>();

        if (backgroundSprite != null && _spriteRenderer != null)
        {
            _spriteRenderer.sprite = backgroundSprite;
            _spriteRenderer.sortingOrder = sortingOrder;
            _spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            _spriteRenderer.size = new Vector2(backgroundWidth, backgroundHeight);

            // Копируем настройки на нижний фон
            if (bottomRenderer != null)
            {
                bottomRenderer.sprite = backgroundSprite;
                bottomRenderer.sortingOrder = sortingOrder;
                bottomRenderer.drawMode = SpriteDrawMode.Sliced;
                bottomRenderer.size = new Vector2(backgroundWidth, backgroundHeight);
            }
        }

        // Устанавливаем масштаб
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Перемещает оба фона на позицию камеры.
    /// </summary>
    private void RepositionBackgrounds()
    {
        float cameraX = _mainCamera.transform.position.x;
        float cameraY = _mainCamera.transform.position.y;

        // Позиционируем верхний фон по центру камеры
        _backgroundTop.transform.position = new Vector3(cameraX, cameraY, transform.position.z);
        
        // Позиционируем нижний фон прямо под верхним
        _backgroundBottom.transform.position = new Vector3(cameraX, cameraY - backgroundHeight, transform.position.z);
    }

    /// <summary>
    /// Публичный метод для сброса позиции фона.
    /// </summary>
    public void ResetPosition()
    {
        if (_mainCamera != null)
        {
            _previousCameraPosition = _mainCamera.transform.position;
            RepositionBackgrounds();
        }
    }

    /// <summary>
    /// Публичный метод для установки коэффициента параллакса.
    /// </summary>
    public void SetParallaxFactor(float factor)
    {
        parallaxFactor = Mathf.Clamp01(factor);
    }

    /// <summary>
    /// Очищает фон при уничтожении.
    /// </summary>
    private void OnDestroy()
    {
        if (_backgroundTop != null && Application.isPlaying)
        {
            Destroy(_backgroundTop);
        }
        if (_backgroundBottom != null && Application.isPlaying)
        {
            Destroy(_backgroundBottom);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// В редакторе обновляем размеры при изменении спрайта.
    /// </summary>
    private void OnValidate()
    {
        // Обновляем только если фон уже создан (во время игры)
        if (backgroundSprite != null && Application.isPlaying && _backgroundTop != null)
        {
            ApplySettings();
        }
    }
#endif
}
