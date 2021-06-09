using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{

    public abstract class Action : MonoBehaviour
    {
        [SerializeField] protected float resetTime = 0.2f;

        protected AIDestinationSetter aiDestinationSetter;
        protected Coroutine actionCoroutine;
        protected ActionData data;
        protected Entity entity;
        protected Rigidbody2D rb;
        protected bool hasStarted = false, isPaused = true;

        public virtual void Initialize(ActionData data)
        {
            this.data = data;
            aiDestinationSetter = data.aiDestinationSetter;
            entity = data.entity;
            rb = entity.rb;
        }
        public virtual void Update()
        {
            if (hasStarted && !isPaused)
                UpdateAction();
        }

        public abstract void UpdateAction();


        public virtual void StartAction()
        {
            hasStarted = true;
            isPaused = false;
        }

        public virtual void StopAction()
        {
            isPaused = true;
            hasStarted = false;
        }

        public virtual void Resume()
        {
            isPaused = false;
        }

        public virtual void Pause()
        {
            isPaused = true;
        }

        public virtual void KillAction()
        {
            StopAction();
            hasStarted = false;
            Destroy(this);
        }

        protected virtual IEnumerator PerformAction()
        {
            yield return new WaitForSeconds(resetTime);
        }
    }
}
