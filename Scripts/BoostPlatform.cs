using UnityEngine;

/// <summary>
/// Платформа с пружиной - даёт высокий прыжок вверх.
/// </summary>
public class BoostPlatform : Platform
{
    [Header("Настройки высокого прыжка")]
    [SerializeField] private float highJumpForce = 18f;
    [SerializeField] private Color boostColor = new Color(1f, 0.3f, 0.3f, 1f);

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        platformType = PlatformType.Boost;
    }

    /// <summary>
    /// Вызывается при прыжке игрока на платформу.
    /// Применяет высокий прыжок вверх.
    /// </summary>
    public override void OnPlayerJump()
    {
        // Находим игрока и применяем высокую силу прыжка
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Устанавливаем высокую вертикальную скорость для супер-прыжка
                playerRb.linearVelocity = new Vector2(
                    playerRb.linearVelocity.x,
                    highJumpForce
                );
            }
        }

        // Визуальный эффект пружины
        StartCoroutine(FlashEffect());
    }

    /// <summary>
    /// Эффект мигания платформы.
    /// </summary>
    private System.Collections.IEnumerator FlashEffect()
    {
        _spriteRenderer.color = boostColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = _originalColor;
    }

    /// <summary>
    /// Визуализация в редакторе.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.3f);
    }
}
