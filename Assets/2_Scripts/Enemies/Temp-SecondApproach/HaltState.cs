using UnityEngine;

namespace _2_Scripts.Enemies.Temp_SecondApproach
{
    public class HaltState : IState
    {
        private readonly EnemyBase _enemy;
        public CleanEnemyStateMachine CleanEnemyStateMachine;

        public HaltState(CleanEnemyStateMachine cleanEnemyStateMachine, EnemyBase enemy)
        {
            CleanEnemyStateMachine = cleanEnemyStateMachine;
            _enemy = enemy;
        }

        public void OnEnter()
        {
            _enemy.SpriteRenderer.color = Color.cyan;
        }
        public void OnUpdate()
        {
            // Debug.Log("Halt");
        }
        public void OnExit()
        {
        }
    }
}