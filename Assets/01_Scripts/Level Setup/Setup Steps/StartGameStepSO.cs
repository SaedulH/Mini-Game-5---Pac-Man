using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Start Game Step", menuName = "Levels/SetupSteps/StartGameStep")]
    public class StartGameStepSO : LevelSetupStepSO
    {
        [field: SerializeField] public float CountdownDuration { get; private set; } = 3f;
        public override async Task Run(LevelContext context)
        {
            if (context.LevelNumber > 0)
            {
                await UIManager.Instance.BeginCountdown(CountdownDuration);
            }
        }
    }
}

