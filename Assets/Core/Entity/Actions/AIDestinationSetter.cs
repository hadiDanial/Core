using Core.Entities;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{

    public class AIDestinationSetter : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Entity entity;
        [SerializeField] private float nextWaypointDistance = 0.25f;
        [SerializeField] private float pathRepeatTime = 0.5f;

        private Seeker seeker;
        private Path path;
        private Transform prevTarget;
        private bool reachedEndOfPath;
        private bool waitForRepath;
        private int pathIndex = 0;

        private void Awake()
        {
            seeker = GetComponent<Seeker>();
            prevTarget = target;
            DebugWarnings();
        }

        private void Start()
        {
            InvokeRepeating("CalculatePath", 0, pathRepeatTime);
        }

        internal virtual void Update()
        {
            if (path == null || waitForRepath || entity.currentEntityState != EntityState.Active) return;
            if (pathIndex >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else reachedEndOfPath = false;

            float distance = Vector2.Distance(transform.position, path.vectorPath[pathIndex]);
            if (distance < nextWaypointDistance && pathIndex < path.vectorPath.Count)
                pathIndex++;
        }
        public void SetTarget(Transform newTarget)
        {
            waitForRepath = true;
            reachedEndOfPath = false;
            prevTarget = target;
            target = newTarget;
            pathIndex = 0;
        }

        public Vector2 GetDestinationDirection2D()
        {
            return ((Vector2)path.vectorPath[pathIndex] - (Vector2)transform.position).normalized;
        }
        public Vector3 GetDestinationDirection()
        {
            return (path.vectorPath[pathIndex] - transform.position).normalized;
        }
        public bool IsDone()
        {
            return reachedEndOfPath;
        }

        internal virtual void CalculatePath()
        {
            if (seeker.IsDone())
                seeker.StartPath(transform.position, target.position, OnPathComplete);
        }

        internal void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                pathIndex = 0;
                waitForRepath = false;
            }
        }

        internal virtual void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, nextWaypointDistance);
            Gizmos.color = Color.red;
        }

        private void DebugWarnings()
        {
            if (target == null)
                Debug.LogError(transform.name + " doesn't have a target!");
            if (entity == null)
                Debug.LogError(transform.name + " doesn't have an entity!");
        }
    }
}