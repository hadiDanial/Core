using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public class JumpMinMax : JumpAction
    {
        [SerializeField] protected float minJumpHeight = 1.6f, maxJumpHeight = 4.1f;
        [SerializeField] protected float jumpSpeed = 9.5f;
        [SerializeField] private float verticalSpeed;
        protected float initialHeight, currentHeight;
        protected float finalMinPos, finalMaxPos;
        private bool goingUp = true;
        private bool isBufferJump = false;

        public override void Initialize(Entity entity, AIDestinationSetter aiDestinationSetter)
        {
            base.Initialize(entity, aiDestinationSetter);
            canBeCancelled = true;
        }
        public override void UpdateAction()
        {
            base.UpdateAction();
            if (hasStarted)
            {
                currentHeight = transform.position.y;
                if (currentHeight <= finalMinPos || (goingUp && currentHeight < finalMaxPos))
                    verticalSpeed = GetVerticalSpeed();
                else// if (currentHeight >= finalMaxPos)
                {
                    StopAction();
                    verticalSpeed = 0;
                    goingUp = false;
                }
                rb.velocity = new Vector2(rb.velocity.x, verticalSpeed);
            }
            else if(entity.isGrounded)
            {
                if(isWaitingOnBuffer)
                {
                    StartAction();
                }
            }
        }

        internal virtual float GetVerticalSpeed()
        {
            return jumpSpeed;
        }

        internal override void HandleJumpBuffer()
        {
            if (isBufferJump && isWaitingOnBuffer && entity.isGrounded && jumpBufferTimer < jumpBufferPeriod)
            {
                jumpBufferTimer = jumpBufferPeriod;
                StartAction();
            }
            else if (jumpBufferTimer > jumpBufferPeriod)
            {
                isWaitingOnBuffer = false;
            }
        }
        public override void StartAction()
        {
            if (!jumpAllowed || !entity.IsActive())
                return;
            base.StartAction();
            if (canJump && !goingUp)
            {
                hasJumped = true;
                goingUp = isBufferJump ? false : true;
                isBufferJump = false;
                initialHeight = transform.position.y;
                finalMinPos = minJumpHeight + initialHeight;
                finalMaxPos = maxJumpHeight + initialHeight;
            }
        }

        public override void StopAction()
        {
            base.StopAction();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            goingUp = false;
        }

        internal override void CancelJump()
        {
            goingUp = false;
            isBufferJump = isWaitingOnBuffer ? true : false;
        }

        protected override bool IsGoingUp()
        {
            return goingUp;
        }
    }
}
