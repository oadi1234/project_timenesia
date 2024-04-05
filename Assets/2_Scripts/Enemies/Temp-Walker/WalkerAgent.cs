using System;
using _2___Scripts.Global;
using _2_Scripts.Enemies.Temp_SecondApproach;
using _2_Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_Walker
{
    public class WalkerAgent : DynamicEnemyBase
    {
        [SerializeField] private float movingSpeed;
        private WalkerStateMachine _stateMachine;
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new WalkerStateMachine(this, movingSpeed);
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
                case (int) Layers.Wall:
                    _stateMachine.SwitchDirection();
                    break;
                case (int) Layers.Hazard:
                    gameObject.SetActive(false);
                    Destroy(this);
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // if (other.otherCollider.name is "LeftEdge" or "RightEdge")
            if (other.gameObject.layer is (int) Layers.Wall or (int) Layers.Default)
                _stateMachine.SwitchDirection();
        }
    }

    public class WalkerStateMachine : StateMachine
    {
        public readonly WalkTillObstacleState WalkRightTillObstacleState;
        public readonly WalkTillObstacleState WalkLeftTillObstacleState;
        public WalkerStateMachine(DynamicEnemyBase enemyBase, float movingSpeed)
        {
            WalkRightTillObstacleState = new WalkTillObstacleState(enemyBase, movingSpeed, 1);
            WalkLeftTillObstacleState = new WalkTillObstacleState(enemyBase, movingSpeed, -1);

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
            _enemy.RigidBody.velocity = new Vector2(_direction * _movingSpeed, _enemy.RigidBody.velocity.y);
        }

        public void OnExit()
        {
        }

        public void OnLogic()
        {
            _enemy.RigidBody.velocity = new Vector2(_direction * _movingSpeed, _enemy.RigidBody.velocity.y);

        }
    }
}