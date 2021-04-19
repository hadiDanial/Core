using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "Core/Game Event")]
    public class GameEvent : ScriptableObject
    {
        HashSet<GameEventListener> _listeners = new HashSet<GameEventListener>();

        public void Invoke()
        {
            foreach(GameEventListener listener in _listeners)
            {
                listener.RaiseEvent();
            }
        }

        public void Register(GameEventListener listener)
        {
            _listeners.Add(listener);
        }

        public void Deregister(GameEventListener listener)
        {
            _listeners.Remove(listener);
        }
    }
}

