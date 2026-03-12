using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int lives = 3;
    public int score = 0;
    public int level = 1;
   

    [Header("References")]
    public GameObject player;
    public Player playerScript;
    public BlockSpawner spawner;
    public GameObject TextTapToStart;
    public TextMeshProUGUI uiText;
    public TextMeshProUGUI levelUpText;
    public GameObject blockPrefab;
    public GameObject pauseButton;

    private bool gameStarted = false;

    void Awake(){

       // Time.timeScale = 0f; // pause game at start
        gameStarted = false;

        if (player) player.SetActive(false);
        if (spawner) spawner.enabled = false;
        if (TextTapToStart) TextTapToStart.SetActive(true);
        if(blockPrefab) blockPrefab.SetActive(false); // hide template
        if (pauseButton) pauseButton.SetActive(false);
       
        if (PlayerPrefs.GetInt("LoadSave", 0) == 1)
        {
            PlayerPrefs.DeleteKey("LoadSave");
            LoadGame();
            StartGame(); 
        }
        UpdateUI();
       
    }


    void LateUpdate()
    {
        UpdateUI();
    }

    public void StartGame()
    {
        if (gameStarted) return; // already started

        gameStarted = true;
        //Time.timeScale = 1f; // unpause

        if (player) player.SetActive(true);
        //if (spawner) spawner.enabled = true;
        if (TextTapToStart) TextTapToStart.SetActive(false);

        StartCoroutine(EnableSpawnerAfterDelay(1f));
        if (pauseButton) pauseButton.SetActive(true); 
    }



    private IEnumerator EnableSpawnerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spawner) spawner.enabled = true;
      //  Debug.Log("Spawner enabled. Game started.");
    }

        public void PlayerHit()
    {
        lives--;
        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            SaveSystem.Delete();
            // TODO: reload scene or show UI
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver"); // reload current scene
        }
        else
        {
            Debug.Log("Lives left: " + lives);
        }
        
    }


    public void AddScore(){
        int points = GetPointsForLevel(level);
        score += points;
        UpdateUI();
         CheckLevelProgression();
       // Debug.Log($"+{points} points! Total: {score}");
       

    }

    public void NextLevel()
    {
        Debug.Log(">>> Entered NextLevel()");
        level++;
        spawner.NextLevel();
        UpdateUI();

        if (playerScript != null)
        {
            playerScript.IncreaseSpeed(level);
            Debug.Log("Player speed increased to " + level);
        }
        else
        {
            Debug.LogWarning(" PlayerScript reference is null!");
        }

        Debug.Log("Level up! Now level " + level);
    }


    private int GetPointsForLevel(int lvl)
    {
        return 10 + (lvl - 1) * 5; // e.g. level 1 = 10, level 2 = 15, level 3 = 20, etc.
    }  

    private void ShowLevelUpText()
        {
            if (!levelUpText) return;
            StopAllCoroutines();
            StartCoroutine(LevelUpFlash());
        }

    private IEnumerator LevelUpFlash()
    {
        levelUpText.gameObject.SetActive(true);
        levelUpText.text = $"LEVEL {level}!";
        yield return new WaitForSeconds(1.5f);
        levelUpText.gameObject.SetActive(false);
    }

    private void CheckLevelProgression()
    {
        while (score >= GetScoreThresholdForLevel(level + 1))
        {
            level++;

            if (spawner) spawner.NextLevel();
            if (playerScript != null)
            {
                playerScript.IncreaseSpeed(level);
                Debug.Log("Player speed increased to " + level);
            }
            else
            {
                Debug.LogWarning("PlayerScript reference is null in level up!");
            }

            Debug.Log("Level up! Now level " + level);

            ShowLevelUpText();
            UpdateUI();
            SaveGame();
        }
    }


        private int GetScoreThresholdForLevel(int lvl)
    {
       return Mathf.RoundToInt(100f * (lvl - 1) * lvl / 2f);
       //Debug.Log("Next level threshold: " + GetScoreThresholdForLevel(lvl));
    }

    private void UpdateUI()
    {
        if (uiText)
            {
                uiText.ForceMeshUpdate();
                uiText.text =
                    $"<size=120%><b>Level {level}</b></size>\n" +
                    $"Score: <color=#00FF00><mspace=0.6em>{score}</mspace></color>\n" +
                    $"Lives: <color=#FF4444>{lives}</color>";
                    Canvas.ForceUpdateCanvases(); 
                    uiText.canvasRenderer.SetAlpha(1f);
            }
    }   

           
            public void SaveGame()
        {
            SaveData data = new SaveData
            {
                score         = score,
                level         = level,
                lives         = lives,
                playerX       = player.transform.position.x,
                playerY       = player.transform.position.y,
                moveSpeed     = playerScript.moveSpeed,      
                blockFallSpeed = spawner.blockFallSpeed,
                spawnInterval = spawner.spawnInterval
            };
            SaveSystem.Save(data);
        }

        public void LoadGame()
        {
            SaveData data = SaveSystem.Load();
            if (data == null) return;

            score  = data.score;
            level  = data.level;
            lives  = data.lives;

            player.transform.position = new Vector3(data.playerX, data.playerY, 0f);
            playerScript.moveSpeed    = data.moveSpeed;
            spawner.blockFallSpeed    = data.blockFallSpeed;
            spawner.spawnInterval     = data.spawnInterval;

            UpdateUI();
        }

}
