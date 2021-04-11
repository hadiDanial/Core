using DG.Tweening;
using UnityEngine;
using Core.Entities;

namespace Core  
{
    [RequireComponent(typeof(Collider2D), typeof(AudioSource))]
    public class Health : MonoBehaviour
    {
        [SerializeField] protected int maxHealth;
        [SerializeField] internal int currentHealth;
        [SerializeField] internal float invulnerabilityTime = 0.15f;
        [SerializeField, ColorUsage(true, true)] public Color normalColor;
        [SerializeField, ColorUsage(true, true)] public Color damageColor;
        [SerializeField, ColorUsage(true, true)] public Color deadColor;

        Entity entity;
        SpriteRenderer spriteRenderer;
        private float timer = 0;
        internal bool canTakeDamage;
        private bool triggerAllKill = true;
        public delegate void Hit(bool heal = false);
        public event Hit OnHit;
        public delegate void Kill(bool deactivate = true);
        public event Kill OnKill;
        public static event Kill OnAllKill;

        public void Setup(Entity entity, bool triggerAll = true)
        {
            this.entity = entity;
            this.spriteRenderer = entity.spriteRenderer;
            triggerAllKill = triggerAll;
            Reset();
        }
        /// <summary>
        /// Damages the Entity and optionally sets it invulnerable for a time based on the hurt animation
        /// </summary>
        /// <param name="dmg"></param>
        /// <param name="setInvulnerable"></param>
        internal virtual void Damage(int dmg, bool setInvulnerable = true)
        {
            if (canTakeDamage)
            {
                currentHealth -= dmg;
                OnHit?.Invoke();
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    Death();
                }
                else
                {

                    //audioSource.PlayOneShot(hitSound);
                }
                if (setInvulnerable)
                {
                    SetInvulnerable();
                    Invoke("SetVulnerable", invulnerabilityTime);
                }
            }
        }
        /// <summary>
        /// Heals the Entity
        /// </summary>
        /// <param name="heal"></param>
        internal virtual void Heal(int heal)
        {
            currentHealth += heal;
            if (currentHealth >= maxHealth)
                currentHealth = maxHealth;
            else
            {
                OnHit?.Invoke(true);
            }
            // TODO - Sound effect here
        }

        internal virtual void Death()
        {
            spriteRenderer.DOColor(deadColor, invulnerabilityTime / 2f);
            //audioSource.PlayOneShot(deathSound);
            CancelInvoke();
            //entity.Kill();
            if (triggerAllKill)
            {
                OnKill?.Invoke();
                OnAllKill?.Invoke();
            }
            else
                OnKill?.Invoke();

        }


        internal virtual void SetInvulnerable()
        {
            canTakeDamage = false;
            spriteRenderer.DOColor(damageColor, invulnerabilityTime / 2f);
        }


        internal virtual void SetVulnerable()
        {
            canTakeDamage = true;
            spriteRenderer.DOColor(normalColor, invulnerabilityTime / 3f);
        }

        /// <summary>
        /// Resets the health and color to their default values.
        /// </summary>
        internal void Reset()
        {
            currentHealth = maxHealth;
            spriteRenderer.color = normalColor;
        }
    }

}