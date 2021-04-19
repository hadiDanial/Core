using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{

    public abstract class Action : MonoBehaviour
    {
        [SerializeField] internal float waitTime = 0.2f;

        internal AIDestinationSetter aiDestinationSetter;
        internal Coroutine actionCoroutine;
        internal ActionData data;
        internal bool hasStarted = false;
        internal virtual void Initialize(AIDestinationSetter aiDestinationSetter, ActionData data)
        {
            this.aiDestinationSetter = aiDestinationSetter;
            this.data = data;
        }

        internal virtual void Execute()
        {
            if(hasStarted)
                Stop();
            hasStarted = true;
            Resume();
        }

        internal virtual void Stop()
        {
            if (actionCoroutine != null)
                StopCoroutine(actionCoroutine);
        }

        internal virtual void Resume()
        {
            actionCoroutine = StartCoroutine(PerformAction());
        }

        internal virtual void Kill()
        {
            Stop();
            hasStarted = false;
        }

        internal abstract IEnumerator PerformAction();
    }
}
