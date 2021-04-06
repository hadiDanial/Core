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
    [SerializeField] bool rotateToDirection = false;

    private EnemyAI enemyAI;
    internal float mass, drag;
    Vector2 currentPos, prevPos, dir;
    internal override void Awake()
    {
        base.Awake();
        enemyAI = GetComponent<EnemyAI>();
        mass = rb.mass;
        drag = rb.drag;
        originalParent = transform.parent;
        prevPos = transform.position;
    }

    internal override void Update()
    {
        base.Update();
        currentPos = transform.position;
        dir = currentPos - prevPos;
        dir *= 10;
        prevPos = currentPos;
        if (rotateToDirection)
        {
            if (enemyAI.rotateDuringAttack && !enemyAI.doneAttacking)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, MathP.RotationFromDirection(-dir), Time.deltaTime * 5f);
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


    internal override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        GameObject other = collision.gameObject;
        Entity entity = other.GetComponent<Entity>();
        if ((enemyMask & 1 << other.layer) != 0)
        {
            if (entity != null)
            {
                health.Damage(1);
            }
            health.Damage(1);
        }
    }

    internal override void Kill(bool deactivate = true)
    {
        enemyAI.SetDeadSprite();
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
