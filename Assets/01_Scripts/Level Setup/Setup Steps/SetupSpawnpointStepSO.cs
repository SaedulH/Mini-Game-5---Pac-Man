using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Spawnpoint Step", menuName = "Levels/SetupSteps/SetupSpawnpointStep")]
    public class SetupSpawnpointStepSO : LevelSetupStepSO
    {
        [SerializeField] private Vector3 PacManSpawnPosition;
        [SerializeField] private Quaternion PlayerOneSpawnRotation;

        [SerializeField] private Vector3[] GhostSpawnPosition;
        [SerializeField] private Quaternion[] GhostSpawnRotation;

        public override async Task Run(LevelContext context)
        {
            GameManager.Instance.PacMan.SetSpawnpoint(PacManSpawnPosition, PlayerOneSpawnRotation);
            //if (GameManager.Instance.Ghosts != null && GameManager.Instance.Ghosts.Length > 0)
            //{
            //    for (int i = 0; i < GameManager.Instance.Ghosts.Length; i++)
            //    {
            //        GameManager.Instance.Ghosts[i].transform.SetPositionAndRotation(GhostSpawnPosition[i], GhostSpawnRotation[i]);
            //    }
            //}

            await Task.CompletedTask;
        }
    }
}