using System;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{

    /// <summary>
    /// Initialises the level, including loading screen management and other setup tasks.
    /// </summary>
    public class LevelInitialiser : NonPersistentSingleton<LevelInitialiser>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public async Task InitialiseTrack(LevelInfo levelInfo, LevelContext levelContext)
        {
            float completed = 0f;
            float totalWeight = levelContext.TotalWeight;

            Debug.Log($"Initialise Level: {levelInfo.LevelName} - Mode:{levelContext.GameMode}," +
                $" Players:{levelContext.PlayerCount}");
            foreach (LevelInitStepSO step in levelInfo.StepOrder)
            {
                try
                {
                    //Debug.Log($"Starting: {step.name}");
                    await step.Run(levelContext);
                    completed += step.Weight;
                    LoadingScreen.Instance.UpdateLoadingProgress(completed);
                    //Debug.Log($"Completed: {step.name} ({completed}/{totalWeight})");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Init step {step.GetType().Name} failed: {ex}");
                    break;
                }
            }
        }
    }
}

