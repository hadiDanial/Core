using UnityEngine;

namespace Core.Entities
{
    public class MoveRigidbody : MovementAction
    {
        public override void UpdateAction()
        {
            if (!canMove) 
                return;
            isGrounded = entity.isGrounded;
            SetMovementVector(movementVector);
            //isHittingSide = _isHittingSide;
            if (entity.IsActive())
            {
                //if (isHittingSide)
                //{
                //    movementVector.x = 0;
                //}
                //jumpTimeElapsed = isGrounded ? jumpGracePeriod : jumpTimeElapsed - Time.deltaTime;
                if (movementVector != Vector2.zero)
                {
                    currentMovementMultiplier = isGrounded ? 1 : airControlPercent;
                    rb.AddForce(movementVector * totalSpeedMultiplier * Time.deltaTime, ForceMode2D.Force);
                }
                else
                {
                    rb.AddForce(-rb.velocity.x * Vector2.right * totalSpeedMultiplier * stoppingPower * Time.deltaTime);
                }
                if (entity.useGravity)
                {
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