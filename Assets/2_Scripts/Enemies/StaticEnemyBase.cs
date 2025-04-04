using System;
using _2_Scripts.Global;
using _2_Scripts.Model;
using UnityEngine;

namespace _2_Scripts.Enemies
{
    public abstract class StaticEnemyBase : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer { get; private set; }

        protected virtual void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
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
    }
}