using UnityEngine;
using System.Collections;

/// <summary>
/// Исчезающая платформа - разрушается после прыжка игрока.
/// </summary>
public class BreakablePlatform : Platform
{
    [Header("Настройки разрушения")]
    [SerializeField] private float destroyDelay = 0.5f;
    [SerializeField] private float flashDuration = 0.3f;

    private SpriteRenderer _spriteRenderer;
    private bool _isBreaking = false;
    private Collider2D _platformCollider;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _platformCollider = GetComponent<Collider2D>();
        platformType = PlatformType.Breakable;
    }

    /// <summary>
    /// Вызывается при прыжке игрока на платформу.
    /// </summary>
    public override void OnPlayerJump()
    {
        if (!_isBreaking)
        {
            StartCoroutine(BreakSequence());
        }
    }

    /// <summary>
    /// Последовательность разрушения платформы.
    /// </summary>
    private IEnumerator BreakSequence()
    {
        _isBreaking = true;

        // Мигание перед разрушением
        float flashInterval = 0.1f;
        int flashCount = Mathf.FloorToInt(flashDuration / flashInterval);

        for (int i = 0; i < flashCount; i++)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
        }
        _spriteRenderer.enabled = true;

        // Задержка перед разрушением (даёт игроку время улететь вверх)
        yield return new WaitForSeconds(destroyDelay);

        // Отключаем коллайдер и скрываем спрайт
        _platformCollider.enabled = false;
        _spriteRenderer.enabled = false;

        // Уничтожаем объект
        DestroyPlatform();
    }

    /// <summary>
    /// Визуализация в редакторе.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
