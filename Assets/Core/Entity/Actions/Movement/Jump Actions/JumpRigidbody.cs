using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public class JumpRigidbody : JumpAction
    {
        [SerializeField] protected float jumpForce = 2.1f;
        [SerializeField] protected bool canDoubleJump = false;
        [SerializeField, Tooltip("If true, the double jump resets vertical velocity. Otherwise, the velocity is relative to current velocity")]
        protected bool absoluteDoubleJump = true;
        public override void StartAction()
        {
            if (!jumpAllowed || !entity.IsActive())
                return;
            base.StartAction();
            // Jump
            if (canJump)
            {
                hasJumped = true;
                jumpTimeElapsed = -1;
                rb.AddForce(Vector2.up * jumpForce * 10, ForceMode2D.Impulse);
            }
            // Double Jump
            else if (canDoubleJump && hasJumped && !hasDoubleJumped)
            {
                Vector2 doubleJumpBoost = Vector2.up;
                // Jump boost for if the player is falling
                if (absoluteDoubleJump)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                }
                else
                {
                    if (rb.velocity.y <= 0.1f)
                        doubleJumpBoost = doubleJumpBoost + Vector2.up * Mathf.Clamp01(Mathf.Abs(rb.velocity.y));
                }
                rb.AddForce(jumpForce * doubleJumpBoost * 10, ForceMode2D.Impulse);
                hasDoubleJumped = true;
            }
        }

        protected override bool IsGoingUp()
        {
            return rb.velocity.y >= 0;
        }
    }
}
