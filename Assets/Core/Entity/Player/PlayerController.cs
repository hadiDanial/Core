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

    #region Input
    public void OnMovement(InputValue value)
    {
        if (!IsActive())
            return;
        input = value.Get<Vector2>();
        SetMovementVector();
    }

    public void OnHorizontal(InputValue value)
    {
        if (!IsActive())
            return;
        input.x = value.Get<float>();
        SetMovementVector();
    }

    internal void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint != null)
            currentCheckpoint.isActive = false;
        currentCheckpoint = checkpoint;
    }

    public void OnVertical(InputValue value)
    {
        if (!IsActive())
            return;
        input.y = value.Get<float>();
        movementVector = useGravity ? new Vector2(input.x, 0).normalized : input;
    }

    public void OnJump()
    {
        if (!IsActive())
            return;
        Jump();
    }



    public void OnMouseAim(InputValue val)
    {
        Vector2 aim = Camera.main.ScreenToWorldPoint(val.Get<Vector2>()) - transform.position;
        SetAim(aim);
    }

    public void OnGamepadAim(InputValue val)
    {
        //cursor.SetGamepad(val.Get<Vector2>());
        Vector2 aim = val.Get<Vector2>();
        SetAim(aim);
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

    internal override void Kill()
    {
        // TODO - Kill the Entity
        currentEntityState = EntityState.Dead;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        spriteRenderer.color = deadColor;
        if (col != null) col.isTrigger = true;
        // TODO - Sound effect here
        audioSource.PlayOneShot(deathSound);
        CancelInvoke();
        OnReset();
    }
}
