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
        private float initialHeight, currentHeight;
        private float finalMinPos, finalMaxPos;
        private bool goingUp = true;
        private bool isBufferJump = false;

        public override void Initialize(ActionData data)
        {
            base.Initialize(data);
            canBeCancelled = true;
        }
        public override void UpdateAction()
        {
            base.UpdateAction();
            if (hasStarted && goingUp)
            {
                currentHeight = transform.position.y;
                if (currentHeight <= finalMinPos || (goingUp && currentHeight < finalMaxPos))
                    verticalSpeed = jumpSpeed;
                else// if (currentHeight >= finalMaxPos)
                {
                    StopAction();
                    verticalSpeed = 0;
                    goingUp = false;
                }
                rb.velocity = new Vector2(rb.velocity.x, verticalSpeed);
            }
        }

        public override void StartAction()
        {
            if (!jumpAllowed || !entity.IsActive())
                return;
            if (canJump && !goingUp)
            {
            base.StartAction();
                hasJumped = true;
                goingUp = isBufferJump ? false : true;
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
