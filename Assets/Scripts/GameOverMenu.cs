using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene("game");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
