using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("game"); // load your actual game scene
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public GameObject continueButton;
    void Start()
    {
        
        if (continueButton)
            continueButton.SetActive(SaveSystem.HasSave());
    }

    public void ContinueGame()
    {
        
        PlayerPrefs.SetInt("LoadSave", 1);
        SceneManager.LoadScene("game");
    }
}
 