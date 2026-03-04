using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject levelsPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OpenLevels()
    {
        mainPanel.SetActive(false);
        levelsPanel.SetActive(true);
    }

    public void BackToMain()
    {
        mainPanel.SetActive(true);
        levelsPanel.SetActive(false);
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Игра закрыта"); 
    }
}