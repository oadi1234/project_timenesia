using UnityEngine;

namespace Assets.Scripts.Enemies.States
{
    internal class AttackState_Fungy : IState
    {
        private Fungy _fungy;
        [SerializeField] private float movingSpeed = 3f;

        public AttackState_Fungy(Fungy fungy)
        {
            _fungy = fungy; 
        }

        public void Awake()
        {

        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnLogic()
        {
            Debug.Log("Atak grzyba: onLogic");
            if (_fungy.PlayerPosition.position.x - _fungy.RigidBody.position.x < 0)
            {
                _fungy.RigidBody.velocity = new Vector2(-movingSpeed, 0);
                _fungy.FlipSprite(false);
            }
            else
            {
                _fungy.RigidBody.velocity = new Vector2(movingSpeed, 0);
                _fungy.FlipSprite(true);
            }

        }
    }
}
