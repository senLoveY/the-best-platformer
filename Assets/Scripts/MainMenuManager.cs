using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки сцен

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject levelsPanel;

    // 1. Кнопка Play - запускает 1-й уровень
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    // 2. Переход к выбору уровней
    public void OpenLevels()
    {
        mainPanel.SetActive(false);
        levelsPanel.SetActive(true);
    }

    // 3. Возврат в главное меню
    public void BackToMain()
    {
        mainPanel.SetActive(true);
        levelsPanel.SetActive(false);
    }

    // 4. Загрузка конкретного уровня по имени
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 5. Выход из игры
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Игра закрыта"); // В редакторе кнопка не закроет окно, поэтому пишем лог
    }
}