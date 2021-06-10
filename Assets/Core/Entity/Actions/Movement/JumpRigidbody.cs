using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public class JumpRigidbody : JumpAction
    {
        [SerializeField] protected float jumpForce = 10;

        public override void StartAction()
        {
            base.StartAction();
            Vector2 doubleJumpBoost = Vector2.up;
            if (!jumpAllowed || !entity.IsActive())
                return;
            // Jump
            if (canJump && !hasJumped)
            {
                hasJumped = true;
                rb.AddForce(Vector2.up * jumpForce * 10, ForceMode2D.Impulse);
            }
            // Double Jump
            else if (canDoubleJump && hasJumped && !hasDoubleJumped)
            {
                // Jump boost for if the player is falling
                if (rb.velocity.y <= 0.1f) doubleJumpBoost = doubleJumpBoost + Vector2.up * Mathf.Clamp01(Mathf.Abs(rb.velocity.y));
                hasDoubleJumped = true;
                rb.AddForce(jumpForce * doubleJumpBoost * 8, ForceMode2D.Impulse);
            }
        }
       
    }
}
