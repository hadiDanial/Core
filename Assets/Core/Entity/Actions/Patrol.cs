using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public class Patrol : Action
    {
        [SerializeField] public List<Transform> patrolPoints;

        int patrolIndex;
      
        protected override IEnumerator PerformAction()
        {
            aiDestinationSetter.SetTarget(patrolPoints[patrolIndex]);
            WaitForSeconds wait = new WaitForSeconds(waitTime);
            while(true)
            {
                if (aiDestinationSetter.IsDone())
                {
                    patrolIndex++;
                    patrolIndex = patrolIndex % patrolPoints.Count;
                    aiDestinationSetter.SetTarget(patrolPoints[patrolIndex]);
                    aiDestinationSetter.SetTarget(patrolPoints[patrolIndex]);
                }
                data.movementDirection = (patrolPoints[patrolIndex].position - transform.position).normalized;
                yield return wait;
            }
        }
    }
}
