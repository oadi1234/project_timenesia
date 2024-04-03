using _2___Scripts.Enemies.Attacks;
using _2___Scripts.Global;
using _2___Scripts.UI;
using Assets.Scripts.Enemies.Attacks;
using System.Collections;
using UnityEngine;

namespace _2___Scripts.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        private int maxHealth;
        private int currentHealth;

        private float iFrame = 0f;

        public HealthBar healthBar;

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
            Attack_AoE.OnAttack += Attack_AoE_OnAttackHit;
        }

        // Update is called once per frame
        void Update()
        {
            //if(Input.GetKeyDown(KeyCode.U))
            //{
            //    TakeDamage(1);
            //}
            //if(Input.GetKeyDown(KeyCode.I))
            //{
            //    IncreaseHealth(1);
            //}
            //if(Input.GetKeyDown(KeyCode.K))
            //{
            //    IncreaseHealth(-1);
            //}
        }

        private void Attack_AoE_OnAttackHit(IBaseAttack obj)
        {
            TakeDamage(obj.Params);
        }

        private void Initialize()
        {
            maxHealth = GameDataManager.Instance.stats.MaxHealth;
            currentHealth = maxHealth;
            healthBar.Initialize();
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);
        }
        public void Restart()
        {
            Initialize();
        }

        public void TakeDamage(int damage)
        {
            TakeDamage(damage, 1f);
        }        
        
        private void TakeDamage(Hurt hurt)
        {
            TakeDamage(hurt.damageDealt, hurt.iFramesGiven);
        }

        public void TakeDamage(int damage, float iFrame)
        {
            if (this.iFrame <= 0f)
            {
                StartCoroutine(ApplyIFrames());
                currentHealth -= damage;
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    //Destroy(gameObject);
                }
                healthBar.SetCurrentHealth(currentHealth);
                this.iFrame = iFrame;
            }
        }

        public void IncreaseHealth(int amount)
        {
            maxHealth += amount;
            currentHealth = maxHealth;
            if (maxHealth < 1) 
                maxHealth = 1;
            healthBar.SetMaxHealth(maxHealth);
        }

        private IEnumerator ApplyIFrames()
        {
            gameObject.layer = (int)LayerNames.PlayerIFrame;
            yield return new WaitForSeconds(iFrame);
            gameObject.layer = (int)LayerNames.Player;
            iFrame = 0f;
        }
    }
}
