using UnityEngine;

/// <summary>
/// Камера следит за игроком по вертикали.
/// Движение плавное, только вверх.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Цель")]
    [SerializeField] private Transform target;
    
    [Header("Настройки слежения")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float yOffset = 0f;
    
    private float _highestY;

    private void Start()
    {
        if (target == null)
        {
            // Пытаемся найти игрока по тегу
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        // Инициализируем самую высокую позицию
        _highestY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        
        FollowTarget();
    }

    /// <summary>
    /// Следит за целью только по вертикали.
    /// Камера двигается только вверх, следуя за игроком.
    /// </summary>
    private void FollowTarget()
    {
        // Целевая позиция Y с оффсетом
        float targetY = target.position.y + yOffset;
        
        // Камера двигается только вверх, когда игрок поднимается выше
        if (targetY > _highestY)
        {
            _highestY = targetY;
        }
        
        // Плавно двигаем камеру к самой высокой точке
        float newY = Mathf.Lerp(transform.position.y, _highestY, followSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    /// <summary>
    /// Публичный метод для сброса позиции камеры.
    /// </summary>
    public void ResetPosition()
    {
        if (target != null)
        {
            transform.position = new Vector3(
                transform.position.x, 
                target.position.y + yOffset, 
                transform.position.z
            );
            _highestY = transform.position.y;
        }
    }

    /// <summary>
    /// Публичный метод для установки цели.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        _highestY = transform.position.y;
    }
}
