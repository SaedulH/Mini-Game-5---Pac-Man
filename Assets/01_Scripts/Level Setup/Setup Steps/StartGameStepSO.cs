using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Start Game Step", menuName = "Levels/SetupSteps/StartGameStep")]
    public class StartGameStepSO : LevelSetupStepSO
    {
        [field: SerializeField] public float CountdownDuration { get; private set; } = 3f;
        public override async Task Run(LevelContext context)
        {
            if (context.StageNumber > 0)
            {
                await HUDManager.Instance.BeginCountdown(CountdownDuration);
            }
        }
    }
}

