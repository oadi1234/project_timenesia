using System;
using System.Collections;
using _2_Scripts.Global.Spells;
using _2_Scripts.Player;
using _2_Scripts.Player.Controllers;
using _2_Scripts.Text;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace _2_Scripts.Global.Health
{
    // Health controller for general purpose. It should be able to handle bosses, normal enemies and destructible obstacles
    // Not designed for player. Player has int as its max and current health.
    public class DamageableHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 25f;
        [SerializeField] private int objectId; //TODO probably change this to an enum with type
        [SerializeField] private ParticleSystem damageParticles;
        [SerializeField] private TextController textController;
        private Rigidbody2D rb2d;

        private float currentHealth;
        private ParticleSystem damageParticlesInstance;
        private TextController textControllerInstance;
        
        public event Action<int> OnDestroyed;

        private void Awake()
        {
            currentHealth = maxHealth;
            rb2d = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == (int) Layers.PlayerAttack)
            {
                DamagingSpell spell = other.gameObject.GetComponent<DamagingSpell>();
                if (spell)
                {
                    if (spell.WasTagged(gameObject)) return;
                    if (!spell.IsMultihit)
                        spell.TagEnemy(gameObject);
                    currentHealth -= spell.spellDamage * PlayerDamageController.Instance.GetSpellMultiplier();
                }
                else
                {
                    currentHealth -= PlayerDamageController.Instance.GetDamage();
                }
                OnHit(other, spell);
                if (currentHealth <= 0)
                {
                    Death();
                }
            }
        }

        private void OnHit(Collider2D other, DamagingSpell spell)
        {
            SpawnText();
            if (spell)
                SpellKnockback(other, spell);
            else
                Knockback(other);
            SpawnParticles();
        }

        private void Knockback(Collider2D other)
        {
            //TODO here now for testing, move to a specific inheritor of damageable health so not all damageable stuff gets knocked back.
            if (rb2d)
            {
                Vector2 direction = (transform.position - other.transform.position).normalized;
                rb2d.MovePosition(rb2d.position + direction * PlayerDamageController.Instance.GetKnockbackForce());
            }
        }

        private void SpellKnockback(Collider2D other, DamagingSpell spell)
        {
            if (rb2d)
            {
                Vector2 direction = (transform.position - other.transform.position).normalized;
                rb2d.MovePosition(rb2d.position + direction * spell.spellKnockback);
            }
                
        }

        private void SpawnText()
        {
            //debug only for build&run, but quite fun.
            if (!textController) return;
            textControllerInstance = Instantiate(textController, transform.position + Vector3.up, Quaternion.identity);
            textControllerInstance.ShowText();
            StartCoroutine(DestroyText(textControllerInstance));
        }

        private IEnumerator DestroyText(TextController instance)
        {
            yield return new WaitForSeconds(1.5f);
            instance.HideText();
            yield return new WaitForSeconds(1.5f);
            Destroy(instance);
        }

        private void SpawnParticles()
        {
            if (damageParticles)
            {
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, transform.position - PlayerPosition.GetPlayerPosition());
                damageParticlesInstance = Instantiate(damageParticles, transform.position, spawnRotation);
            }
        }

        private void Death()
        {
            // default behaviour, should be overriden when necessary (spawn death clutter etc)
            Destroy(gameObject);
        }
        
        private void OnDestroy()
        {
            OnDestroyed?.Invoke(objectId); //TODO: attached killcounts etc. here
        }
    }
}