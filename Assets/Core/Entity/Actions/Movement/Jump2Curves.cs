using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public class Jump2Curves : JumpAction
    {
        [SerializeField] protected AnimationCurve jumpCurve, fallCurve;
        [SerializeField] protected float jumpSpeed = 8, fallSpeed = 11;
        [SerializeField] protected float jumpTime = 0.6f, fallTime = 0.45f;
        private float time = 0, jumpPercent, fallPercent;
        private float verticalVelocity;
        private bool goingUp = true, isDone = false;
        public override void UpdateAction()
        {
            base.UpdateAction();
            if (hasStarted && !isDone)
            {
                time += Time.deltaTime;
                if (goingUp)
                {
                    jumpPercent = time / jumpTime;
                    if (jumpPercent >= 1)
                    {
                        time = 0;
                        goingUp = false;
                    }
                }
                else
                {
                    fallPercent = time / fallTime;
                }
                if (fallPercent >= 1)
                    isDone = true;
                verticalVelocity = goingUp ? jumpCurve.Evaluate(jumpPercent) * jumpSpeed : fallCurve.Evaluate(fallPercent) * fallSpeed;
                if (isDone)
                    verticalVelocity = fallCurve.Evaluate(1) * fallSpeed;
                rb.velocity = new Vector2(rb.velocity.x, verticalVelocity);
            }
        }
        public override void StartAction()
        {
            if (hasStarted)
                return;
            base.StartAction();
            time = 0;
            goingUp = true;
            isDone = false;
            jumpPercent = 0;
            fallPercent = 0;
        }
    }
}
