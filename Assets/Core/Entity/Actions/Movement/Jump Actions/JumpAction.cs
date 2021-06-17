using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public abstract class JumpAction : Action
    {
        [SerializeField, Range(0, 0.4f)]
        protected float jumpGracePeriod = 0.1f;
        [SerializeField, Range(0, 0.35f)]
        protected float jumpBufferPeriod = 0.08f;
        [SerializeField] protected bool jumpAllowed = true;

        internal float jumpTimeElapsed;
        internal float jumpBufferTimer;
        [SerializeField] protected bool stopOnHittingWall = true;
        
        protected bool canBeCancelled = false;

        internal bool canJump => !hasJumped && (entity.isGrounded || jumpTimeElapsed >= 0);
        internal bool hasJumped, hasDoubleJumped;
        internal bool isWaitingOnBuffer = false;

        public override void Initialize(Entity entity, AIDestinationSetter aiDestinationSetter)
        {
            base.Initialize(entity, aiDestinationSetter);
            alwaysUpdate = true;
            jumpBufferTimer = jumpBufferPeriod;
        }

        public override void UpdateAction()
        {
            if (!CanPerformAction())
                return;
            if (entity.isHittingHead)
                StopAction();
            if (entity.isGrounded && hasStarted)
                StopAction();
            if (entity.isHittingSide)
            {
                if (!IsGoingUp() && hasStarted)
                    StopAction();
            }
            jumpTimeElapsed = entity.isGrounded ? jumpGracePeriod : jumpTimeElapsed - Time.deltaTime;
            jumpBufferTimer += Time.deltaTime;
            HandleJumpBuffer();
        }

        protected abstract bool IsGoingUp();

        internal virtual void HandleJumpBuffer()
        {
            if (isWaitingOnBuffer && entity.isGrounded && jumpBufferTimer < jumpBufferPeriod)
            {
                jumpBufferTimer = jumpBufferPeriod;
                StartAction();
            }
            else if(jumpBufferTimer > jumpBufferPeriod)
            {
                isWaitingOnBuffer = false;
            }
        }

        public override void StartAction()
        {
            if (!entity.isGrounded)
            {
                isWaitingOnBuffer = true;
                jumpBufferTimer = 0;
            }
            else
            {
                base.StartAction();
                isWaitingOnBuffer = false;
                jumpBufferTimer = jumpBufferPeriod;
            }
        }
        public override void StopAction()
        {
            base.StopAction();
            hasJumped = false;
            hasDoubleJumped = false;
        }

        internal virtual void CancelJump()
        {
            if (canBeCancelled)
                StopAction();
        }
    }
}
