using System;

namespace Game.SaveIntegration.Data
{
    [Serializable]
    public class GameSaveData
    {
        public int   saveVersion   = 1;
        public int   score         = 0;
        public int   level         = 1;
        public int   lives         = 10;
        public float playerX       = 0.12f;
        public float playerY       = -3.26f;
        public float moveSpeed     = 5f;
        public float blockFallSpeed = 2f;
        public float spawnInterval = 2f;
    }
}