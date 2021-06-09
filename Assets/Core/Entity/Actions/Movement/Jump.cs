using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public class Jump : Action
    {
        [SerializeField] protected float minJumpHeight, maxJumpHeight;
        [SerializeField] protected float jumpTime;
        [SerializeField] protected float jumpForce = 10;
        [SerializeField, Range(0, 0.4f)]
        protected float jumpGracePeriod = 0.1f;
        [SerializeField] protected bool jumpAllowed = true, canDoubleJump = false;
        [SerializeField, Range(0, 2)]
        protected float airControlPercent = 0.45f;
        [SerializeField] protected AnimationCurve minJumpCurve, maxJumpCurve, fallCurve;
        internal float jumpTimeElapsed;
        internal bool canJump => entity.isGrounded || jumpTimeElapsed >= 0;
        internal bool isCollided, hasJumped, hasDoubleJumped;


        public override void UpdateAction()
        {
            jumpTimeElapsed = entity.isGrounded ? jumpGracePeriod : jumpTimeElapsed - Time.deltaTime;
        }

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
        internal virtual void OnCollisionEnter2D(Collision2D collision)
        {
            isCollided = true;
            if (entity.isGrounded)
            {
                hasJumped = false;
                hasDoubleJumped = false;
            }
        }
    }
}
