using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Enter GameState Step", menuName = "Levels/InitSteps/EnterGameStateStep")]
    public class EnterGameStateStepSO : LevelInitStepSO
    {
        [SerializeField] private GameState NewGameState;

        public override async Task Run(LevelContext context)
        {
            GameManager.Instance.EnterGameState(NewGameState);

            await Task.Yield();
        }
    }
}

