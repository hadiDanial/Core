using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public abstract class MovementAction : Action
    {
        [Header("Movement")]
        [SerializeField] protected float movementSpeed;
        [SerializeField, Range(0, 1f), Tooltip("How fast the Entity stops moving when there is no input")]
        protected float stoppingPower = 0.125f;
        [SerializeField, Range(0, 2)]
        protected float airControlPercent = 0.45f;
        [SerializeField] protected float normalGravity = 1, fallGravity = 2;

        protected float currentMovementMultiplier;
        protected float totalSpeedMultiplier => movementSpeed * currentMovementMultiplier * internalSpeedMultiplier;
        protected const int internalSpeedMultiplier = 1000;
        protected bool isGrounded;
        protected Vector2 movementVector;

        protected virtual void SetMovementVector(Vector2 movementInput)
        {
            movementVector = entity.useGravity ? new Vector2(movementInput.x, 0).normalized : movementInput;
            if (movementVector.x > 0) entity.sign = 1;
            else if (movementVector.x < 0) entity.sign = -1;
        }

        internal void SetInput(Vector2 movementVector)
        {
            this.movementVector = movementVector;
            SetMovementVector(movementVector);
        }
    }
}
