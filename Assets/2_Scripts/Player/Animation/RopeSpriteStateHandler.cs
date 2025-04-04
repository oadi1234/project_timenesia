using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Animation
{
    public class RopeSpriteStateHandler : MonoBehaviour, IStateHandler
    {
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private HingeSpriteType hingeSpriteType;
        [SerializeField] private PlayerAnimationStateHandler playerAnimationStateHandler;

        private int hingeObjectState;
        private int bufferedState;
        private float currentStateDuration;
        

        void Update()
        {
            if (currentStateDuration > 0f) currentStateDuration -= Time.deltaTime;

            hingeObjectState = GetState();
        }

        private int GetState()
        {
            // Check if currently playing animation has a rope element
            if (PointPerState.Instance.IsNonRopeState(playerAnimationStateHandler.GetCurrentState()))
            {
                currentStateDuration = 0f;
                return AC.None;
            }

            if (currentStateDuration > 0f)
            {
                return hingeObjectState;
            }
            
            if (playerMovementController.GetTotalVelocity() < 2f)
            {
                if (hingeObjectState == AC.RopeMove)
                {
                    return PlayState(AC.RopeStopMove, AC.CloakStopMoveDuration);
                }
                return AC.RopeIdle;
            }

            if (playerMovementController.GetTotalVelocity() > 2f)
            {
                if (hingeObjectState == AC.RopeIdle)
                {
                    return PlayState(AC.RopeStartMove, AC.CloakStartMoveDuration);
                }
                return AC.RopeMove;
            }

            return AC.None;
        }

        public int GetCurrentState()
        {
            return hingeObjectState;
        }

        public bool LockXFlip()
        {
            //it might be necessary to add something here in the future, when dealing with hurt anims for example.
            return false;
        }

        public bool ShouldRestartAnim()
        {
            // this one might actually be useless. Oh well.
            return false;
        }

        private int PlayState(int state, float stateDuration)
        {
            currentStateDuration = stateDuration;
            return state;
        }

        public int GetHingeSpriteType()
        {
            return (int) hingeSpriteType;
        }
        
        private void SetFacingDirection()
        {
            transform.localScale = new Vector3(transform.localScale.x, playerMovementController.IsFacingLeft() ? 1 : -1,
                transform.localScale.z);
        }

        private enum HingeSpriteType
        {
            None = 0,
            Hair = 1,
            Cloak = 2
        }
    }
}