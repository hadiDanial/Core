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
        [SerializeField, Range(0, 0.2f)]
        protected float jumpBufferPeriod = 0.08f;
        [SerializeField] protected bool jumpAllowed = true;

        internal float jumpTimeElapsed;
        internal float jumpBufferTimer;
        [SerializeField] protected bool stopOnHittingWall = true;
        
        protected bool canBeCancelled = false;

        internal bool canJump => !hasJumped && (entity.isGrounded || jumpTimeElapsed >= 0);
        internal bool hasJumped, hasDoubleJumped;
        internal bool isWaitingOnBuffer = false;

        public override void Initialize(ActionData data)
        {
            base.Initialize(data);
            alwaysUpdate = true;
            jumpBufferTimer = jumpBufferPeriod;
        }

        public override void UpdateAction()
        {
            if (!CanPerformAction())
                return;
            jumpTimeElapsed = entity.isGrounded ? jumpGracePeriod : jumpTimeElapsed - Time.deltaTime;
            jumpBufferTimer += Time.deltaTime;
            HandleJumpBuffer();
        }

        internal virtual void HandleJumpBuffer()
        {
            if (isWaitingOnBuffer && entity.isGrounded && jumpBufferTimer < jumpBufferPeriod)
            {
                jumpBufferTimer = jumpBufferPeriod;
                StartAction();
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (entity != null)
            {
                if (entity.isHittingHead || (stopOnHittingWall && entity.isHittingSide))
                    StopAction();
            }
        }

        internal virtual void CancelJump()
        {
            if (canBeCancelled)
                StopAction();
        }
    }
}
