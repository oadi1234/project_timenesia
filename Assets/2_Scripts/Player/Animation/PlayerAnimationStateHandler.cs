using System;
using _2_Scripts.Player.Controllers;
using UnityEngine;
using _2_Scripts.Player.Animation.model;

namespace _2_Scripts.Player.Animation
{
    public class PlayerAnimationStateHandler : MonoBehaviour, IStateHandler
    {
        [SerializeField] private PlayerMovementController playerMovementController;

        [SerializeField] private PlayerInputManager playerInputManager;

        [SerializeField] private int currentState;
        [SerializeField] private int hurtLayerState;

        private int bufferedState;
        private float currentStateDuration = -1f; //total time of animation
        private float currentStateLockDuration = -1f; // for how long the animation is not changeable
        private float bufferedStateDuration = -1f;
        private float bufferedStateLockDuration = -1f;
        private float hurtDuration = -1f;
        private bool dashTriggered;
        private bool doubleJumpedTriggered;
        private bool hurtTriggered;
        private bool attackTriggered;
        private bool heavyHurtTriggered;
        private bool inputReceived;
        private bool attackOngoing = false;
        private static readonly float attackMoveMagnitude = 0.9f;

        void Awake()
        {
            RegisterEvents();
            ResetLogic();
        }

        void Update()
        {
            if (currentStateDuration < 0f && attackOngoing) attackOngoing = false;
            if (currentStateDuration > 0f) currentStateDuration -= Time.deltaTime;
            if (currentStateLockDuration > 0f) currentStateLockDuration -= Time.deltaTime;
            if (hurtDuration > 0f) hurtDuration -= Time.deltaTime;
            ClearBufferedState();
            currentState = GetState();
            ResetLogic();
            // if (currentStateLockDuration < 0f && currentStateDuration > 0f) Debug.Log("Combo window");
        }

        public int GetCurrentState()
        {
            return currentState;
        }

        public int GetCurrentFrame()
        {
            return (int)Math.Floor(currentStateDuration);
        }

        public float GetRotation()
        {
            return 0f;
        }

        public bool LockXFlip()
        {
            return attackOngoing;
        }

        private void ClearBufferedState()
        {
            if (bufferedState == currentState)
            {
                bufferedState = AC.None;
            }
        }

