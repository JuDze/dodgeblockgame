using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject continueButton;

    void Start()
    {
        if (continueButton)
            continueButton.SetActive(GameSaveManager.HasSave());
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("game");
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetInt("LoadSave", 1);
        SceneManager.LoadScene("game");
    }

    public void QuitGame()
    {
        UnityEngine.Debug.Log("Quit Game");
        Application.Quit();
    }
}