using System;
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
        [SerializeField] internal bool useGravity = true;
        [SerializeField] protected float normalGravity = 1, fallGravity = 2;
        [SerializeField] protected bool canMove = true;

        protected float currentMovementMultiplier;
        protected float totalSpeedMultiplier => movementSpeed * currentMovementMultiplier * internalSpeedMultiplier;
        protected const int internalSpeedMultiplier = 1000;
        protected bool isGrounded, isHittingHead, isHittingSide;
        protected Vector2 movementVector, movementInput;
        public override void UpdateAction()
        {
            if (!canMove || !CanPerformAction())
                return;
            isGrounded = entity.isGrounded;
            isHittingHead = entity.isHittingHead;
            isHittingSide = entity.isHittingSide;
        }
        protected virtual void SetMovementVector()
        {
            movementVector = useGravity ? new Vector2(movementInput.x, 0).normalized : movementInput;
            if (movementVector.x > 0) entity.sign = 1;
            else if (movementVector.x < 0) entity.sign = -1;
        }

        internal void SetInput(Vector2 movementInput)
        {
            this.movementInput = movementInput;
        }

        internal void SetCanMove(bool v)
        {
            canMove = v;
        }

        public override void Initialize(Entity entity, AIDestinationSetter aiDestinationSetter)
        {
            base.Initialize(entity, aiDestinationSetter);
            resetTime = 0;
        }
    }
}
