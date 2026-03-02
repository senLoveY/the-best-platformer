using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для смены сцен

public class FinishPoint : MonoBehaviour
{
    [Header("Настройки времени")]
    public float delayBeforeNextLevel = 3f; // Задержка между уровнями
    public float delayBeforeMenu = 5f;      // Задержка после финала игры

    [Header("Настройки сцен")]
    public string nextLevelName;           // Имя следующей сцены (например, Level2)
    public bool isLastLevel = false;       // Поставь галочку только на последнем уровне

    [Header("Интерфейс (UI)")]
    public GameObject victoryText;         // Текст "Level Complete"
    public GameObject gameCompleteUI;      // Текст "The End / Thanks for playing"

    private bool isFinished = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что в триггер вошел именно игрок
        if (other.CompareTag("Player") && !isFinished)
        {
            isFinished = true;

            // 1. Смена музыки на победную
            if (AudioManager.instance != null)
            {
                AudioManager.instance.ChangeMusic(AudioManager.instance.victoryMusic);
            }

            // 2. Остановка игрока
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false; // Отключаем управление

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero; // Обнуляем скорость

            // 3. Логика завершения
            if (isLastLevel)
            {
                // Если это финал всей игры
                if (gameCompleteUI != null) gameCompleteUI.SetActive(true);
                Debug.Log("ВСЯ ИГРА ПРОЙДЕНА. Возврат в меню...");
                
                Invoke("ReturnToMenu", delayBeforeMenu);
            }
            else
            {
                // Если это просто конец текущего уровня
                if (victoryText != null) victoryText.SetActive(true);
                Debug.Log("Уровень пройден. Загрузка следующего...");

                Invoke("LoadNextLevel", delayBeforeNextLevel);
            }
        }
    }

    // Метод для перехода на следующий уровень
    void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogError("Имя следующего уровня не указано в Инспекторе!");
        }
    }

    // Метод для возврата в главное меню
    void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}