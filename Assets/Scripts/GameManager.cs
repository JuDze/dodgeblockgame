using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int lives;
    public int score;
    public int level;

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

    void Awake()
    {
        gameStarted = false;

        if (player) player.SetActive(false);
        if (spawner) spawner.enabled = false;
        if (TextTapToStart) TextTapToStart.SetActive(true);
        if (blockPrefab) blockPrefab.SetActive(false);
        if (pauseButton) pauseButton.SetActive(false);

        // Always reset score/level/lives from defaults first
        var defaults = new Game.SaveIntegration.Data.GameSaveData();
        score = defaults.score;
        level = defaults.level;
        lives = defaults.lives;

        if (PlayerPrefs.GetInt("LoadSave", 0) == 1)
        {
            PlayerPrefs.DeleteKey("LoadSave");
            LoadGame();
            StartGame();
        }

        UpdateUI();
    }

    void Start()
    {
        // Reset speeds here because playerScript/spawner are guaranteed initialized
        if (PlayerPrefs.GetInt("LoadSave", 0) != 1)
        {
            var defaults = new Game.SaveIntegration.Data.GameSaveData();
            if (playerScript) playerScript.moveSpeed = defaults.moveSpeed;
            if (spawner)      spawner.blockFallSpeed  = defaults.blockFallSpeed;
            if (spawner)      spawner.spawnInterval   = defaults.spawnInterval;
        }
    }

    void LateUpdate()
    {
        UpdateUI();
    }

    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        if (player) player.SetActive(true);
        if (TextTapToStart) TextTapToStart.SetActive(false);
        if (pauseButton) pauseButton.SetActive(true);

        StartCoroutine(EnableSpawnerAfterDelay(1f));
    }

    private IEnumerator EnableSpawnerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spawner) spawner.enabled = true;
    }

    public void PlayerHit()
    {
        lives--;
        if (lives <= 0)
        {
            GameSaveManager.Delete();
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
    }

    public void AddScore()
    {
        score += GetPointsForLevel(level);
        UpdateUI();
        CheckLevelProgression();
    }

    private int GetPointsForLevel(int lvl) => 10 + (lvl - 1) * 5;

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
            if (playerScript != null) playerScript.IncreaseSpeed(level);
            ShowLevelUpText();
            UpdateUI();
            SaveGame();
        }
    }

    private int GetScoreThresholdForLevel(int lvl)
    {
        return Mathf.RoundToInt(100f * (lvl - 1) * lvl / 2f);
    }

    private void UpdateUI()
    {
        if (!uiText) return;
        uiText.ForceMeshUpdate();
        uiText.text =
            $"<size=120%><b>Level {level}</b></size>\n" +
            $"Score: <color=#00FF00><mspace=0.6em>{score}</mspace></color>\n" +
            $"Lives: <color=#FF4444>{lives}</color>";
        Canvas.ForceUpdateCanvases();
        uiText.canvasRenderer.SetAlpha(1f);
    }

    public void SaveGame()
    {
        var data = new Game.SaveIntegration.Data.GameSaveData
        {
            score          = score,
            level          = level,
            lives          = lives,
            playerX        = player.transform.position.x,
            playerY        = player.transform.position.y,
            moveSpeed      = playerScript.moveSpeed,
            blockFallSpeed = spawner.blockFallSpeed,
            spawnInterval  = spawner.spawnInterval
        };
        GameSaveManager.Save(data);
    }

    public void LoadGame()
    {
        // First check — show warning if needed
        SaveWarningService.CheckAndShow();

        if (GameSaveManager.HasSave())
        {
            var data = GameSaveManager.Load();

            if (!GameSaveManager.LastLoadWasCorrupted)
            {
                score = data.score;
                level = data.level;
                lives = data.lives;
                player.transform.position = new Vector3(data.playerX, data.playerY, 0f);
                playerScript.moveSpeed    = data.moveSpeed;
                spawner.blockFallSpeed    = data.blockFallSpeed;
                spawner.spawnInterval     = data.spawnInterval;
                UpdateUI();
                return;
            }
        }

        // No save or corrupted — defaults already set in Awake, reset speeds too
        var defaults = new Game.SaveIntegration.Data.GameSaveData();
        if (playerScript) playerScript.moveSpeed = defaults.moveSpeed;
        if (spawner)      spawner.blockFallSpeed  = defaults.blockFallSpeed;
        if (spawner)      spawner.spawnInterval   = defaults.spawnInterval;
        UpdateUI();
    }
}