using _2___Scripts.Enemies.Attacks;
using _2___Scripts.Global;
using _2___Scripts.UI;
using Assets.Scripts.Enemies.Attacks;
using UnityEngine;

namespace _2___Scripts.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public int maxHealth = 3;
        public int currentHealth = 3;

        public float iFrame = 0f;

        public HealthBar healthBar;

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
            Attack_AoE.OnAttack += Attack_AoE_OnAttackHit;
        }

        private void Attack_AoE_OnAttackHit(IBaseAttack obj)
        {
            TakeDamage(obj.Params);
        }

        private void Initialize()
        {
            // var s = SaveManager.Instance.Load();
            currentHealth = maxHealth;
            healthBar.Initialize();
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);
        }
        public void Restart()
        {
            Initialize();
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

        private void FixedUpdate()
        {
            if (iFrame > 0)
            {
                gameObject.layer = (int) LayerNames.PlayerIFrame;
                iFrame -= Time.fixedDeltaTime;
            }
            else gameObject.layer = (int) LayerNames.Player;
        }

        public void TakeDamage(int damage)
        {
            TakeDamage(damage, 2f);
        }        
        
        private void TakeDamage(Hurt hurt)
        {
            TakeDamage(hurt.damageDealt, hurt.iFramesGiven);
        }

        public void TakeDamage(int damage, float iFrame)
        {
            if (this.iFrame <= 0)
            {
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
    }
}
