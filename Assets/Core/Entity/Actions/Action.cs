using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{

    public abstract class Action : MonoBehaviour
    {
        [SerializeField] protected float waitTime = 0.2f;

        protected AIDestinationSetter aiDestinationSetter;
        protected Coroutine actionCoroutine;
        protected ActionData data;
        protected bool hasStarted = false;
        public virtual void Initialize(AIDestinationSetter aiDestinationSetter, ActionData data)
        {
            this.aiDestinationSetter = aiDestinationSetter;
            this.data = data;
        }

        public virtual void Execute()
        {
            if(hasStarted)
                Stop();
            hasStarted = true;
            Resume();
        }

        public virtual void Stop()
        {
            if (actionCoroutine != null)
                StopCoroutine(actionCoroutine);
        }

        public virtual void Resume()
        {
            actionCoroutine = StartCoroutine(PerformAction());
        }

        public virtual void Kill()
        {
            Stop();
            hasStarted = false;
        }

        protected abstract IEnumerator PerformAction();
    }
}
