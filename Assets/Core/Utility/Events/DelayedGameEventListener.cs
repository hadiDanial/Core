using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class DelayedGameEventListener : GameEventListener
    {
        [SerializeField] float delay = 1f;
        [SerializeField] List<UnityEvent> delayedEvents;
        private Coroutine delayedCoroutine;
        internal override void RaiseEvent()
        {
            base.RaiseEvent();
            if(delayedCoroutine != null)
            {
                StopCoroutine(delayedCoroutine);
            }
            delayedCoroutine = StartCoroutine(DelayedEvent());
        }

        internal virtual IEnumerator DelayedEvent()
        {
            WaitForSeconds wait = new WaitForSeconds(delay);
            for (int i = 0; i < delayedEvents.Count; i++)
            {
                yield return wait;
                delayedEvents[i]?.Invoke();
            }
        }
    }

}