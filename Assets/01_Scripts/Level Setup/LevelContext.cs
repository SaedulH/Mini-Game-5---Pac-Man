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

        public float TotalWeight;
    }
}

