using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private bool isPaused = false;
    private bool _toggleBlocked = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        _toggleBlocked = false;
    }

    public void TogglePause()
    {
        Debug.Log($"TogglePause called | isPaused={isPaused} | blocked={_toggleBlocked}");

        if (_toggleBlocked)
            return;

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Debug.Log("PauseGame called");

        _toggleBlocked = true;
        isPaused = true;
        Time.timeScale = 0f;

        FindObjectOfType<GameManager>()?.SaveGame();

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Debug.Log("ResumeGame called");

        _toggleBlocked = true;
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (!GameSaveManager.HasSave())
        {
            Debug.LogWarning("Save file not found. Returning to MainMenu.");
            SceneManager.LoadScene("MainMenu");
            return;
        }

        GameSaveManager.Load();
        Debug.Log("LastLoadWasCorrupted=" + GameSaveManager.LastLoadWasCorrupted);

        if (GameSaveManager.LastLoadWasCorrupted)
        {
            Debug.LogWarning("Save file is corrupted. Deleting save and returning to MainMenu.");
            GameSaveManager.Delete();
            SceneManager.LoadScene("MainMenu");
            return;
        }

        StartCoroutine(ResetToggleDelay());
    }

    private IEnumerator ResetToggleDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        _toggleBlocked = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        _toggleBlocked = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}