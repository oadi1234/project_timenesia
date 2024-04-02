using _2_Scripts.Global.FSM;
using UnityEngine;

namespace _2_Scripts.Enemies.States
{
    public class HaltState : StateBase
    {
        private EnemyBase _enemy;

        public HaltState(EnemyBase enemy)
        {
            _enemy = enemy;
        }

        public override void OnEnter()
        {
            _enemy.SpriteRenderer.color = Color.white;
        }
    }
}