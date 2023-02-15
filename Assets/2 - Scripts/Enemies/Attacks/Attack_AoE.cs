using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Enemies.Attacks
{
    public class Attack_AoE : MonoBehaviour, IBaseAttack
    {
        public static event Action<IBaseAttack> OnAttack;

        [SerializeField] private string attackName;
        public string AttackName => attackName;

        public virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (OnAttack != null && collision.CompareTag("Player"))
                OnAttack(this);
        }
    }
}
