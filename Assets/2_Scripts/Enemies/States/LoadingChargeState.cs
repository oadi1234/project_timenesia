using System;
using _2_Scripts.Global.FSM;
using Assets.Scripts.Enemies;
using UnityEngine;

namespace _2_Scripts.Enemies.States
{
    internal class LoadingChargeState : IState
    {
        private EnemyBase _enemy;
        private float _timeInterval = 2f;
        private float _currentTimeInterval = 0f;
        private float _xForce;
        public bool ChargeFinished;
        private Color redding = new Color(0, -0.01f, -0.01f, 0);
        public static event Action<bool> chargeFinished;

        public Func<bool> IsChargeFinished()
        {
            return () => ChargeFinished;
        }

        public LoadingChargeState(EnemyBase enemy, float xForce)
        {
            _enemy = enemy;
            _xForce = xForce;
        }
        public void OnEnter()
        {
            ChargeFinished = false;
        }

        public void OnExit()
        {
            _enemy.SpriteRenderer.color = Color.white;
        }

        public void OnLogic()
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
            _enemy.SpriteRenderer.color += redding;
            _currentTimeInterval += Time.deltaTime;
            if (ChargeFinished)
            {
                ChargeFinished = false;
                chargeFinished?.Invoke(ChargeFinished);
                _enemy.SpriteRenderer.color = Color.white;
            }
        }

        private void Charge()
        {
            _currentTimeInterval = 0;
            _enemy.RigidBody.AddForce(new Vector2(_xForce, 0), ForceMode2D.Impulse);
            ChargeFinished = true;
            _enemy.SpriteRenderer.color = Color.black;
            
            chargeFinished?.Invoke(ChargeFinished);
        }
    }
}
