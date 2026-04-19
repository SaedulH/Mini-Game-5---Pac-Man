using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Spawnpoint Step", menuName = "Levels/SetupSteps/SetupSpawnpointStep")]
    public class SetupSpawnpointStepSO : LevelSetupStepSO
    {
        [field: SerializeField] private Vector3 PacManSpawnPosition;
        [field: SerializeField] private Quaternion PlayerOneSpawnRotation;

        [field: SerializeField, Tooltip("[0: Blinky, 1: Inky, 2: Pinky, 3: Clyde]")] private Vector3[] GhostSpawnPosition;
        [field: SerializeField, Tooltip("[0: Blinky, 1: Inky, 2: Pinky, 3: Clyde]")] private Quaternion[] GhostSpawnRotation;

        public override async Task Run(LevelContext context)
        {
            GameManager.Instance.PacMan.SetSpawnpoint(PacManSpawnPosition, PlayerOneSpawnRotation);
            if (GameManager.Instance.Ghosts != null && GameManager.Instance.Ghosts.Length > 0)
            {
                for (int i = 0; i < GameManager.Instance.Ghosts.Length; i++)
                {
                    GameManager.Instance.Ghosts[i].SetSpawnpoint(GhostSpawnPosition[i], GhostSpawnRotation[i]);
                }
            }

            await Task.CompletedTask;
        }
    }
}