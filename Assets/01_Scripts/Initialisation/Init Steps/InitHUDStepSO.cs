using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Init HUD Step", menuName = "Levels/InitSteps/InitHUDStep")]
    public class InitHUDStepSO : LevelInitStepSO
    {
        [field: SerializeField] public float GoldTime { get; private set; }
        [field: SerializeField] public float HardGoldTime { get; private set; }
        [field: SerializeField] public float SilverTime { get; private set; }
        [field: SerializeField] public float HardSilverTime { get; private set; }
        [field: SerializeField] public float BronzeTime { get; private set; }
        [field: SerializeField] public float HardBronzeTime { get; private set; }

        public override async Task Run(LevelContext context)
        {
            List<float> medalTimes;
            string difficulty = PlayerPrefs.GetString("Difficulty");
            if (Enum.TryParse(difficulty, out Difficulty parsedDifficulty))
            {
                medalTimes = new()
                {
                    GetGoldTimeForDifficulty(parsedDifficulty),
                    GetSilverTimeForDifficulty(parsedDifficulty),
                    GetBronzeTimeForDifficulty(parsedDifficulty)
                };
            }
            else
            {
                medalTimes = new()
                {
                    GoldTime,
                    SilverTime,
                    BronzeTime
                };
            }

            //await GameManager.Instance.InitialiseHUD(context, medalTimes);
        }

        public float GetBronzeTimeForDifficulty(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => BronzeTime,
                Difficulty.Hard => HardBronzeTime,
                _ => BronzeTime
            };
        }

        public float GetSilverTimeForDifficulty(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => SilverTime,
                Difficulty.Hard => HardSilverTime,
                _ => SilverTime
            };
        }

        public float GetGoldTimeForDifficulty(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => GoldTime,
                Difficulty.Hard => HardGoldTime,
                _ => GoldTime
            };
        }
    }
}

