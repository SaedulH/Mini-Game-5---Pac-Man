using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "TrackInfo", menuName = "Track/TrackInfo")]
    public class LevelInfo : ScriptableObject
    {
        [field: SerializeField] public string LevelName { get; private set; }
        [field: SerializeField] public int LevelIndex { get; private set; }
        [field: SerializeField] public Sprite LevelImage { get; private set; }
        [field: SerializeField] public List<LevelInitStepSO> StepOrder { get; private set; }
        [field: SerializeField, Range(min: 1, max: 10)] public int RaceLapCount { get; private set; }
        [field: SerializeField, Range(min: 1, max: 10)] public int TimedLapCount { get; private set; }

        public int GetLapCountForMode(GameMode gameMode)
        {
            if(gameMode == GameMode.Timed)
            {
                return TimedLapCount;
            }
            else if(gameMode == GameMode.Race)
            {
                return RaceLapCount;
            }

            return Constants.DEFAULT_LAP_COUNT;
        }
    }
}

