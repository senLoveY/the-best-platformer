using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private void Awake()
    {
        // Логика Синглтона (чтобы менеджер был только один и не удалялся)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Подписываемся на событие загрузки сцены
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Этот метод срабатывает АВТОМАТИЧЕСКИ каждый раз, когда загружается любая сцена
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Как только сцена загрузилась, проверяем музыку
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBackgroundMusic();
        }
    }

    public void Respawn()
    {
        // Перезагрузка текущей сцены (все враги и игрок появятся заново)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}