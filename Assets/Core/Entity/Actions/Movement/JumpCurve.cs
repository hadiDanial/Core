using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public class JumpCurve : JumpAction
    {
        [SerializeField] protected AnimationCurve jumpCurve;
        [SerializeField] protected float jumpSpeed = 8;
        [SerializeField] protected float jumpTime = 0.8f;
        private float time = 0, jumpPercent;
        private float verticalVelocity;
        private bool isDone = false;
        public override void UpdateAction()
        {
            base.UpdateAction();
            if (hasStarted && !isDone)
            {
                time += Time.deltaTime;
                jumpPercent = time / jumpTime;
                if (jumpPercent >= 1)
                {
                    time = 0;
                }            
                if (jumpPercent >= 1)
                    isDone = true;
                verticalVelocity = jumpCurve.Evaluate(jumpPercent) * jumpSpeed;
                if (isDone)
                    verticalVelocity = jumpCurve.Evaluate(1) * jumpSpeed;
                rb.velocity = new Vector2(rb.velocity.x, verticalVelocity);
            }
        }
        public override void StartAction()
        {
            if (hasStarted)
                return;
            base.StartAction();
            time = 0;
            isDone = false;
            jumpPercent = 0;
        }
    }
}
