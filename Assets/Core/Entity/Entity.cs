using System;
using UnityEngine;


namespace Core.Entities
{
    [RequireComponent(typeof(AudioSource))]
    public class Entity : MonoBehaviour
    {
        [SerializeField] internal EntityState currentEntityState = EntityState.Active;
        [Header("Movement")]
        [SerializeField] protected float movementSpeed;
        [SerializeField, Range(0, 1f), Tooltip("How fast the Entity stops moving when there is no input")]
        protected float stoppingPower = 0.125f;
        [Header("Jump")]
        [SerializeField] protected float jumpForce;
        [SerializeField, Range(0, 0.4f)]
        protected float jumpGracePeriod = 0.1f;
        [SerializeField] protected bool jumpAllowed = true, canDoubleJump = false;
        [SerializeField, Range(0, 2)]
        protected float airControlPercent = 0.45f;
        [SerializeField] protected bool useGravity;
        [SerializeField, Tooltip("This is only to show whether the entity is grounded. Can not modify.")]
        internal bool isGrounded;
        [SerializeField, Tooltip("This is only to show whether the entity is hitting a wall on the sides. Can not modify.")]
        internal bool isHittingSide;
        [SerializeField] protected float normalGravity = 1, fallGravity = 2;

        [Header("Sounds")]
        [SerializeField] internal AudioClip hitSound = null, deathSound = null;


        [Header("Ground Check")]
        [SerializeField] protected GameObject groundCheckLeft;
        [SerializeField] protected GameObject groundCheckRight;
        [SerializeField] protected GameObject groundCheckMiddle;
        [SerializeField] protected LayerMask groundMask;
        internal float groundCheckDistance = 0.1f;
        [Header("Side Check")]
        [SerializeField] protected GameObject sideCheckLeft;
        [SerializeField] protected GameObject sideCheckRight;
        [SerializeField] protected LayerMask wallMask;
        internal float sideCheckDistance = 0.1f;

        const int internalSpeedMultiplier = 1000;

        [Header("GFX")]
        [SerializeField] protected GameObject GFX;
        [SerializeField] internal SpriteRenderer spriteRenderer;

        [Header("Other")]
        [SerializeField] protected float deactivationTime;
        [SerializeField] protected LayerMask enemyMask;


        internal Health health = null;
        internal AudioSource audioSource;
        internal Rigidbody2D rb;
        internal Collider2D col;
        internal Animator anim = null;
        internal Transform initialTransform;
        internal EntityState initialState;
        internal Vector2 input, aim = Vector2.zero, movementVector;
        internal bool canMove; // Might remove later, useless now

        internal float currentMovementMultiplier;
        internal float jumpTimeElapsed;
        internal bool _isGrounded => CheckIsGrounded();
        internal bool _isHittingSide => CheckIsHittingSide();
        internal bool canJump => _isGrounded || jumpTimeElapsed >= 0;
        internal bool isCollided, hasJumped, hasDoubleJumped;
        internal float totalSpeedMultiplier => movementSpeed * currentMovementMultiplier * internalSpeedMultiplier;
        internal int sign = 1;

        Vector3 initialPosition;

        internal virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            SetupEntity();
        }

        internal virtual void Update()
        {
            SetAnimation();
        }

        internal virtual void SetAnimation()
        {

        }

        internal virtual void FixedUpdate()
        {
            Move();
        }

        /// <summary>
        /// Sets the movement vector based on the movementInput and gravity options.
        /// </summary>
        internal void SetMovementInput(Vector2 movementInput)
        {
            input = movementInput;
            SetMovementVector(movementInput);
        }

        /// <summary>
        /// Sets the movement vector based on the input and gravity options.
        /// </summary>
        internal void SetMovementInput()
        {
            SetMovementVector(input);
        }

