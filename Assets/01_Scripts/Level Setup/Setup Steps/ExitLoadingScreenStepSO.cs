using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Exit Loading Screen Step", menuName = "Levels/SetupSteps/ExitLoadingScreenStep")]
    public class ExitLoadingScreenStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {
            GameManager.Instance.EnterGameState(GameState.Playing);
            await Task.CompletedTask;
        }
    }
}

