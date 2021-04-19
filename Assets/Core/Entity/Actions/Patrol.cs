using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Patrol : Action
    {
        [SerializeField] public List<Transform> patrolPoints;

        int patrolIndex;
      
        internal override IEnumerator PerformAction()
        {
            WaitForSeconds wait = new WaitForSeconds(waitTime);
            while(true)
            {
                if (aiDestinationSetter.IsDone())
                {
                    patrolIndex++;
                    patrolIndex = patrolIndex % patrolPoints.Count;
                    aiDestinationSetter.SetTarget(patrolPoints[patrolIndex]);
                }
                data.movementDirection = (patrolPoints[patrolIndex].position - transform.position).normalized;
                yield return wait;
            }
        }
    }
}
