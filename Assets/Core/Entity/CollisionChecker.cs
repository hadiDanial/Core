using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{

    public class CollisionChecker : MonoBehaviour
    {
        [Header("Ground Check")]
        [SerializeField] protected List<GameObject> groundChecks = new List<GameObject>();
        [SerializeField] protected LayerMask groundMask;
        [SerializeField] internal float groundCheckDistance = 0.1f;
        [Header("Side Check")]
        [SerializeField] protected List<GameObject> sideChecksLeft = new List<GameObject>();
        [SerializeField] protected List<GameObject> sideChecksRight = new List<GameObject>();
        [SerializeField] protected LayerMask wallMask;
        [SerializeField] internal float sideCheckDistance = 0.075f;
        [Header("Head Check")]
        [SerializeField] protected List<GameObject> headChecks = new List<GameObject>();
        [SerializeField] internal float headCheckDistance = 0.075f;

        List<RaycastHit2D> groundHits = new List<RaycastHit2D>();
        List<RaycastHit2D> leftHits = new List<RaycastHit2D>();
        List<RaycastHit2D> rightHits = new List<RaycastHit2D>();
        List<RaycastHit2D> headHits = new List<RaycastHit2D>();

        /// <summary>
        /// Returns true if the Entity is grounded. 
        /// The entity is grounded if at least one of its ground checks returns true.
        /// </summary>
        public bool CheckIsGrounded()
        {
            groundHits.Clear();
            bool hasHit = false;
            foreach (GameObject groundCheck in groundChecks)
            {
                if (groundCheck == null)
                {
                    Debug.LogError(transform.name + ": Null Ground Check Object!");
                    return false;
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(groundCheck.transform.position, Vector2.down, groundCheckDistance, groundMask);
                    if (hit)
                        groundHits.Add(hit);
                    hasHit = hasHit || hit;
                }
            }
            return hasHit;
        }

        public bool CheckIsHittingHead()
        {
            headHits.Clear();
            bool hasHit = false;
            foreach (GameObject headCheck in headChecks)
            {
                if (headCheck == null)
                {
                    Debug.LogError(transform.name + ": No Head Check Object!");
                    return false;
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(headCheck.transform.position, Vector2.up, headCheckDistance, groundMask);
                    if (hit)
                        headHits.Add(hit);
                    hasHit = hasHit || hit;
                }
            }
            return hasHit;
        }

        public bool CheckIsHittingSide(int sign)
        {
            leftHits.Clear();
            rightHits.Clear();
            List<GameObject> currentSideList;
            List<RaycastHit2D> currentHits;
            bool hasHit = false;
            switch (sign)
            {
                case 1:
                    currentSideList = sideChecksRight;
                    currentHits = rightHits;
                    break;
                case -1:
                    currentSideList = sideChecksLeft;
                    currentHits = leftHits;
                    break;
                default:
                    currentSideList = sideChecksRight;
                    currentHits = rightHits;
                    break;
            }
            foreach (GameObject sideCheck in currentSideList)
            {
                if (sideCheck == null)
                {
                    Debug.LogError(transform.name + ": Side Check Object is Null! " + sign);
                    return false;
                }
                RaycastHit2D hit = Physics2D.Raycast(sideCheck.transform.position, Vector2.right * sign, sideCheckDistance, wallMask);
                if (hit)
                    currentHits.Add(hit);
                hasHit = hasHit || hit;
            }
            // Add walljump check
            return hasHit;
        }

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (GameObject groundCheck in groundChecks)
            {
                if (groundCheck != null)
                    Gizmos.DrawLine(groundCheck.transform.position, groundCheck.transform.position + Vector3.down * groundCheckDistance);
            }
            Gizmos.color = Color.green;
            foreach (GameObject sideCheck in sideChecksRight)
            {
                if (sideCheck != null)
                    Gizmos.DrawLine(sideCheck.transform.position, sideCheck.transform.position + Vector3.right * sideCheckDistance);
            }
            foreach (GameObject sideCheck in sideChecksLeft)
            {
                if (sideCheck != null)
                    Gizmos.DrawLine(sideCheck.transform.position, sideCheck.transform.position + Vector3.left * sideCheckDistance);
            }
            Gizmos.color = Color.yellow;
            foreach (GameObject headCheck in headChecks)
            {
                if(headCheck != null)
                    Gizmos.DrawLine(headCheck.transform.position, headCheck.transform.position + Vector3.up * headCheckDistance);
            }
        }
    }

}