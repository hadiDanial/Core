using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class Enemy : Entity
{
    [Header("Enemy Layers")]
    [SerializeField] internal int enemyLayer = 9;
    [SerializeField] internal int heldEnemyLayer = 10;
    [SerializeField] private Transform originalParent = null;
    [SerializeField] bool isHeld = false;
    [SerializeField] bool rotateToDirection = false;

    private EnemyAI enemyAI;
    internal float mass, drag;
    Vector2 currentPos, prevPos, dir1, dir2;
    internal override void Awake()
    {
        base.Awake();
        enemyAI = GetComponent<EnemyAI>();
        mass = rb.mass;
        drag = rb.drag;
        originalParent = transform.parent;
        prevPos = transform.position;
    }

    internal void Update()
    {
        currentPos = transform.position;
        dir1 = currentPos - prevPos;
        dir1 *= 10;
        prevPos = currentPos;
        if (rotateToDirection)
        {
            if (enemyAI.rotateDuringAttack && !enemyAI.doneAttacking)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, MathP.RotationFromDirection(-dir1), Time.deltaTime * 5f);
            }
            else
                transform.right = rb.velocity;
        }
        else
        {
            transform.rotation = Quaternion.identity;
            RotateGFX(input);
        }
    }


    internal void ResetEnemy()
    {
        enemyAI.ResetAI();
        
    }

    #region HOOK_BEHAVIOR
    public Vector2 GetPosition()
    {
        return transform.position;
    }
    public void Hook(Transform parent)
    {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.mass = 1;
        rb.drag = 1;
        transform.SetParent(parent);
        gameObject.layer = heldEnemyLayer;
        transform.localPosition = Vector2.zero;
        spriteRenderer.DOColor(heldColor, invulnerabilityTime);
        currentEntityState = EntityState.Inactive;
        enemyAI.SetHeldSprite();
        enemyAI.PauseAI();
        CancelInvoke();
    }
    public void Hold()
    {
        isHeld = true;
    }
    public void SetPosition(Vector2 position)
    {
        transform.position += (Vector3) position;
    }
    public void Throw(Vector2 direction)
    {
        
        //transform.position = transform.localToWorldMatrix * transform.localPosition;
        transform.SetParent(originalParent);
        rb.isKinematic = false;
        rb.velocity = direction;
        Invoke("ResetLayer", 1f);
    }

    public Rigidbody2D GetRB()
    {
        return rb;
    }
    private void ResetLayer()
    {
        gameObject.layer = enemyLayer;
        isHeld = false;
        spriteRenderer.DOColor(normalColor, invulnerabilityTime);
        currentEntityState = EntityState.Active;
        rb.mass = mass;
        rb.drag = drag;

    }
    #endregion

    internal override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        GameObject other = collision.gameObject;
        Entity entity = other.GetComponent<Entity>();
        if (!isHeld) return;
        if ((enemyMask & 1 << other.layer) != 0)
        {
            if (entity != null)
            {
                entity.Damage(1);
            }
            Damage(1);
        }
    }

    internal override void Kill()
    {
        enemyAI.SetDeadSprite();
        ResetLayer();
        base.Kill();
    }

    private void OnDestroy()
    {
        //if (isHeld)
        //    FindObjectOfType<HookController>().Clear();
        DOTween.Kill(rb);
        DOTween.Kill(spriteRenderer);
    }

}
