using _2_Scripts.Enemies.Temp_FirstApproach;
using Assets.Scripts.Enemies;
using UnityEngine;

namespace _2_Scripts.Enemies.States
{
    internal class JumpInPlaceState : IState
    {
        private DynamicEnemyBase _dynamicEnemy;
        [SerializeField] private float movingSpeed = 3f;
        private float _timeInterval = 0.5f;
        private float _currentTimeInterval = 0f;

        public JumpInPlaceState(DynamicEnemyBase dynamicEnemy)
        {
            _dynamicEnemy = dynamicEnemy;
        }
        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnLogic()
        {
            Debug.Log("JumpInPlace");
            Jump();
        }

        private void Jump()
        {
            if (_currentTimeInterval >= _timeInterval)
            {
                _currentTimeInterval = 0;
                _dynamicEnemy.RigidBody.AddForce(new Vector2(0, 20), ForceMode2D.Impulse);
            }
            else
                _currentTimeInterval += Time.deltaTime;
        }
    }
}
