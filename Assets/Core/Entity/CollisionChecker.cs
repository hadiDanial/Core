using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{

    public class CollisionChecker : MonoBehaviour
    {
        [Header("Ground Check")]
        [SerializeField] protected GameObject groundCheckLeft;
        [SerializeField] protected GameObject groundCheckRight;
        [SerializeField] protected GameObject groundCheckMiddle;
        [SerializeField] protected LayerMask groundMask;
        internal float groundCheckDistance = 0.1f;
        [Header("Side Check")]
        [SerializeField] protected GameObject sideCheckLeft;
        [SerializeField] protected GameObject sideCheckRight;
        [SerializeField] protected LayerMask wallMask;
        internal float sideCheckDistance = 0.1f;
        [Header("Head Check")]
        [SerializeField] protected GameObject headCheck;


        /// <summary>
        /// Returns true if the Entity is grounded. 
        /// The entity is grounded if at least one of its ground checks returns true.
        /// </summary>
        public bool CheckIsGrounded()
        {
            if (groundCheckLeft == null || groundCheckRight == null || groundCheckMiddle == null)
            {
                Debug.LogError(transform.name + ": No Ground Check Object!");
                return false;
            }
            RaycastHit2D hitL = Physics2D.Raycast(groundCheckLeft.transform.position, Vector2.down, groundCheckDistance, groundMask);
            RaycastHit2D hitR = Physics2D.Raycast(groundCheckRight.transform.position, Vector2.down, groundCheckDistance, groundMask);
            RaycastHit2D hitM = Physics2D.Raycast(groundCheckMiddle.transform.position, Vector2.down, groundCheckDistance, groundMask);
            if (hitR || hitL || hitM)
                return true;
            return false;
        }

        public bool CheckIsHittingHead()
        {
            if (headCheck == null)
            {
                Debug.LogError(transform.name + ": No Head Check Object!");
                return false;
            }
            RaycastHit2D hit = Physics2D.Raycast(headCheck.transform.position, Vector2.up, groundCheckDistance, groundMask);
            if (hit)
                return true;
            return false;
        }

        public bool CheckIsHittingSide(int sign)
        {
            if (sideCheckLeft == null || sideCheckRight == null)
            {
                Debug.LogError(transform.name + ": No Side Check Object!");
                return false;
            }
            GameObject check;
            switch (sign)
            {
                case 1:
                    check = sideCheckRight;
                    break;
                case 2:
                    check = sideCheckLeft;
                    break;
                default:
                    check = sideCheckRight;
                    break;
            }
            RaycastHit2D hit = Physics2D.Raycast(check.transform.position, Vector2.right * sign, sideCheckDistance, wallMask);
            // Add walljump check
            if (hit) return true;
            return false;

        }

        private void OnDrawGizmos()
        {
            if (groundCheckLeft == null || groundCheckRight == null || groundCheckMiddle == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheckRight.transform.position, groundCheckRight.transform.position + Vector3.down * groundCheckDistance);
            Gizmos.DrawLine(groundCheckLeft.transform.position, groundCheckLeft.transform.position + Vector3.down * groundCheckDistance);
            if (sideCheckLeft == null || sideCheckRight == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(sideCheckLeft.transform.position, sideCheckLeft.transform.position + Vector3.left * sideCheckDistance);
            Gizmos.DrawLine(sideCheckRight.transform.position, sideCheckRight.transform.position + Vector3.right * sideCheckDistance);
            if (headCheck == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(headCheck.transform.position, headCheck.transform.position + Vector3.up * groundCheckDistance);
            Gizmos.DrawLine(groundCheckLeft.transform.position, groundCheckLeft.transform.position + Vector3.down * groundCheckDistance);
        }
    }

}