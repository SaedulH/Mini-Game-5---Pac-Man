using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace CoreSystem
{
    public static class SceneLoader
    {
        public static async Task LoadContentScene(AssetReference sceneRef)
        {
            // 1. Unload current content (if any)
            if (SceneRegistry.HasContentScene)
            {
                await Addressables
                    .UnloadSceneAsync(SceneRegistry.CurrentContent, true)
                    .Task;

                SceneRegistry.CurrentContent = default;
            }

            // 2. Load new content
            var newHandle = sceneRef.LoadSceneAsync(LoadSceneMode.Additive);
            await newHandle.Task;

            if (newHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load scene: {sceneRef.RuntimeKey}");
                return;
            }

            SceneRegistry.CurrentContent = newHandle;
            SceneManager.SetActiveScene(newHandle.Result.Scene);
        }
    }
}
