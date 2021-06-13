using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Entities
{
    public class PlayerController : Entity
    {
        public Checkpoint currentCheckpoint;

        PlayerInput playerInput;
        Vector2 prevAim = Vector2.right;

        internal override void Awake()
        {
            base.Awake();
            playerInput = new PlayerInput();
        }
        private void OnEnable()
        {
            if (playerInput != null)
            {
                playerInput.Player.Enable();
                playerInput.Player.Jump.performed += ctx => OnJump();
                playerInput.Player.Jump.canceled += ctx => OnJumpCancel();
                playerInput.Player.Horizontal.performed += ctx => OnHorizontal(ctx.ReadValue<float>());
                playerInput.Player.Vertical.performed += ctx => OnVertical(ctx.ReadValue<float>());
            }
        }
        private void OnDisable()
        {
            if (playerInput != null)
            {
                playerInput.Player.Disable();
                playerInput.Player.Jump.performed -= ctx => OnJump();
                playerInput.Player.Jump.canceled -= ctx => OnJumpCancel();
                playerInput.Player.Horizontal.performed -= ctx => OnHorizontal(ctx.ReadValue<float>());
                playerInput.Player.Vertical.performed -= ctx => OnVertical(ctx.ReadValue<float>());
            }

        }
        internal void ResetPlayer(Vector3 spawnPosition)
        {
            ResetEntity();
            ResetVelocityAndInput();
            transform.position = spawnPosition;
        }

        internal void SetActiveCheckpoint(Checkpoint checkpoint)
        {
            if (currentCheckpoint != null)
                currentCheckpoint.isActive = false;
            currentCheckpoint = checkpoint;
        }

        public void OnReset()
        {
            if (currentCheckpoint != null)
                currentCheckpoint.ResetCheckpoint(this);
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
        internal override void Kill(bool deactivate = true)
        {
            base.Kill(false);
            OnReset();
        }

        #region Input

        public void OnHorizontal(InputValue value)
        {
            if (!IsActive())
                return;
            input.x = value.Get<float>();
        }
        public void OnHorizontal(float value)
        {
            if (!IsActive())
                return;
            input.x = value;
        }

        public void OnVertical(InputValue value)
        {
            if (!IsActive())
                return;
            input.y = value.Get<float>();
        }
        public void OnVertical(float value)
        {
            if (!IsActive())
                return;
            input.y = value;
        }

        public void OnJump()
        {
            Jump();
        }

        private void OnJumpCancel()
        {
            CancelJump();
        }

        public void OnMouseAim(InputValue val)
        {
            Vector2 aim = Camera.main.ScreenToWorldPoint(val.Get<Vector2>()) - transform.position;
            SetAim(aim);
        }

        public void OnGamepadAim(InputValue val)
        {
            Vector2 aim = val.Get<Vector2>();
            SetAim(aim);
        }


        private void SetAim(Vector2 aim)
        {
            if (aim == Vector2.zero)
            {
                aim = prevAim;
            }
            RotateGFX(aim);
            prevAim = aim;
        }
        #endregion

    }
}