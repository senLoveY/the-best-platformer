using UnityEngine;
using UnityEngine.SceneManagement; 

public class FinishPoint : MonoBehaviour
{
    [Header("Настройки времени")]
    public float delayBeforeNextLevel = 3f; 
    public float delayBeforeMenu = 5f;      

    [Header("Настройки сцен")]
    public string nextLevelName;           
    public bool isLastLevel = false;      

    [Header("Интерфейс (UI)")]
    public GameObject victoryText;         
    public GameObject gameCompleteUI;      

    private bool isFinished = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isFinished)
        {
            isFinished = true;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.ChangeMusic(AudioManager.instance.victoryMusic);
            }

            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false; 

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero; 

            if (isLastLevel)
            {
                if (gameCompleteUI != null) gameCompleteUI.SetActive(true);
                Debug.Log("ВСЯ ИГРА ПРОЙДЕНА. Возврат в меню...");
                
                Invoke("ReturnToMenu", delayBeforeMenu);
            }
            else
            {
                if (victoryText != null) victoryText.SetActive(true);
                Debug.Log("Уровень пройден. Загрузка следующего...");

                Invoke("LoadNextLevel", delayBeforeNextLevel);
            }
        }
    }

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

    void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}