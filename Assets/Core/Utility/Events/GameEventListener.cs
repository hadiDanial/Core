using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{

    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] protected GameEvent gameEvent;
        [SerializeField] protected UnityEvent unityEvent;
        [Tooltip("If true, register/deregister on Enable/Disable, otherwise on Awake/Destroy")]
        [SerializeField] bool registerOnEnable = true;

        internal virtual void RaiseEvent()
        {
            unityEvent.Invoke();
        }

        private void Register()
        {
            gameEvent.Register(this);
        }
        private void Deregister()
        {
            gameEvent.Deregister(this);
        }

        private void Awake()
        {
            if (!registerOnEnable)
                Register();
        }

        private void OnDestroy()
        {
            if (!registerOnEnable)
                Deregister();
        }

        private void OnEnable()
        {
            if (registerOnEnable)
                Register();
        }

        private void OnDisable()
        {
            if (registerOnEnable)
                Deregister();
        }

    }

}