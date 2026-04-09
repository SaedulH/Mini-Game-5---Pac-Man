using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Step", menuName = "Levels/SetupSteps/SetupLightingStep")]
    public class SetupLightingStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {
            await Task.CompletedTask;
        }
    }
}

