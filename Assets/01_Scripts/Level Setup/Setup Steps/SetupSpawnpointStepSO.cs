using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Spawnpoint Step", menuName = "Levels/SetupSteps/SetupSpawnpointStep")]
    public class SetupSpawnpointStepSO : LevelSetupStepSO
    {
        [field: SerializeField] private Vector3 PacManSpawnPosition;
        [field: SerializeField] private Quaternion PlayerOneSpawnRotation;

        [field: SerializeField, Tooltip("[0: Blinky, 1: Inky, 2: Pinky, 3: Clive]")] private Vector3[] GhostSpawnPosition;
        [field: SerializeField, Tooltip("[0: Blinky, 1: Inky, 2: Pinky, 3: Clive]")] private Quaternion[] GhostSpawnRotation;

        public override async Task Run(LevelContext context)
        {
            await GameManager.Instance.SetupEntitySpawnpoints(PacManSpawnPosition, PlayerOneSpawnRotation, 
                GhostSpawnPosition, GhostSpawnRotation);
        }
    }
}