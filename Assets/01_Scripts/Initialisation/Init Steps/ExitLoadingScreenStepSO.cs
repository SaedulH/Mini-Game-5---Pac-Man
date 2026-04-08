using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Exit Loading Screen Step", menuName = "Levels/InitSteps/ExitLoadingScreenStep")]
    public class ExitLoadingScreenStepSO : LevelInitStepSO
    {
        public override async Task Run(LevelContext context)
        {
            await LoadingScreen.Instance.HideLoadingScreen();
        }
    }
}

