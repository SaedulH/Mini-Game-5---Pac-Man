using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UserInterface;
using Utilities;

namespace CoreSystem
{

    /// <summary>
    /// Initialises the level, including loading screen management and other setup tasks.
    /// </summary>
    public class LevelSetupHandler : NonPersistentSingleton<LevelSetupHandler>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public async Task SetupLevel(LevelInfo levelInfo, LevelContext levelContext)
        {
            float completed = 0f;
            levelContext.TotalWeight = levelInfo.StepOrder.Sum(s => s.Weight);

            Debug.Log($"Setup Level: {levelInfo.LevelName} - Mode:{levelContext.LevelNumber}");
            foreach (LevelSetupStepSO step in levelInfo.StepOrder)
            {
                try
                {
                    //Debug.Log($"Starting: {step.name}");
                    await step.Run(levelContext);
                    completed += step.Weight;
                    UIManager.Instance.UpdateLoadingProgress(completed);
                    //Debug.Log($"Completed: {step.name} ({completed}/{totalWeight})");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Setup step {step.GetType().Name} failed: {ex}");
                    break;
                }
            }
        }
    }
}

