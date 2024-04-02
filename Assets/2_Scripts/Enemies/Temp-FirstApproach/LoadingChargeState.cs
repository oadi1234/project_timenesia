using System;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_FirstApproach
{
    internal class LoadingChargeState : StateBase
    {
        private EnemyBase _enemy;
        private float _timeInterval = 2f;
        private float _currentTimeInterval = 0f;
        private float _xForce;
        private bool _chargeFinished;
        private readonly Color _redding = new (0, -0.01f, -0.01f, 0);
        public static event Action OnChargeFinished;

        public LoadingChargeState(EnemyBase enemy, float xForce)
        {
            _enemy = enemy;
            _xForce = xForce;
        }
        public override void OnEnter()
        {
            _currentTimeInterval = 0;
        }

        public override void OnExit()
        {
            _enemy.SpriteRenderer.color = Color.white;
        }

        public override void OnLogic()
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

        private void LoadCharge()
        {
            _enemy.SpriteRenderer.color += _redding;
            _currentTimeInterval += Time.deltaTime;
        }

        private void Charge()
        {
            _enemy.RigidBody.AddForce(new Vector2(_xForce, 0), ForceMode2D.Impulse);
            _enemy.SpriteRenderer.color = Color.black;
            
            OnChargeFinished?.Invoke();
        }
    }
}
