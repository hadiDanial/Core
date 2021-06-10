using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public abstract class JumpAction : Action
    {
        [SerializeField, Range(0, 0.4f)]
        protected float jumpGracePeriod = 0.1f;
        [SerializeField] protected bool jumpAllowed = true, canDoubleJump = false;
        internal float jumpTimeElapsed;

        internal bool canJump => entity.isGrounded || jumpTimeElapsed >= 0;
        internal bool hasJumped, hasDoubleJumped;

        public override void Initialize(ActionData data)
        {
            base.Initialize(data);
            alwaysUpdate = true;
        }

        public override void UpdateAction()
        {
            jumpTimeElapsed = entity.isGrounded ? jumpGracePeriod : jumpTimeElapsed - Time.deltaTime;
        }

        public override void StartAction()
        {
            base.StartAction();
            if(!hasJumped)
            {
                hasJumped = true;
            }
        }
        public override void StopAction()
        {
            base.StopAction();
            hasJumped = false;
            hasDoubleJumped = false;
        }
    }
}
