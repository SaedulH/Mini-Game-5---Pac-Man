using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Init Step", menuName = "Levels/InitSteps/InitInteractablesStep")]
    public class InitInteractablesStepSO : LevelInitStepSO
    {
        public override async Task Run(LevelContext context)
        {
            await Task.CompletedTask;
        }
    }
}

