using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [Serializable]
    public class LevelContext
    {
        [field: Header("Level")]
        public MapName MapName;
        public bool UseMapSeed = false;
        public int MapSeed;
        public bool RandomiseSeed = false;
        public int RemainingLives = 3;
        public int LevelNumber;

        [field: Header("Skins")]
        public int PacManSkinIndex = 0;
        public int BlinkySkinIndex = 0;
        public int PinkySkinIndex = 0;
        public int InkySkinIndex = 0;
        public int ClydeSkinIndex = 0;

        public float TotalWeight;
    }
}

