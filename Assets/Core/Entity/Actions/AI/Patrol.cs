using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    [RequireComponent(typeof(AIDestinationSetter))]
    public class Patrol : Action
    {
        [SerializeField] public List<Transform> patrolPoints;

        int patrolIndex;
        public override void Initialize(Entity entity, AIDestinationSetter aiDestinationSetter)
        {
            base.Initialize(entity, aiDestinationSetter);
            if (aiDestinationSetter == null)
                this.aiDestinationSetter = GetComponent<AIDestinationSetter>();
        }
        public override void StartAction()
        {
            base.StartAction();
            aiDestinationSetter.SetTarget(patrolPoints[patrolIndex]);
        }

        public override void UpdateAction()
        {
            if (aiDestinationSetter.IsDone())
            {
                patrolIndex++;
                patrolIndex = patrolIndex % patrolPoints.Count;
                aiDestinationSetter.SetTarget(patrolPoints[patrolIndex]);
                aiDestinationSetter.SetTarget(patrolPoints[patrolIndex]);
            }
            entity.input = (patrolPoints[patrolIndex].position - transform.position).normalized;
        }
    }
}
