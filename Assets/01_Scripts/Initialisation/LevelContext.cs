using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [Serializable]
    public class LevelContext
    {
        public GameMode GameMode;
        [field: Range(min: 1, max: 2)] public int PlayerCount;
        public int LapCount;
        public float TotalWeight;
    }
}

