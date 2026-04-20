using System.Collections.Generic;
using UnityEngine;

namespace EventSystem
{
    public abstract class DualEventChannel<T1, T2> : ScriptableObject
    {
        readonly HashSet<DualEventListener<T1, T2>> observers = new();

        public void Invoke(T1 value1, T2 value2)
        {
            foreach (var observer in observers)
            {
                observer.Raise(value1, value2);
            }
        }

        public void Register(DualEventListener<T1, T2> observer) => observers.Add(observer);
        public void Deregister(DualEventListener<T1, T2> observer) => observers.Remove(observer);
    }

}