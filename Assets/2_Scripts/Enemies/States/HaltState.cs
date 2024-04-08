using _2_Scripts.Enemies.FSM;
using UnityEngine;

namespace _2_Scripts.Enemies.States
{
    public class HaltState : IState
    {
        private readonly StaticEnemyBase _dynamicEnemy;
        public StateMachine StateMachine;

        public HaltState(StateMachine cleanEnemyStateMachine, StaticEnemyBase dynamicEnemy)
        {
            StateMachine = cleanEnemyStateMachine;
            _dynamicEnemy = dynamicEnemy;
        }

        public void OnEnter()
        {
            _dynamicEnemy.SpriteRenderer.color = Color.cyan;
        }
        public void OnLogic()
        {
            // Debug.Log("Halt");
        }
        public void OnExit()
        {
        }
    }
}