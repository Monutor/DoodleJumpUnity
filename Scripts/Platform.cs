using UnityEngine;

/// <summary>
/// Типы платформ в игре.
/// </summary>
public enum PlatformType
{
    Normal,     // Обычная платформа (зелёная)
    Moving,     // Движущаяся платформа (синяя)
    Breakable,  // Исчезающая платформа (жёлтая)
    Boost       // Ускоряющая платформа (красная)
}

/// <summary>
/// Базовый класс платформы.
/// PlatformEffector2D настроен в префабе для односторонней коллизии.
/// </summary>
public class Platform : MonoBehaviour
{
    [Header("Настройки платформы")]
    [SerializeField] protected PlatformType platformType = PlatformType.Normal;

    public PlatformType Type => platformType;

    /// <summary>
    /// Метод для обработки прыжка игрока на платформу.
    /// Переопределяется в наследниках.
    /// </summary>
    public virtual void OnPlayerJump()
    {
        // Базовая реализация - ничего не делает
    }

    /// <summary>
    /// Уничтожить платформу.
    /// </summary>
    public virtual void DestroyPlatform()
    {
        Destroy(gameObject);
    }
}
