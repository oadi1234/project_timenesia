using UnityEngine;

namespace _2_Scripts.Global.Health
{
    // Health controller for general purpose. It should be able to handle bosses, normal enemies and destructible obstacles
    // Not designed for player. Player has int as its max and current health.
    public class HealthController : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 5f;

        private float currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }
        
        public void OnHit(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                OnDeath();
            }
        }

        public void OnDeath()
        {
            //TODO when game data persistence is correctly implemented this will probably need to be updated
            // also might be better to give it a bit of a grace period, death animation first, or to not even
            // delete the body, just leave it there. Destroy is for development purposes only.
            Destroy(gameObject);
        }
    }
}