using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Setup Maze Layout Step", menuName = "Levels/SetupSteps/SetupMazeLayoutStep")]
    public class SetupMazeLayoutStepSO : LevelSetupStepSO
    {
        [SerializeField] private AssetReference sceneReference;

        public override async Task Run(LevelContext context)
        {
            await SceneLoader.LoadContentScene(sceneReference);
        }
    }
}

