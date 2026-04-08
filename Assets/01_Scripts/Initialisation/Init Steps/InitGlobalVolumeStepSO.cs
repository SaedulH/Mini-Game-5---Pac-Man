using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Init Global Volume Step", menuName = "Levels/InitSteps/InitGlobalVolumeStep")]
    public class InitGlobalVolumeStepSO : LevelInitStepSO
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

