using _2_Scripts.Enemies.Attacks;
using UnityEngine;

namespace _2___Scripts.Enemies.Attacks
{
    public class FirePillar : AttackAoE
    {
        public override void OnTriggerEnter2D(Collider2D collision)
        {
            gameObject.GetComponent<Animator>().SetTrigger("Fire");
            base.OnTriggerEnter2D(collision);
        }
    }
}
