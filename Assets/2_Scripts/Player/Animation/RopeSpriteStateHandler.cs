using _2_Scripts.Global.Animation;
using _2_Scripts.Global.Animation.Model;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Animation
{
    public class RopeSpriteStateHandler : AbstractStateHandler
    {
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private PlayerVerletRope playerVerletRope;
        [SerializeField] private HingeSpriteType hingeSpriteType;
        [SerializeField] private PlayerAnimationStateHandler playerAnimationStateHandler;

        private int hingeObjectState;
        private int bufferedState;
        private float currentStateDuration;
        private float lockMoveTimer = 0f;


        private void Update()
        {
            if (currentStateDuration > 0f) currentStateDuration -= Time.deltaTime;

            hingeObjectState = GetState();
        }

        private void FixedUpdate()
        {
            if (lockMoveTimer > 0f) lockMoveTimer -= Time.fixedDeltaTime;
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
            
            if (ShouldStartOrKeepMoving())
            {
                if (hingeObjectState == AC.RopeIdle)
                {
                    return PlayState(AC.RopeStartMove, AC.CloakStartMoveDuration);
                }
                return AC.RopeMove;
            }
            
            if (ShouldStopOrKeepIdling())
            {
                if (hingeObjectState == AC.RopeMove)
                {
                    return PlayState(AC.RopeStopMove, AC.CloakStopMoveDuration);
                }
                return AC.RopeIdle;
            }

            return AC.None;
        }

        public override int GetCurrentState()
        {
            return hingeObjectState;
        }

        public override int GetCurrentHurtState()
        {
            return playerAnimationStateHandler.GetCurrentHurtState();
        }

        public void SetMoveTimer(float time)
        {
            lockMoveTimer = time;
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

        private bool ShouldStartOrKeepMoving()
        {
            return playerMovementController.GetTotalVelocity() > 2f || lockMoveTimer > 0;
        }

        private bool ShouldStopOrKeepIdling()
        {
            return playerMovementController.GetTotalVelocity() < 2f;
        }

        private enum HingeSpriteType
        {
            None = 0,
            Hair = 1,
            Cloak = 2
        }
    }
}