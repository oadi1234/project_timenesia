using _2_Scripts.Enemies.Temp_olds;
using UnityEngine;

namespace _2_Scripts.Global.Hazards
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
