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
        protected bool alwaysUpdate = false, activeAction = true;
        private float timer = 0;

        public virtual void Initialize(ActionData data)
        {
            this.data = data;
            if(data.aiDestinationSetter != null)
                aiDestinationSetter = data.aiDestinationSetter;
            entity = data.entity;
            rb = entity.rb;
            entity.health.OnKill += OnEntityDeath;
        }

        private void OnEntityDeath(bool deactivate = true)
        {
            StopAction();
        }

        public virtual void Update()
        {
            if (alwaysUpdate || (hasStarted && !isPaused))
                UpdateAction();
            timer += Time.deltaTime;
        }

        public abstract void UpdateAction();


        public virtual void StartAction()
        {
            hasStarted = true;
            isPaused = false;
            timer = 0;
        }

        public virtual void StopAction()
        {
            isPaused = true;
            hasStarted = false;
            timer = resetTime;
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
        protected virtual bool CanPerformAction()
        {
            return activeAction && timer >= resetTime && entity != null;
        }
        protected virtual IEnumerator PerformAction()
        {
            yield return new WaitForSeconds(resetTime);
        }
        public void SetActive(bool value)
        {
            activeAction = value;
        }

        private void OnDisable()
        {
            entity.health.OnKill -= OnEntityDeath;
            activeAction = false;
        }
    }
}
