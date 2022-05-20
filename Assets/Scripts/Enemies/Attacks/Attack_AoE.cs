using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemies.Attacks
{
    internal class Attack_AoE : MonoBehaviour, IBaseAttack
    {
        public static event Action<IBaseAttack> OnAttack;

        [SerializeField] private string _attackName;
        public string AttackName => _attackName;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (OnAttack != null && collision.tag == "Player")
                OnAttack(this);
        }
    }
}
