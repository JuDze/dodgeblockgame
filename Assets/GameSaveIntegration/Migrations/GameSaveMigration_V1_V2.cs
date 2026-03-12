using Game.SaveIntegration.Data;
using SaveSystem.Versioning;

namespace Game.SaveIntegration.Migrations
{
    // Example migration from save version 1 to version 2.
    // Uncomment and register in SaveSystemTester when adding new fields to GameSaveData.
    public class GameSaveMigration_V1_V2 : ISaveMigration<GameSaveData>
    {
        public int FromVersion => 1;
        public int ToVersion   => 2;

        public GameSaveData Migrate(GameSaveData old)
        {
            // Example: set default value for a newly added field in V2
            // old.newField = "default_value";
            return old;
        }
    }
}
