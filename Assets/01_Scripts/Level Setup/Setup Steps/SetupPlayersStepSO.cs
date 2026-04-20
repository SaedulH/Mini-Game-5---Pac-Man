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
            await GameManager.Instance.SetupPlayer(context.PacManSkinIndex);

            // Setup AI
            await GameManager.Instance.SetupGhost(GhostType.Blinky, context.BlinkySkinIndex);          
            await GameManager.Instance.SetupGhost(GhostType.Inky, context.InkySkinIndex);          
            await GameManager.Instance.SetupGhost(GhostType.Pinky, context.PinkySkinIndex);          
            await GameManager.Instance.SetupGhost(GhostType.Clyde, context.ClydeSkinIndex);

            await GameManager.Instance.AssignTargetTransforms();
        }
    }
}

