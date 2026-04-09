using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Enter GameState Step", menuName = "Levels/SetupSteps/EnterGameStateStep")]
    public class EnterGameStateStepSO : LevelSetupStepSO
    {
        [SerializeField] private GameState NewGameState;

        public override async Task Run(LevelContext context)
        {
            GameManager.Instance.EnterGameState(NewGameState);

            await Task.Yield();
        }
    }
}

