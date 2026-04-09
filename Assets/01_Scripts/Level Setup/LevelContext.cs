using System;

namespace CoreSystem
{
    [Serializable]
    public class LevelContext
    {
        public int PacManSkinIndex = 0;
        public int BlinkySkinIndex = 0;
        public int PinkySkinIndex = 0;
        public int InkySkinIndex = 0;
        public int ClydeSkinIndex = 0;
        public int RemainingLives = 3;
        public int StageNumber;
        public float TotalWeight;
    }
}

