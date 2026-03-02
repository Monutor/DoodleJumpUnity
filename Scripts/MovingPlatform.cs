using UnityEngine;

/// <summary>
/// Движущаяся платформа - перемещается горизонтально.
/// </summary>
public class MovingPlatform : Platform
{
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveRange = 2f;

    private Vector3 _startPosition;
    private float _moveDirection = 1f;

    private void Start()
    {
        _startPosition = transform.position;
        platformType = PlatformType.Moving;
    }

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// Движение платформы туда-обратно.
    /// </summary>
    private void Move()
    {
        Vector3 newPosition = transform.position;
        newPosition.x += moveSpeed * _moveDirection * Time.deltaTime;

        // Проверяем, не вышли ли за пределы диапазона
        float distanceFromStart = Mathf.Abs(newPosition.x - _startPosition.x);
        
        if (distanceFromStart >= moveRange)
        {
            _moveDirection *= -1f; // Меняем направление
        }

        transform.position = newPosition;
    }

    /// <summary>
    /// Визуализация диапазона движения в редакторе.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            _startPosition + Vector3.left * moveRange,
            _startPosition + Vector3.right * moveRange
        );
    }
}
