using System;
using System.Collections;
using _2_Scripts.Global;
using _2_Scripts.Global.Health.EnemyAttack;
using _2_Scripts.Player.Controllers;
using _2_Scripts.UI.Elements.HUD;
using UnityEngine;

namespace _2_Scripts.Player.Statistics
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private ParticleSystem damageParticles;
        private int maxHealth;
        private int currentHealth;
        private PlayerMovementController playerMovementController;
        private float timescaleTimer = 0f;
        private bool hasIFrames = false;

        private ParticleSystem damageParticlesInstance;

        public event Action Damaged;

        [SerializeField] private HealthBar healthBar;

        void Start()
        {
            Initialize();
            BaseAttack.PlayerHit += OnBasicPlayerHitHit;
            playerMovementController = GetComponent<PlayerMovementController>();
        }

        private void OnBasicPlayerHitHit(BaseAttack obj)
        {
            TakeDamage(obj.Params);
        }

        private void Initialize()
        {
            maxHealth = GameDataManager.Instance.currentGameData.MaxHealth;
            currentHealth = maxHealth;
            healthBar.Initialize();
            healthBar.SetMax(maxHealth);
            healthBar.SetCurrent(currentHealth);
        }
        public void Restart()
        {
            Initialize();
        }

        private void TakeDamage(DamageParameters damageParameters)
        {
            Damaged?.Invoke();
            playerMovementController.HurtKnockback(damageParameters.KnockbackStrength, damageParameters.DamageSourcePosition);
            damageParticlesInstance = Instantiate(damageParticles, transform.position, Quaternion.identity);
            SlowDownTime();
            if (damageParameters.IFramesGiven >= 0f)
            {
                StartCoroutine(ApplyIFrames(damageParameters.IFramesGiven));
            }
            currentHealth -= damageParameters.DamageDealt;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                // TODO play death animation here. Then load game.
            }
            healthBar.SetCurrent(currentHealth);
        }

        private void SlowDownTime()
        {
            Time.timeScale = 0;
            StartCoroutine(RestoreTimeFlowOverTime());
        }

        private IEnumerator RestoreTimeFlowOverTime()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            timescaleTimer = 2f;
            while (Time.timeScale < 1f)
            {
                timescaleTimer += Time.fixedDeltaTime;
                Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1, timescaleTimer * 0.01f);
                
                yield return null;
            }
        }

        public void IncreaseHealth(int amount)
        {
            maxHealth += amount;
            currentHealth = maxHealth;
            if (maxHealth < 1) // wtf is this
                maxHealth = 1;
            healthBar.SetMax(maxHealth);
            //TODO play increase max health animation here. Both on health bar and do an overlay of sorts maybe?
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            healthBar.SetCurrent(currentHealth);
            //TODO play heal animation on the health bar here.
        }

        public bool HasIFrames()
        {
            return hasIFrames;
        }

        private IEnumerator ApplyIFrames(float iFrame)
        {
            gameObject.layer = (int)Layers.PlayerIFrame;
            hasIFrames = true;
            yield return new WaitForSeconds(iFrame);
            gameObject.layer = (int)Layers.Player;
            hasIFrames = false;
        }
    }
}
