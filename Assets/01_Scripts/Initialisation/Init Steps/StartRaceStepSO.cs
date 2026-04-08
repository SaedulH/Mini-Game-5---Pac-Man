using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Start Race Step", menuName = "Levels/InitSteps/StartRaceStep")]
    public class StartRaceStepSO : LevelInitStepSO
    {
        [field: SerializeField] public float CountdownDuration { get; private set; } = 3f;
        public override async Task Run(LevelContext context)
        {
            await HUDManager.Instance.BeginCountdown(CountdownDuration);
        }
    }
}

