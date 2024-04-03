using System;
using _2_Scripts.Player;
using UnityEngine;

namespace _2_Scripts.Enemies.Attacks
{
    public abstract class BaseAttack : MonoBehaviour
    {
        public static event Action<BaseAttack> Attack;
        public string AttackName { get; protected set; }
        public Hurt Params { get; protected set; }
        
        protected virtual void OnAttack(BaseAttack attack)
        {
            Attack?.Invoke(attack);
        }
    }
}
