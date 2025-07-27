﻿using _2_Scripts.Global;
using _2_Scripts.Global.Health;
using _2_Scripts.Global.Health.EnemyAttack;
using _2_Scripts.Player;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_olds
{
    public class AttackAoE : BaseAttack
    {
        [SerializeField] private string attackName;
        public string AttackName => attackName;
        public DamageParameters Params => null;//gameObject.AddComponent<Hurt>(); /*TODO*/

        public virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == (int) Layers.Player)
            {
                OnPlayerHit(this);
            }
        }
    }
}
