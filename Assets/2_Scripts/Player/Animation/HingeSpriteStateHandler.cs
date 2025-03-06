using System;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Animation
{
    public class HingeSpriteStateHandler : MonoBehaviour, IStateHandler
    {
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private HingeSpriteType hingeSpriteType;
        [SerializeField] private PlayerAnimationStateHandler playerAnimationStateHandler;

        private Rigidbody2D pendulum;

        private Transform pendulumTransform;

        private int hingeObjectState;
        private int bufferedState;
        private float turnAroundTimer;
        private float currentStateDuration;
        private float directionDragAdjustment;

        void Awake()
        {
            pendulumTransform = GetComponent<Transform>();
            pendulum = GetComponent<Rigidbody2D>();
            playerMovementController.Flipped += (bleb) => { turnAroundTimer = 0.1f; };
        }

        void Update()
        {
            if (currentStateDuration > 0f) currentStateDuration -= Time.deltaTime;
            pendulum.drag = 20f * playerMovementController.GetTotalVelocity() / 10f + 10f + directionDragAdjustment;
            pendulum.gravityScale =
                5f / (playerMovementController.GetTotalVelocity() + 0.1f);
            if (turnAroundTimer > 0f)
            {
                turnAroundTimer -= Time.deltaTime;
                pendulum.drag = 5f;
                pendulum.gravityScale = 50f;
            }

            hingeObjectState = GetState();
            if (hingeObjectState == AC.HingeIdle)
            {
                pendulum.drag = 30f;
            }
        }

        private int GetState()
        {
            // Check if currently playing animation has a hinge element
            if (PivotPerFrame.Instance.IsIncorrectState(playerAnimationStateHandler.GetCurrentState()))
            {
                SetRotation(0f); //most animations work best with 0 rotation afterwards.
                // ResetVelocity();
                currentStateDuration = 0f;
                return AC.None;
            }

            if (currentStateDuration > 0f)
            {
                return hingeObjectState;
            }
            
            if (playerMovementController.GetTotalVelocity() < 2f)
            {
                if (hingeObjectState == AC.HingeMove)
                {
                    return PlayState(AC.HingeStopMove, AC.CloakStopMoveDuration);
                }
                return AC.HingeIdle;
            }

            if (playerMovementController.GetTotalVelocity() > 2f)
            {
                if (hingeObjectState == AC.HingeIdle)
                {
                    return PlayState(AC.HingeStartMove, AC.CloakStartMoveDuration);
                }
                return AC.HingeMove;
            }

            return AC.None;
        }

        public int GetCurrentState()
        {
            return hingeObjectState;
        }

        public float GetRotationAngle()
        {
            return pendulumTransform.eulerAngles.z;
        }

        public bool LockXFlip()
        {
            //it might be necessary to add something here in the future, when dealing with hurt anims for example.
            return false;
        }

        private int PlayState(int state, float stateDuration)
        {
            currentStateDuration = stateDuration;
            return state;
        }

        // Necessary to use after certain animations, i.e. dash, double jump, attack. Can also be useful for fast snap to angle if hit really hard
        private void SetRotation(float angle)
        {
            pendulum.SetRotation(angle);
        }

        public int GetHingeSpriteType()
        {
            return (int) hingeSpriteType;
        }

        private enum HingeSpriteType
        {
            None = 0,
            Hair = 1,
            Cloak = 2
        }
    }
}