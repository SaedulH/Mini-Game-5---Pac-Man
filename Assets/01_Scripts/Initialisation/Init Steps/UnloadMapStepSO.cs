using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Unload Map Step", menuName = "Levels/InitSteps/UnloadMapStep")]
    public class UnloadMapStepSO : LevelInitStepSO
    {
        [SerializeField] private AssetReference sceneReference;

        public override async Task Run(LevelContext context)
        {
            await Task.CompletedTask;
            //AsyncOperationHandle<SceneInstance> handle = context.SceneHandle;
            //if (!handle.IsValid())
            //{
            //    Debug.LogWarning("No valid scene handle to unload.");
            //    return;
            //}

            //// Unload via Addressables
            //AsyncOperationHandle<SceneInstance> unloadHandle = Addressables.UnloadSceneAsync(handle);
            //await unloadHandle.Task;

            //if (unloadHandle.Status == AsyncOperationStatus.Succeeded)
            //{
            //    Debug.Log($"Scene unloaded successfully.");
            //}
            //else
            //{
            //    Debug.LogError($"Failed to unload scene {handle.Result.Scene.name}");
            //}

            //// Clear handle after unloading
            //context.SceneHandle = default;
        }
    }
}

