using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Контроллер игрока (Doodler).
/// Обрабатывает прыжки от платформ, гравитацию и движение.
/// </summary>
public class DoodlerController : MonoBehaviour
{
    [Header("Физика")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float maxFallSpeed = -15f;

    [Header("Движение")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 30f;

    [Header("Проверка земли")]
    [SerializeField] private float groundCheckDistance = 0.55f;
    [SerializeField] private LayerMask platformLayerMask;

    [Header("Настройки коллизий")]
    [SerializeField] private float platformTopTolerance = 0.1f;

    [Header("Границы экрана")]
    [SerializeField] private float leftBoundary = -3f;
    [SerializeField] private float rightBoundary = 3f;

    [Header("Анимация")]
    [SerializeField] private float flipVelocityThreshold = 0.1f;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite jumpSprite;
    [SerializeField] private Sprite fallSprite;

    private Rigidbody2D _rigidbody;
    private float _horizontalInput;
    private float _currentVelocityX;
    private Collider2D _playerCollider;
    private SpriteRenderer _spriteRenderer;
    private bool _isGrounded;
    private float _walkFrameTimer;
    private int _currentWalkFrame;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody.gravityScale = gravityScale;

        // Устанавливаем тег Player для камеры
        if (!CompareTag("Player"))
        {
            gameObject.tag = "Player";
        }
    }

    private void Update()
    {
        HandleMovement();
        CheckGround();
        ClampPosition();
        UpdateAnimation();

        // Ограничиваем скорость падения
        if (_rigidbody.linearVelocity.y < maxFallSpeed)
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, maxFallSpeed);
        }
    }

    /// <summary>
    /// Обрабатывает ввод и применяет движение.
    /// </summary>
    private void HandleMovement()
    {
        // Получаем ввод из Input System
        _horizontalInput = GetHorizontalInput();
        
        // Плавное ускорение/замедление
        if (Mathf.Abs(_horizontalInput) > 0.1f)
        {
            _currentVelocityX = Mathf.MoveTowards(
                _currentVelocityX, 
                _horizontalInput * moveSpeed, 
                acceleration * Time.deltaTime
            );
        }
        else
        {
            // Плавная остановка
            _currentVelocityX = Mathf.MoveTowards(
                _currentVelocityX, 
                0f, 
                deceleration * Time.deltaTime
            );
        }
        
        // Применяем скорость к Rigidbody2D (только по X)
        _rigidbody.linearVelocity = new Vector2(_currentVelocityX, _rigidbody.linearVelocity.y);
    }

    /// <summary>
    /// Получает горизонтальный ввод из Input System.
    /// </summary>
    private float GetHorizontalInput()
    {
        float input = 0f;
        
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                input -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                input += 1f;
        }
        
        return Mathf.Clamp(input, -1f, 1f);
    }

    /// <summary>
    /// Проверяет, находится ли игрок НАД платформой.
    /// </summary>
    private void CheckGround()
    {
        // Проверяем, находимся ли мы над какой-либо платформой
        // Пускаем короткий луч вниз от центра игрока
        Vector2 rayOrigin = (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, platformLayerMask);

        _isGrounded = false; // Сбрасываем флаг

        if (hit.collider != null)
        {
            // Проверяем, что игрок находится ВЫШЕ платформы (ноги около верха платформы)
            float playerBottom = transform.position.y - (_playerCollider.bounds.extents.y);
            float platformTop = hit.collider.bounds.max.y;

            // Игрок должен быть чуть выше платформы и падать вниз
            if (playerBottom >= platformTop - platformTopTolerance && _rigidbody.linearVelocity.y <= 0f)
            {
                _isGrounded = true; // Игрок на земле

                // Вызываем событие на платформе (для бонусных платформ)
                Platform platform = hit.collider.GetComponent<Platform>();
                if (platform != null)
                {
                    // Для BoostPlatform платформа сама применяет прыжок
                    if (platform is BoostPlatform)
                    {
                        platform.OnPlayerJump();
                        return; // Не вызываем обычный Jump()
                    }

                    platform.OnPlayerJump();
                }

                Jump();
            }
        }
    }

    /// <summary>
    /// Прыжок вверх.
    /// </summary>
    private void Jump()
    {
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, jumpForce);
    }

    /// <summary>
    /// Ограничивает позицию игрока границами экрана.
    /// </summary>
    private void ClampPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, leftBoundary, rightBoundary);
        transform.position = position;
    }

    /// <summary>
    /// Визуализация проверки земли в редакторе.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    /// <summary>
    /// Обновляет параметры анимации.
    /// </summary>
    private void UpdateAnimation()
    {
        if (_spriteRenderer == null) return;

        // Определяем состояние и выбираем спрайт
        Sprite currentSprite;
        bool isMoving = Mathf.Abs(_currentVelocityX) > flipVelocityThreshold;

        if (!_isGrounded)
        {
            // В воздухе
            if (_rigidbody.linearVelocity.y > 0)
            {
                currentSprite = jumpSprite; // Прыжок
            }
            else
            {
                currentSprite = fallSprite; // Падение
            }
        }
        else if (isMoving)
        {
            // Анимация ходьбы
            _walkFrameTimer += Time.deltaTime;
            if (_walkFrameTimer >= 0.1f) // Смена кадра каждые 0.1 сек
            {
                _walkFrameTimer = 0f;
                _currentWalkFrame = (_currentWalkFrame + 1) % walkSprites.Length;
            }
            currentSprite = walkSprites[_currentWalkFrame];
        }
        else
        {
            currentSprite = idleSprite; // Бездействие
        }

        _spriteRenderer.sprite = currentSprite;

        // Разворачиваем спрайт в зависимости от направления движения
        UpdateSpriteDirection();
    }

    /// <summary>
    /// Разворачивает спрайт в зависимости от направления движения.
    /// </summary>
    private void UpdateSpriteDirection()
    {
        if (_spriteRenderer == null) return;

        // Разворачиваем спрайт в направлении движения
        if (_currentVelocityX > flipVelocityThreshold)
        {
            _spriteRenderer.flipX = false; // Движемся вправо
        }
        else if (_currentVelocityX < -flipVelocityThreshold)
        {
            _spriteRenderer.flipX = true; // Движемся влево
        }
    }

    /// <summary>
    /// Публичный метод для сброса позиции (после Game Over).
    /// </summary>
    public void ResetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        _rigidbody.linearVelocity = Vector2.zero;
        _currentVelocityX = 0f;
        _isGrounded = false;
    }

    /// <summary>
    /// Публичный метод для настройки границ.
    /// </summary>
    public void SetBoundaries(float left, float right)
    {
        leftBoundary = left;
        rightBoundary = right;
    }
}