        private int GetState()
        {
            //being hurt can happen regardless of animation state locks - it is the highest priority animation
            //Technically speaking it can happen even while being hurt, but it shouldn't due to invulnerability frames.
            if (hurtTriggered && currentState != AC.HurtLight)
            {
                InterruptState();
                hurtLayerState = AC.HurtBlink;
                return PlayState(AC.HurtLight, AC.LightHurtDuration, AC.LightHurtStateLockDuration);
            }

            if (heavyHurtTriggered && currentState != AC.HurtHeavy)
            {
                InterruptState();
                hurtLayerState = AC.HurtBlink;
                return PlayState(AC.HurtHeavy, AC.HeavyHurtDuration, AC.HeavyHurtStateLockDuration);
            }

            if (currentStateLockDuration > 0f)
            {
                return currentState;
            }

            if (currentStateDuration > 0f && !inputReceived)
            {
                return currentState;
            }

            if (bufferedState != AC.None)
            {
                currentStateDuration = bufferedStateDuration;
                currentStateLockDuration = bufferedStateLockDuration;
                return bufferedState;
            }


            if (attackTriggered && playerInputManager.GetYInput() > 0f)
            {
                InterruptState();
                attackOngoing = true;
                playerMovementController.ReduceInputMoveStrength(AC.StaffAttackStateLockDuration, attackMoveMagnitude);
                return PlayState(AC.StaffAttackAirUp, AC.StaffAttackUpDuration, AC.StaffAttackStateLockDuration);
            }

            if (attackTriggered && playerMovementController.GetIsGrounded() && playerInputManager.GetYInput() == 0f)
            {
                InterruptState();
                attackOngoing = true;
                playerMovementController.AttackLunge();
                playerMovementController.ReduceInputMoveStrength(AC.StaffAttackStateLockDuration, attackMoveMagnitude);
                return PlayState(
                    currentState == AC.StaffAttack1
                        ? AC.StaffAttack2
                        : AC.StaffAttack1, AC.StaffAttackDuration,
                    AC.StaffAttackStateLockDuration);
            }

            if (!playerMovementController.GetIsGrounded() && attackTriggered && playerInputManager.GetYInput() < 0f)
            {
                InterruptState();
                attackOngoing = true;
                playerMovementController.ReduceInputMoveStrength(AC.StaffAttackStateLockDuration, attackMoveMagnitude);
                return PlayState(
                    currentState == AC.StaffAttackAirDown1
                        ? AC.StaffAttackAirDown2
                        : AC.StaffAttackAirDown1,
                    AC.StaffAttackDuration,
                    AC.StaffAttackStateLockDuration);
            }

            if (attackTriggered && !playerMovementController.GetIsGrounded())
            {
                InterruptState();
                attackOngoing = true;
                playerMovementController.ReduceInputMoveStrength(AC.StaffAttackStateLockDuration, attackMoveMagnitude);
                return PlayState(
                    currentState == AC.StaffAttackAir1
                        ? AC.StaffAttackAir2
                        : AC.StaffAttackAir1,
                    AC.StaffAttackDuration,
                    AC.StaffAttackStateLockDuration);
            }
            
            if (doubleJumpedTriggered)
            {
                InterruptState();
                return PlayState(AC.DoubleJump, AC.DoubleJumpDuration, AC.DoubleJumpStateLockDuration);
            }

            if (dashTriggered)
            {
                InterruptState();
                bufferedStateDuration = AC.DashEndDuration;
                bufferedStateLockDuration = AC.DashEndStateLockDuration;
                return PlayState(AC.Dash, AC.DashDuration, AC.DashStateLockDuration, AC.DashEnd);
            }

            if (playerMovementController.GetIsWallSliding())
            {
                InterruptState();
                return AC.Wallslide;
            }

            if (!attackOngoing || !(currentStateDuration > 0f))
            {
                if (!playerMovementController.GetIsGrounded() && playerMovementController.GetYVelocity() < 0f &&
                    !playerMovementController.IsOnCoyoteTime())
                {
                    InterruptState();
                    return AC.Descent;
                }

                if (!playerMovementController.GetIsGrounded() && playerMovementController.GetYVelocity() >= 0f)
                {
                    InterruptState();
                    return AC.Ascend;
                }


                if (playerMovementController.GetIsGrounded() && playerInputManager.GetXInput() == 0f &&
                    playerInputManager.IsInputEnabled)
                {
                    InterruptState();
                    if (currentState == AC.Run)
                    {
                        return PlayState(AC.RunStop, AC.RunStopDuration, 0f);
                    }

                    if (currentStateDuration > 0f)
                        return AC.RunStop;

                    return AC.Idle;
                }

                if (playerMovementController.GetIsGrounded() && playerInputManager.GetXInput() != 0f &&
                    playerInputManager.IsInputEnabled)
                {
                    InterruptState();
                    if (currentState == AC.Idle)
                    {
                        return PlayState(AC.RunStart, AC.RunStartDuration, 0f);
                    }

                    if (currentStateDuration > 0f)
                        return AC.RunStart;

                    return AC.Run;
                }
            }
            else
            {
                return currentState;
            }

            return AC.None;
        }

        private void InterruptState()
        {
            currentStateDuration = -1f;
            currentStateLockDuration = -1f;
            bufferedState = AC.None;
        }

        private int PlayState(int state, float stateDuration, float stateLockDuration, int bufferState)
        {
            bufferedState = bufferState;
            return PlayState(state, stateDuration, stateLockDuration);
        }

        private int PlayState(int state, float stateDuration, float stateLockDuration)
        {
            currentStateDuration = stateDuration;
            currentStateLockDuration = stateLockDuration;
            return state;
        }

        #region event_handling

        private void RegisterEvents()
        {
            playerMovementController.DoubleJumped += () => { doubleJumpedTriggered = true; };
            playerMovementController.Dashed += () => { dashTriggered = true; };
            playerInputManager.Attacked += () => { attackTriggered = true; };
            playerInputManager.InputReceived += () => { inputReceived = true; };
        }

        private void ResetLogic()
        {
            attackTriggered = false;
            dashTriggered = false;
            doubleJumpedTriggered = false;
            hurtTriggered = false;
            inputReceived = false;
            if (hurtDuration < 0f)
            {
                hurtLayerState = AC.HurtNone;
            }
        }

        #endregion


        private enum WeaponState
        {
            Staff,
            Orb,
            Daggerwand,
            Sword,
            Rod,
            Musket,
            Pistol,
            Unarmed,
            None
        }
    }
}