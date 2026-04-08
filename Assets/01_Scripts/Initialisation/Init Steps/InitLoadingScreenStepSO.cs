using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Init Loading Screen Step", menuName = "Levels/InitSteps/InitLoadingScreenStep")]
    public class InitLoadingScreenStepSO : LevelInitStepSO
    {
        [SerializeField] private string TrackTitle;
        [SerializeField] private string TrackDescription;
        [SerializeField] private Sprite LevelImage;

        public override async Task Run(LevelContext context)
        {
            await LoadingScreen.Instance.SetLevelInfo(TrackTitle, TrackDescription, LevelImage, context);

            GameManager.Instance.EnterGameState(GameState.Loading);

            await LoadingScreen.Instance.ShowLoadingScreen();
        }
    }
}

