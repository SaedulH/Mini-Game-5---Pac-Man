using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Loading Screen Step", menuName = "Levels/SetupSteps/SetupLoadingScreenStep")]
    public class SetupLoadingScreenStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {
            await LoadingScreen.Instance.SetLevelInfo(context);

            GameManager.Instance.EnterGameState(GameState.Loading);

            await LoadingScreen.Instance.ShowLoadingScreen();
        }
    }
}

