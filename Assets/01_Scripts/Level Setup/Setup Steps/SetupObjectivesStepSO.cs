using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Step", menuName = "Levels/SetupSteps/SetupObjectivesStep")]
    public class SetupObjectivesStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {
            await Task.CompletedTask;
        }
    }
}

