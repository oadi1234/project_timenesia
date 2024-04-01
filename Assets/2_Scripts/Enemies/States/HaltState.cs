using _2_Scripts.Global.FSM;
using UnityEngine;

namespace _2_Scripts.Enemies.States
{
    public class HaltState : IState
    {
        private EnemyBase _enemy;

        public HaltState(EnemyBase enemy)
        {
            _enemy = enemy;
        }

        public void OnLogic()
        {
            Debug.Log("Halt");
        }

        public void OnEnter()
        {
            _enemy.SpriteRenderer.color = Color.white;
        }

        public void OnExit()
        {
        }
    }
}