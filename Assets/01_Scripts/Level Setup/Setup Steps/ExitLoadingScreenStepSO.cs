using System.Threading.Tasks;
using UnityEngine;
using UserInterface;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Exit Loading Screen Step", menuName = "Levels/SetupSteps/ExitLoadingScreenStep")]
    public class ExitLoadingScreenStepSO : LevelSetupStepSO
    {
        public override async Task Run(LevelContext context)
        {
            await LoadingScreen.Instance.HideLoadingScreen();
        }
    }
}

