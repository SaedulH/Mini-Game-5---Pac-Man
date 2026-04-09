using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Global Volume Step", menuName = "Levels/SetupSteps/SetupGlobalVolumeStep")]
    public class SetupGlobalVolumeStepSO : LevelSetupStepSO
    {
        [SerializeField] private GameObject globalVolumeObject;

        public override async Task Run(LevelContext context)
        {
            GameObject volumeObject = Instantiate(globalVolumeObject);

            GlobalVolumeManager volumeManager = volumeObject.GetOrAdd<GlobalVolumeManager>();

            volumeManager.ResetAll();

            await Task.CompletedTask;
        }
    }
}

