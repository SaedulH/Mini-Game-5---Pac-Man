using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Players Step", menuName = "Levels/SetupSteps/SetupPlayersStep")]
    public class SetupPlayersStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {
            await GameManager.Instance.SetupEntities(context.LevelNumber);
        }
    }
}

