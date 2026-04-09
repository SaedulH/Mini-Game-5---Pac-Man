using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Map Step", menuName = "Levels/SetupSteps/SetupMapStep")]
    public class SetupMapStepSO : LevelSetupStepSO
    {
        [SerializeField] private AssetReference sceneReference;

        public override async Task Run(LevelContext context)
        {
            await SceneLoader.LoadContentScene(sceneReference);
        }
    }
}

