using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Entity
{
    public Checkpoint currentCheckpoint;


    Vector2 prevAim = Vector2.right;

    internal override void Awake()
    {
        base.Awake();
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
        SetMovementInput();
    }

    public void OnVertical(InputValue value)
    {
        if (!IsActive())
            return;
        input.y = value.Get<float>();
        SetMovementInput();
    }

    public void OnJump()
    {
        Jump();
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
