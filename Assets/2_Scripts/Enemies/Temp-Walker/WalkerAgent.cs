using System;
using _2___Scripts.Global;
using _2_Scripts.Enemies.Temp_SecondApproach;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_Walker
{
    public class WalkerAgent : DynamicEnemyBase
    {
        private WalkerStateMachine _stateMachine;
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new WalkerStateMachine(this);
            _stateMachine.ChangeState(_stateMachine.WalkRightTillObstacleState);
        }

        private void FixedUpdate()
        {
            _stateMachine.OnUpdate();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            switch (other.gameObject.layer)
            {
                case (int) LayerNames.Wall:
                    _stateMachine.SwitchDirection();
                    break;
                case (int) LayerNames.Hazard:
                    gameObject.SetActive(false);
                    Destroy(this);
                    break;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.otherCollider.name is "LeftEdge" or "RightEdge")
                _stateMachine.SwitchDirection();
        }
    }

    public class WalkerStateMachine : StateMachine
    {
        public readonly WalkTillObstacleState WalkRightTillObstacleState;
        public readonly WalkTillObstacleState WalkLeftTillObstacleState;
        public WalkerStateMachine(DynamicEnemyBase enemyBase)
        {
            WalkRightTillObstacleState = new WalkTillObstacleState(enemyBase, 1.5f, 1);
            WalkLeftTillObstacleState = new WalkTillObstacleState(enemyBase, 1.5f, -1);

            ChangeState(WalkRightTillObstacleState);
        }

        public void SwitchDirection()
        {
            ChangeState(CurrentState == WalkRightTillObstacleState 
                ? WalkLeftTillObstacleState
                : WalkRightTillObstacleState);
        }
    }

    public class WalkTillObstacleState : IState
    {
        private DynamicEnemyBase _enemy;
        private int _direction;
        private float _movingSpeed;
        public WalkTillObstacleState(DynamicEnemyBase enemy, float movingSpeed, int direction)
        {
            _enemy = enemy;
            _movingSpeed = movingSpeed;
            _direction = direction;
        }
        public void OnEnter()
        {
            //switch sprite here (or on exit)
            _enemy.RigidBody.velocity = new Vector2(_direction * _movingSpeed, 0);
        }

        public void OnExit()
        {
        }

        public void OnLogic()
        {
            _enemy.RigidBody.velocity = new Vector2(_direction * _movingSpeed, 0);

        }
    }
}