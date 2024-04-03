using System;
using _2___Scripts.Global;
using _2_Scripts.Player;
using UnityEngine;

namespace _2_Scripts.Enemies.Attacks
{
    public class BodyDamageOnHit : BaseAttack
    {
        [SerializeField]
        private string attackName = "basic";
        [SerializeField]
        private bool persistent;

        private void Start()
        {
            Params = new Hurt {DamageDealt = 1, IFramesGiven = 0};
            AttackName = attackName;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == (int) LayerNames.Player)
            {
                OnAttack(this);
                if (persistent) return;
                
                gameObject.SetActive(false);
                Destroy(this); //todo: is this 100% safe?
            }
        }
    }
}