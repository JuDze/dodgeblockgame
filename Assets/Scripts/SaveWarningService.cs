using UnityEngine;

public class SaveWarningService : MonoBehaviour
{
    public static SaveWarningService Instance { get; private set; }

    [SerializeField] private TMPro.TextMeshProUGUI warningText;

    private void Awake()
    {
        Instance = this;
    }

    // Call this anywhere in the game
    public static void CheckAndShow()
    {
        Debug.Log("CheckAndShow called. Instance=" + Instance);
        
        if (Instance == null)
        {
            Debug.LogWarning("SaveWarningService instance is null!");
            return;
        }

        if (!GameSaveManager.HasSave())
        {
            Debug.Log("No save file found!");
            Instance.Show("⚠ Save file not found!");
        }
        else
        {
            GameSaveManager.Load();
            Debug.Log("LastLoadWasCorrupted=" + GameSaveManager.LastLoadWasCorrupted);
            if (GameSaveManager.LastLoadWasCorrupted)
                Instance.Show("⚠ Save file corrupted. Starting fresh!");
        }
    }

    private void Show(string message)
    {
        StartCoroutine(ShowRoutine(message));
    }

    private System.Collections.IEnumerator ShowRoutine(string message)
    {
        if (!warningText) yield break;
        warningText.text = message;
        warningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        warningText.gameObject.SetActive(false);
    }
}