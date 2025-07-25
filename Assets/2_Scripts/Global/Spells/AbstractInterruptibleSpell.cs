using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.Global.Spells
{
    public abstract class AbstractInterruptibleSpell : MonoBehaviour
    {
        [SerializeField] protected GameObject interruptSpellEffect;
        protected PlayerHealth PlayerHealth;
        
        private GameObject interruptInstance;

        protected virtual void Awake()
        {
            PlayerHealth = transform.GetComponentInParent<PlayerHealth>();
            RegisterEvents();
        }

        protected void InterruptAnimation()
        {
            DeregisterEvents();
            interruptSpellEffect = Instantiate(interruptSpellEffect, transform.position, transform.rotation);
            interruptSpellEffect.transform.localScale = transform.localScale;
            Destroy(gameObject);
        }

        protected virtual void DeregisterEvents()
        {
            if (PlayerHealth)
                PlayerHealth.Damaged -= InterruptAnimation;
        }

        protected virtual void RegisterEvents()
        {
            if (PlayerHealth)
                PlayerHealth.Damaged += InterruptAnimation;
        }
    }
}