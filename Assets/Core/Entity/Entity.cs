using System;
using UnityEngine;


namespace Core.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CollisionChecker), typeof(Health))]
    [RequireComponent(typeof(AudioSource))]
    public class Entity : MonoBehaviour
    {
        [SerializeField] internal EntityState currentEntityState = EntityState.Active;
        [SerializeField, Tooltip("This is only to show whether the entity is grounded. Can not modify.")]
        internal bool isGrounded;
        [SerializeField, Tooltip("This is only to show whether the entity is hitting a wall on the sides. Can not modify.")]
        internal bool isHittingSide;
        [SerializeField, Tooltip("This is only to show whether the entity is hitting something above them. Can not modify.")]
        internal bool isHittingHead;
        [SerializeField] protected float normalGravity = 1, fallGravity = 2;

        [Header("Sounds")]
        [SerializeField] internal AudioClip hitSound = null;
        [SerializeField] internal AudioClip deathSound = null;

        [Header("GFX")]
        [SerializeField] protected GameObject gfxObject;
        [SerializeField] internal SpriteRenderer spriteRenderer;

        [Header("Other")]
        [SerializeField] protected float deactivationTime;
        [SerializeField] protected LayerMask enemyMask;


        internal EntityState initialState;
        internal Health health = null;
        internal Animator anim = null;
        internal AudioSource audioSource;
        internal Rigidbody2D rb;
        internal CollisionChecker collisionChecker;
        internal Collider2D col;
        internal Transform initialTransform;
        internal Vector2 input, aim = Vector2.zero, movementVector;

        internal float currentMovementMultiplier;
        private bool _isGrounded => collisionChecker.CheckIsGrounded();
        private bool _isHittingSide => collisionChecker.CheckIsHittingSide(sign);
        private bool _isHittingHead => collisionChecker.CheckIsHittingHead();
        internal bool isCollided;
        internal int sign = 1;

        Vector3 initialPosition;
        public MovementAction movementAction;
        public JumpAction jumpAction;
        private Action[] actions;

        internal virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            collisionChecker = GetComponent<CollisionChecker>();
            SetupEntity();
            SetupActions();
        }

        internal virtual void SetupActions()
        {
            actions = GetComponents<Action>();
            foreach (Action action in actions)
            {
                action?.Initialize(this, null);
            }
            movementAction = GetComponent<MovementAction>();
            movementAction?.StartAction();
            jumpAction = GetComponent<JumpAction>();
        }

        internal virtual void Update()
        {
            if (currentEntityState != EntityState.Active)
                return;
            isGrounded = _isGrounded;
            isHittingSide = _isHittingSide;
            isHittingHead = _isHittingHead;
            SetAnimation();
            movementAction.SetInput(input);
        }

        internal virtual void SetAnimation()
        {

        }

        /// <summary>
        /// Reset velocity and input to zero.
        /// </summary>
        internal void ResetVelocityAndInput()
        {
            input = Vector2.zero;
            rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// Jump
        /// </summary>
        internal virtual void Jump()
        {
            jumpAction?.StartAction();
        }

        internal virtual void CancelJump()
        {
            jumpAction?.CancelJump();
        }

        /// <summary>
        /// Rotates the graphics object to face the direction on the y-axis.
        /// </summary>
        /// <param name="dir">The direction to face.</param>
        internal void RotateGFX(Vector2 dir)
        {
            if (gfxObject == null) return;
            if (dir.x < 0)
            {
                gfxObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (dir.x > 0)
            {
                gfxObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        /// <summary>
        /// The initial setup of the Entity
        /// </summary>
        internal virtual void SetupEntity()
        {
            health.Setup(this);
            health.OnKill += Kill;
            initialTransform = transform;
            initialPosition = transform.position;
            initialState = currentEntityState;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            col = GetComponent<Collider2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (col != null) col.isTrigger = false;
            audioSource = GetComponent<AudioSource>();
            EnforceEntityState();
        }

        /// <summary>
        /// Resets the entity to its initial state: health, position, and EntityState
        /// </summary>
        internal virtual void ResetEntity()
        {
            health.Reset();
            transform.position = initialPosition;
            transform.rotation = initialTransform.rotation;
            if (col != null) col.isTrigger = false;
            SetEntityState(initialState);
        }


        /// <summary>
        /// Kills the Entity
        /// </summary>
        internal virtual void Kill(bool deactivate = true)
        {
            // TODO - Kill the Entity
            currentEntityState = EntityState.Dead;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            if (col != null) col.isTrigger = true;
            // TODO - Animation?
            audioSource.PlayOneShot(deathSound);
            foreach (Action action in actions)
            {
                action?.StopAction();
                action?.SetActive(false);
            }
            if (deactivate)
            {
                CancelInvoke();
                Invoke("Deactivate", deactivationTime);
            }
        }


        /// <summary>
        /// Deactivates the game object.
        /// </summary>
        internal virtual void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            health.OnKill -= Kill;
        }


        /// <summary>
        /// Sets a new EntityState.
        /// </summary>
        /// <param name="newState"></param>
        internal virtual void SetEntityState(EntityState newState)
        {
            currentEntityState = newState;
            EnforceEntityState();
        }

        /// <summary>
        /// Sets whether the entity can move and if it's invulnerable based on the current state.
        /// </summary>
        private void EnforceEntityState()
        {
            switch (currentEntityState)
            {
                case EntityState.Active:
                    {
                        movementAction?.SetCanMove(true);
                        health.SetVulnerable();
                    }
                    break;
                case EntityState.Inactive:
                    {
                        movementAction?.SetCanMove(false);
                        health.SetInvulnerable();
                    }
                    break;
                case EntityState.Dead:
                    {
                        movementAction?.SetCanMove(false);
                        health.SetInvulnerable();
                    }
                    break;
                default:
                    {
                        movementAction?.SetCanMove(true);
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns true if the currentEntityState is Active
        /// </summary>
        public bool IsActive()
        {
            return currentEntityState == EntityState.Active;
        }

        internal virtual void OnCollisionEnter2D(Collision2D collision)
        {
            isCollided = true;
            if (_isGrounded)
            {
                jumpAction?.StopAction();
            }
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            isCollided = true;
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            isCollided = false;
        }
    }

    public enum EntityState
    {
        Active, Inactive, Dead
    }
}