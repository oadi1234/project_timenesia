using System;
using _2_Scripts.Player;
using UnityEngine;

namespace _2_Scripts.Global.Health.EnemyAttack
{
    public abstract class BaseAttack : MonoBehaviour
    {
        public static event Action<BaseAttack> PlayerHit;
        public string AttackName { get; protected set; }
        public DamageParameters Params { get; protected set; }
        
        protected virtual void OnPlayerHit(BaseAttack attack)
        {
            PlayerHit?.Invoke(attack);
        }
    }
}
