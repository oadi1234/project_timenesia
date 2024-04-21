using System.Collections;
using _2___Scripts.UI;
using _2_Scripts.Enemies.Attacks;
using _2_Scripts.Global;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Statistics
{
    public class PlayerHealth : MonoBehaviour
    {
        private Animator animator;
        private int maxHealth;
        private int currentHealth;
        private PlayerMovementController playerMovementController;

        public HealthBar healthBar;

        void Start()
        {
            Initialize();
            BaseAttack.Attack += OnBasicAttackHit;
            animator = GetComponentInChildren<Animator>();
            playerMovementController = GetComponent<PlayerMovementController>();
        }

        private void OnBasicAttackHit(BaseAttack obj)
        {
            TakeDamage(obj.Params);
        }

        private void Initialize()
        {
            maxHealth = GameDataManager.Instance.Stats.MaxHealth;
            currentHealth = maxHealth;
            healthBar.Initialize();
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);
        }
        public void Restart()
        {
            Initialize();
        }

        private void TakeDamage(Hurt hurt)
        {

            playerMovementController.Knockback(hurt.KnockbackStrength);
            if (hurt.IFramesGiven >= 0f)
            {
                StartCoroutine(ApplyIFrames(hurt.IFramesGiven));
            }
            currentHealth -= hurt.DamageDealt;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                // TODO play death animation here. Then load game.
            }
            healthBar.SetCurrentHealth(currentHealth);
        }

        public void IncreaseHealth(int amount)
        {
            maxHealth += amount;
            currentHealth = maxHealth;
            if (maxHealth < 1) // wtf is this
                maxHealth = 1;
            healthBar.SetMaxHealth(maxHealth);
            //TODO play increase max health animation here. Both on health bar and do an overlay of sorts maybe?
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            healthBar.SetCurrentHealth(currentHealth);
            //TODO play heal animation on the health bar here.
        }

        private IEnumerator ApplyIFrames(float iFrame)
        {
            gameObject.layer = (int)Layers.PlayerIFrame;
            animator.SetBool("iFrame", true);
            yield return new WaitForSeconds(iFrame);
            gameObject.layer = (int)Layers.Player;
            animator.SetBool("iFrame", false);
        }
    }
}
