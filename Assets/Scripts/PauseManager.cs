using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; 
    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // make sure game starts unpaused
    }

    public void TogglePause()
    {
        Debug.Log("Toggling pause. Currently paused: " + isPaused);
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // freeze physics and updates
        FindObjectOfType<GameManager>()?.SaveGame();
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // resume normal speed
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

