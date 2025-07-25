using System;
using System.Collections;
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
        private float iFrameTimer = 0f;
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
            if (iFrameTimer <=0f && other.gameObject.layer == (int) Layers.PlayerAttack)
            {
                // other.GetComponent<WeaponAttackHandler>();
                currentHealth -= PlayerDamageController.Instance.currentDamage;
                OnHit(other);
                if (currentHealth <= 0)
                {
                    Death();
                }
            }
        }

        private void OnHit(Collider2D other)
        {
            SpawnText();
            Knockback(other);
            SpawnParticles();
        }

        private void Knockback(Collider2D other)
        {
            Vector2 direction = (transform.position - other.transform.position).normalized;
            //TODO here now for testing, move to a specific inheritor of damageable health so not all damageable stuff gets knocked back.
            if (rb2d)
                rb2d.MovePosition(rb2d.position + direction * PlayerDamageController.Instance.currentKnockbackForce);
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