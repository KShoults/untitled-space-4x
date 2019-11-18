using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Main View");
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
