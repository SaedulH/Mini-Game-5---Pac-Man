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
            // Setup Player
            await GameManager.Instance.SetupPlayer(context.LevelNumber);

            // Setup AI
            await GameManager.Instance.SetupGhost(GhostType.Blinky, context.LevelNumber);          
            await GameManager.Instance.SetupGhost(GhostType.Inky, context.LevelNumber);          
            await GameManager.Instance.SetupGhost(GhostType.Pinky, context.LevelNumber);          
            await GameManager.Instance.SetupGhost(GhostType.Clive, context.LevelNumber);

            await GameManager.Instance.AssignTargetTransforms();
        }
    }
}

