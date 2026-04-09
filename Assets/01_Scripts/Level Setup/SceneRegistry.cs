using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace CoreSystem
{
    /// <summary>
    /// Central authority for all Addressables-loaded scenes.
    /// This is the single source of truth for scene lifetime.
    /// </summary>
    public static class SceneRegistry
    {
        public static AsyncOperationHandle<SceneInstance> CoreScene;
        public static AsyncOperationHandle<SceneInstance> CurrentContent;

        public static bool HasCoreScene =>
            CoreScene.IsValid();

        public static bool HasContentScene =>
            CurrentContent.IsValid();
    }
}
