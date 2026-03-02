using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управляет состоянием игры: счёт, Game Over, перезапуск.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Настройки игры")]
    [SerializeField] private float fallDeathThreshold = -10f;
    
    private DoodlerController _player;
    private CameraFollow _camera;
    private bool _isGameOver;
    private int _score;

    public int Score => _score;
    public bool IsGameOver => _isGameOver;

    private void Awake()
    {
        // Находим игрока и камеру
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.GetComponent<DoodlerController>();
        }
        
        _camera = Camera.main.GetComponent<CameraFollow>();
    }

    private void Update()
    {
        if (_isGameOver) return;
        
        CheckGameOver();
        UpdateScore();
    }

    /// <summary>
    /// Проверяет условие проигрыша (игрок упал слишком низко).
    /// </summary>
    private void CheckGameOver()
    {
        if (_player == null) return;
        
        if (_player.transform.position.y < fallDeathThreshold)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Обновляет счёт на основе высоты игрока.
    /// </summary>
    private void UpdateScore()
    {
        if (_player == null) return;
        
        // Счёт = высота в метрах (округлённая)
        int newScore = Mathf.Max(0, Mathf.FloorToInt(_player.transform.position.y));
        if (newScore > _score)
        {
            _score = newScore;
        }
    }

    /// <summary>
    /// Завершает игру.
    /// </summary>
    private void GameOver()
    {
        _isGameOver = true;
        Debug.Log($"Game Over! Счёт: {_score}");
        
        // Перезапуск через 2 секунды
        Invoke(nameof(RestartGame), 2f);
    }

    /// <summary>
    /// Перезапускает сцену.
    /// </summary>
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Сбрасывает состояние игры.
    /// </summary>
    public void ResetGame()
    {
        _isGameOver = false;
        _score = 0;
    }
}