        private void SetMovementVector(Vector2 movementInput)
        {
            movementVector = useGravity ? new Vector2(movementInput.x, 0).normalized : movementInput;
            if (movementVector.x > 0) sign = 1;
            else if (movementVector.x < 0) sign = -1;
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
        /// Movement logic.
        /// </summary>
        internal virtual void Move()
        {
            isGrounded = _isGrounded;
            isHittingSide = _isHittingSide;
            if (IsActive())
            {
                if (isHittingSide)
                {
                    movementVector.x = 0;
                }
                jumpTimeElapsed = isGrounded ? jumpGracePeriod : jumpTimeElapsed - Time.deltaTime;
                if (input != Vector2.zero)
                {
                    currentMovementMultiplier = isGrounded ? 1 : airControlPercent;
                    rb.AddForce(movementVector * totalSpeedMultiplier * Time.deltaTime, ForceMode2D.Force);
                }
                else
                {
                    rb.AddForce(-rb.velocity.x * Vector2.right * totalSpeedMultiplier * stoppingPower * Time.deltaTime);
                }
                if (useGravity)
                {
                    rb.gravityScale = (rb.velocity.y < 0f) ? fallGravity : normalGravity;
                }
            }
        }

        /// <summary>
        /// Jump
        /// </summary>
        internal virtual void Jump()
        {
            Vector2 doubleJumpBoost = Vector2.up;
            if (!jumpAllowed || !IsActive())
                return;
            // Jump
            if (canJump && !hasJumped)
            {
                hasJumped = true;
                rb.AddForce(Vector2.up * jumpForce * 10, ForceMode2D.Impulse);
            }
            // Double Jump
            else if (canDoubleJump && hasJumped && !hasDoubleJumped)
            {
                // Jump boost for if the player is falling
                if (rb.velocity.y <= 0.1f) doubleJumpBoost = doubleJumpBoost + Vector2.up * Mathf.Clamp01(Mathf.Abs(rb.velocity.y));
                hasDoubleJumped = true;
                rb.AddForce(jumpForce * doubleJumpBoost * 8, ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// Rotates the graphics object to face the direction on the y-axis.
        /// </summary>
        /// <param name="dir">The direction to face.</param>
        internal void RotateGFX(Vector2 dir)
        {
            if (GFX == null) return;
            if (dir.x < 0)
            {
                GFX.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (dir.x > 0)
            {
                GFX.transform.rotation = Quaternion.Euler(0, 0, 0);
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
                        canMove = true;
                        health.SetVulnerable();
                    }
                    break;
                case EntityState.Inactive:
                    {
                        canMove = false;
                        health.SetInvulnerable();
                    }
                    break;
                case EntityState.Dead:
                    {
                        canMove = false;
                        health.SetInvulnerable();
                    }
                    break;
                default:
                    {
                        canMove = true;
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


        /// <summary>
        /// Returns true if the Entity is grounded. 
        /// The entity is grounded if at least one of its ground checks returns true.
        /// </summary>
        private bool CheckIsGrounded()
        {
            if (groundCheckLeft == null || groundCheckRight == null || groundCheckMiddle == null)
            {
                Debug.LogError(transform.name + ": No Ground Check Object!");
                return false;
            }
            RaycastHit2D hitL = Physics2D.Raycast(groundCheckLeft.transform.position, Vector2.down, groundCheckDistance, groundMask);
            RaycastHit2D hitR = Physics2D.Raycast(groundCheckRight.transform.position, Vector2.down, groundCheckDistance, groundMask);
            RaycastHit2D hitM = Physics2D.Raycast(groundCheckMiddle.transform.position, Vector2.down, groundCheckDistance, groundMask);
            if (hitR || hitL || hitM)
                return true;
            return false;
        }

        private bool CheckIsHittingSide()
        {
            if (sideCheckLeft == null || sideCheckRight == null)
            {
                Debug.LogError(transform.name + ": No Side Check Object!");
                return false;
            }
            GameObject check;
            switch (sign)
            {
                case 1:
                    check = sideCheckRight;
                    break;
                case 2:
                    check = sideCheckLeft;
                    break;
                default:
                    check = sideCheckRight;
                    break;
            }
            RaycastHit2D hit = Physics2D.Raycast(check.transform.position, Vector2.right * sign, sideCheckDistance, wallMask);
            // Add walljump check
            if (hit) return true;
            return false;

        }




        internal virtual void OnCollisionEnter2D(Collision2D collision)
        {
            isCollided = true;
            if (_isGrounded)
            {
                hasJumped = false;
                hasDoubleJumped = false;
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

        private void OnDrawGizmos()
        {
            if (groundCheckLeft == null || groundCheckRight == null || groundCheckMiddle == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheckRight.transform.position, groundCheckRight.transform.position + Vector3.down * groundCheckDistance);
            Gizmos.DrawLine(groundCheckLeft.transform.position, groundCheckLeft.transform.position + Vector3.down * groundCheckDistance);
        }
    }

    public enum EntityState
    {
        Active, Inactive, Dead
    }
}