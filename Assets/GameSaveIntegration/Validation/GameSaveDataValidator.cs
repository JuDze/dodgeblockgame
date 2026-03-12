using Game.SaveIntegration.Data;
using SaveSystem.Validation;

namespace Game.SaveIntegration.Validation
{
    public class GameSaveDataValidator : ISaveDataValidator<GameSaveData>
    {
        public GameSaveData Validate(GameSaveData data)
        {
            var defaults = new GameSaveData();
            if (data.lives  <= 0) data.lives  = defaults.lives;
            if (data.level  < 1)  data.level  = defaults.level;
            if (data.score  < 0)  data.score  = defaults.score;
            if (data.moveSpeed      <= 0) data.moveSpeed      = defaults.moveSpeed;
            if (data.blockFallSpeed <= 0) data.blockFallSpeed = defaults.blockFallSpeed;
            if (data.spawnInterval  <= 0) data.spawnInterval  = defaults.spawnInterval;
            return data;
        }
    }
}