using System;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_SecondApproach
{
    public class ChargeState : IState
    {
        // public IStateMachine StateMachine;
        public CleanEnemyStateMachine CleanEnemyStateMachine;
        
        private DynamicEnemyBase _dynamicEnemy;
        private float _xForce;
        
        private float _timeInterval = 2f;
        private float _currentTimeInterval;
        private readonly Color _redding = new (0, -0.01f, -0.01f, 0);

        public ChargeState(DynamicEnemyBase dynamicEnemy, float xForce, CleanEnemyStateMachine cleanEnemyStateMachine)
        {
            _dynamicEnemy = dynamicEnemy;
            _xForce = xForce;
            CleanEnemyStateMachine = cleanEnemyStateMachine;
        }

        public virtual void OnEnter()
        {
            _currentTimeInterval = 0;
        }

        public virtual void OnLogic()
        {
            if (_currentTimeInterval >= _timeInterval)
            {
                Charge();
            }
            else
            {
                LoadCharge();
            }
        }
        public virtual void OnExit()
        {
            _dynamicEnemy.SpriteRenderer.color = Color.white;
            _dynamicEnemy.SetSightEnabled(true);
        }
        
        private void LoadCharge()
        {
            _dynamicEnemy.SpriteRenderer.color += _redding;
            _currentTimeInterval += Time.deltaTime;
        }

        private void Charge()
        {
            _dynamicEnemy.RigidBody.AddForce(new Vector2(_xForce, 0), ForceMode2D.Impulse);
            _dynamicEnemy.SpriteRenderer.color = Color.black;
            CleanEnemyStateMachine.ChangeState(CleanEnemyStateMachine.HaltState);
        }
    }
}