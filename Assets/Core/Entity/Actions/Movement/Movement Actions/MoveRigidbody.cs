using UnityEngine;

namespace Core.Entities
{
    public class MoveRigidbody : MovementAction
    {
        public override void UpdateAction()
        {
            if (!canMove) 
                return;
            base.UpdateAction();
            SetMovementVector(movementVector);
            //isHittingSide = _isHittingSide;
            if (entity.IsActive())
            {
                if (movementVector != Vector2.zero)
                {
                    if (isHittingSide)
                    {
                        movementVector.x = 0;
                    }
                    else
                    {
                        currentMovementMultiplier = isGrounded ? 1 : airControlPercent;
                        rb.AddForce(movementVector * totalSpeedMultiplier * Time.deltaTime, ForceMode2D.Force);
                    }
                }
                else
                {
                    rb.AddForce(-rb.velocity.x * Vector2.right * totalSpeedMultiplier * stoppingPower * Time.deltaTime);
                }
                if (entity.useGravity)
                {
                    if (isGrounded)
                        rb.gravityScale = 0;
                    else
                        rb.gravityScale = (rb.velocity.y < 0f) ? fallGravity : normalGravity;
                }
            }
        }

        public override void Update()
        {

        }

        public void FixedUpdate()
        {
            if (hasStarted && !isPaused)
                UpdateAction();
        }
    }
}