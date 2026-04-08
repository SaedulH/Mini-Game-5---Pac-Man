using UnityEngine;

namespace Utilities
{
    public class NonPersistentSingleton<T> : MonoBehaviour where T : Component
    {
        public bool AutoUnparentOnAwake = true;
        protected static T instance;

        public static bool HasInstance => instance != null;
        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                }

                return instance;
            }
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;

            if (AutoUnparentOnAwake)
            {
                transform.SetParent(null);
            }
        }
    }
}