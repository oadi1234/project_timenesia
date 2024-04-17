using System;
using _2_Scripts.Global;
using _2_Scripts.Model;
using UnityEngine;

namespace _2_Scripts.Enemies
{
    public abstract class StaticEnemyBase : MonoBehaviour
    {
        public int MaxHP;
        private int _currentHp;
        public SpriteRenderer SpriteRenderer { get; private set; }
        public event Action<int> OnEnemyKilled;

        protected virtual void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            _currentHp = MaxHP;
        }

        public virtual void OnSight(Collider2D other)
        {
            
        }

        public virtual void OnHearing(Collider2D other)
        {
            
        }

        // public virtual void OnLogic()
        // {
        //     
        // }

        public virtual void SetSightEnabled(bool sightEnabled)
        {
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == (int) Layers.Weapon)
            {
                _currentHp -= other.gameObject.GetComponent<WeaponBase>().Damage;
                if (_currentHp <= 0)
                    Destroy(gameObject);
            }
        }
        

        private void OnDestroy()
        {
            OnEnemyKilled?.Invoke(1); //TODO: attached killcounts etc. here
        }
    }
}