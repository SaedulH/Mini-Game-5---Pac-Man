using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CoreSystem
{
    public enum SceneName
    {
        None,
        GameScene,
    }

    /// <summary>
    /// Bootstrapper is responsible for loading the core scenes of the game.
    /// It first loads the PersistentScene additively to ensure that essential managers and systems remain active throughout the game.
    /// After that, it loads the MainMenu scene, which serves as the entry point for players.
    /// </summary>
    [DefaultExecutionOrder(-100)] // Ensure this runs before other scripts
    public class Bootstrapper : MonoBehaviour
    {
        [field: SerializeField] private SceneName SceneName = SceneName.GameScene;
        [field: SerializeField] private bool UsePreset = false;
        [field: SerializeField] private ScenePreset ScenePreset;

        private async void Start()
        {
            Scene bootstrapScene = SceneManager.GetActiveScene();
            if (bootstrapScene.name != "Bootstrapper")
            {
                Debug.LogError("Bootstrapper script is not in the Bootstrapper scene.");
                return;
            }

            // Load CoreScene
            AsyncOperationHandle<SceneInstance> coreHandle = Addressables.LoadSceneAsync("CoreScene", LoadSceneMode.Additive);
            await coreHandle.Task;

            if (coreHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load CoreScene.");
                return;
            }
            SceneRegistry.CoreScene = coreHandle;
            Debug.Log("CoreScene loaded.");

            if (UsePreset && ScenePreset != null)
            {
                // Load Preset Scene
                await GameManager.Instance.SetupScene(ScenePreset.TrackInfo, ScenePreset.LevelContext);

                //// 3. Unload the Bootstrapper scene
                await SceneManager.UnloadSceneAsync(bootstrapScene);
            }
            else
            {
                if(SceneName == SceneName.None)
                {
                    Debug.LogError("No Additive Scene specified to load.");
                    return;
                }

                string sceneName = SceneName.ToString() != "" ? SceneName.ToString() : "GameScene";

                // Load MainMenu
                AsyncOperationHandle<SceneInstance> menuHandle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                await menuHandle.Task;

                if (menuHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError("Failed to load GameScene.");
                    return;
                }
                SceneRegistry.CurrentContent = menuHandle;
                SceneManager.SetActiveScene(menuHandle.Result.Scene);
                Debug.Log("GameScene loaded.");

                // 3. Unload the Bootstrapper scene
                await SceneManager.UnloadSceneAsync(bootstrapScene);
            }
        }
    }
}