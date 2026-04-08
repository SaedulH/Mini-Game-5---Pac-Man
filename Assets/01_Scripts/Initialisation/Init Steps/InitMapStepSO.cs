using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Init Map Step", menuName = "Levels/InitSteps/InitMapStep")]
    public class InitMapStepSO : LevelInitStepSO
    {
        [SerializeField] private AssetReference sceneReference;

        public override async Task Run(LevelContext context)
        {
            await SceneLoader.LoadContentScene(sceneReference);
        }
    }
}

