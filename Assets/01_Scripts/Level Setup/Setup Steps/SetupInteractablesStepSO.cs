using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Step", menuName = "Levels/SetupSteps/SetupInteractablesStep")]
    public class SetupInteractablesStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {
            await Task.CompletedTask;
        }
    }
}

