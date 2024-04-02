using System;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_CleanEnemy
{
    public class ChargeState : IState
    {
        // public IStateMachine StateMachine;
        public CleanEnemyStateMachine CleanEnemyStateMachine;
        
        private EnemyBase _enemy;
        private float _xForce;
        
        private float _timeInterval = 2f;
        private float _currentTimeInterval;
        private bool _chargeFinished;
        private readonly Color _redding = new (0, -0.01f, -0.01f, 0);
        public static event Action OnChargeFinished;

        public ChargeState(EnemyBase enemy, float xForce, CleanEnemyStateMachine cleanEnemyStateMachine)
        {
            _enemy = enemy;
            _xForce = xForce;
            CleanEnemyStateMachine = cleanEnemyStateMachine;
        }

        public virtual void OnEnter()
        {
            _chargeFinished = false;
            _currentTimeInterval = 0;
        }

        public virtual void OnUpdate()
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
            _enemy.SpriteRenderer.color = Color.white;
            OnChargeFinished?.Invoke();
        }
        
        private void LoadCharge()
        {
            _enemy.SpriteRenderer.color += _redding;
            _currentTimeInterval += Time.deltaTime;
        }

        private void Charge()
        {
            _enemy.RigidBody.AddForce(new Vector2(_xForce, 0), ForceMode2D.Impulse);
            _enemy.SpriteRenderer.color = Color.black;
            CleanEnemyStateMachine.ChangeState(CleanEnemyStateMachine.HaltState);
        }
    }
}