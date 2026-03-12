using Game.SaveIntegration.Data;
using UnityEngine;

namespace Game.SaveIntegration
{
    public class SaveSystemTester : MonoBehaviour
    {
        [ContextMenu("Test Save")]
        public void TestSave()
        {
            GameSaveData testData = new GameSaveData
            {
                saveVersion    = 1,
                score          = 150,
                level          = 2,
                lives          = 3,
                playerX        = 3.5f,
                playerY        = 1.0f,
                moveSpeed      = 5.5f,
                blockFallSpeed = 2.5f,
                spawnInterval  = 1.8f
            };

            GameSaveManager.Save(testData);
            Debug.Log("Test save completed.");
        }

        [ContextMenu("Test Load")]
        public void TestLoad()
        {
            GameSaveData data = GameSaveManager.Load();

            Debug.Log("=== Loaded save data ===");
            Debug.Log("Version: "        + data.saveVersion);
            Debug.Log("Score: "          + data.score);
            Debug.Log("Level: "          + data.level);
            Debug.Log("Lives: "          + data.lives);
            Debug.Log("Player X: "       + data.playerX);
            Debug.Log("Player Y: "       + data.playerY);
            Debug.Log("Move speed: "     + data.moveSpeed);
            Debug.Log("Block speed: "    + data.blockFallSpeed);
            Debug.Log("Spawn interval: " + data.spawnInterval);
        }

        [ContextMenu("Delete Save")]
        public void DeleteSave()
        {
            GameSaveManager.Delete();
            Debug.Log("Save file deleted.");
        }

        [ContextMenu("Check Save Exists")]
        public void CheckSaveExists()
        {
            Debug.Log("Save exists: " + GameSaveManager.HasSave());
        }
    }
}