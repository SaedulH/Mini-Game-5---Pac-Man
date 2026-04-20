using UnityEngine;
using UnityEngine.Events;

namespace EventSystem
{
    public abstract class EventListener<T> : MonoBehaviour
    {
        [SerializeField] EventChannel<T> eventChannel;
        [SerializeField] UnityEvent<T> unityEvent;

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

        public void Raise(T value)
        {
            unityEvent?.Invoke(value);
        }

    }
    public class EventListener : EventListener<Empty> { }
}