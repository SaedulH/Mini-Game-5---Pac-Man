using UnityEngine;
using UnityEngine.Events;

namespace EventSystem
{
    public abstract class DualEventListener<T1, T2> : MonoBehaviour
    {
        [SerializeField] DualEventChannel<T1, T2> eventChannel;
        [SerializeField] UnityEvent<T1, T2> unityEvent;

        protected void Awake()
        {
            if (eventChannel != null)
                eventChannel.Register(this);
        }

        protected void OnDestroy()
        {
            if (eventChannel != null)
                eventChannel.Deregister(this);
        }

        public void Raise(T1 value1, T2 value2)
        {
            unityEvent?.Invoke(value1,value2);
        }

    }
}