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

        private SpellType spellType; 
        private int bufferedState;
        private float currentStateDuration = -1f; //total time of animation
        private float currentStateLockDuration = -1f; // for how long the animation is not changeable
        private float bufferedStateDuration = -1f;
        private float bufferedStateLockDuration = -1f;
        private float hurtDuration = -1f;
        private float timeOnGround = 0f;
        private bool dashTriggered;
        private bool doubleJumpedTriggered;
        private bool hurtTriggered;
        private bool attackTriggered;
        private bool heavyHurtTriggered;
        private bool heavyAttackTriggered;
        private bool inputReceived;
        private bool shouldDoMovementStates = true;
        private const float AttackMoveMagnitude = 1f;
        private const float CooldownAfterLanding = 0.2f;

        void Awake()
        {
            RegisterEvents();
            ResetLogic();
        }

        void Update()
        {
            if (currentStateDuration < 0f && !shouldDoMovementStates) shouldDoMovementStates = true;
            if (currentStateDuration > 0f) currentStateDuration -= Time.deltaTime;
            if (currentStateLockDuration > 0f) currentStateLockDuration -= Time.deltaTime;
            if (hurtDuration > 0f) hurtDuration -= Time.deltaTime;
            if (playerMovementController.GetIsGrounded()) timeOnGround += Time.deltaTime;
            else timeOnGround = 0f;
            ClearBufferedState();
            currentState = GetState();
            ResetLogic();

            // if (currentStateLockDuration < 0f && currentStateDuration > 0f) Debug.Log("Combo window");
        }

        public int GetCurrentState()
        {
            return currentState;
        }

        public bool LockXFlip()
        {
            return !shouldDoMovementStates && currentState != AC.DashEnd;
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

            if (dashTriggered && (currentState != AC.HurtHeavy) == (currentState != AC.HurtLight))
            {
                InterruptState();
                BufferState(AC.DashEnd, AC.DashEndStateLockDuration, AC.DashEndDuration);
                return PlayState(AC.Dash, AC.DashDuration, AC.DashStateLockDuration);
            }

            if (currentStateLockDuration > 0f)
            {
                if (!(currentState == AC.Dash && playerMovementController.GetIsWallSliding()))
                    return currentState;
            }

            if (doubleJumpedTriggered)
            {
                InterruptState();
                shouldDoMovementStates = false;
                return PlayState(AC.DoubleJump, AC.DoubleJumpDuration, AC.DoubleJumpStateLockDuration);
            }
            
            if (heavyAttackTriggered)
            {
                InterruptState();
                shouldDoMovementStates = false;
                playerMovementController.AirHangForAttacks(AC.StaffHeavyAttackDuration, 0f);
                heavyAttackTriggered = false;
                playerInputManager.SetChargeCooldown(AC.StaffHeavyAttackStateLockDuration);
                return PlayState(AC.StaffHeavyAttack, AC.StaffHeavyAttackDuration,
                    AC.StaffHeavyAttackStateLockDuration);
            }

            if (playerMovementController.GetIsWallSliding())
            {
                InterruptState();
                return AC.Wallslide;
            }

            if (currentStateDuration > 0f && !inputReceived)
            {
                return currentState;
            }

            if (bufferedState != AC.None)
            {
                return PlayBufferedState();
            }

            if (spellType != SpellType.None)
            {
                if (spellType == SpellType.Bolt)
                {
                    InterruptState();
                    spellType = SpellType.None;
                    playerMovementController.AirHangForAttacks(AC.StaffSpellcastBoltStateLockDuration, 0.15f);
                    if (playerInputManager.GetYInput() > 0f)
                    {
                        return PlayState(AC.SpellcastStaffBoltUp, AC.StaffSpellcastBoltDuration,
                            AC.StaffSpellcastBoltStateLockDuration);
                    }

                    if (playerInputManager.GetYInput() < 0f)
                    {
                        return PlayState(AC.SpellcastStaffBoltDown, AC.StaffSpellcastBoltDuration,
                            AC.StaffSpellcastBoltStateLockDuration);
                    }

                    return PlayState(AC.SpellcastStaffBolt, AC.StaffSpellcastBoltDuration, AC.StaffSpellcastBoltStateLockDuration);
                }

                if (spellType == SpellType.Heavy)
                {
                    InterruptState();
                    spellType = SpellType.None;
                    playerMovementController.AirHangForAttacks(AC.StaffSpellcastHeavyStateLockDuration, 0f);
                    playerMovementController.AttackLunge(40f);
                    return PlayState(AC.SpellcastStaffHeavy, AC.StaffSpellcastHeavyDuration, AC.StaffSpellcastHeavyStateLockDuration);
                }
                
                if (spellType == SpellType.Aoe)
                {
                    InterruptState();
                    spellType = SpellType.None;
                    playerMovementController.AirHangForAttacks(AC.StaffSpellcastAoeStateLockDuration, 0f);
                    return PlayState(AC.SpellcastStaffAoE, AC.StaffSpellcastAoeDuration, AC.StaffSpellcastAoeStateLockDuration);
                }
                spellType = SpellType.None;
            }

            if (attackTriggered && playerInputManager.GetYInput() > 0f)
            {
                InterruptState();
                shouldDoMovementStates = false;
                playerMovementController.AdjustInputMoveStrength(AC.StaffAttackStateLockDuration, AttackMoveMagnitude);
                return PlayState(AC.StaffAttackAirUp, AC.StaffAttackUpDuration, AC.StaffAttackStateLockDuration);
            }

            if (attackTriggered && playerMovementController.GetIsGrounded() && playerInputManager.GetYInput() == 0f)
            {
                InterruptState();
                shouldDoMovementStates = false;
                playerMovementController.AdjustInputMoveStrength(AC.StaffAttackStateLockDuration, AttackMoveMagnitude);
                playerMovementController.AttackLunge(15f);
                return PlayState(
                    currentState == AC.StaffAttack1
                        ? AC.StaffAttack2
                        : AC.StaffAttack1, AC.StaffAttackDuration,
                    AC.StaffAttackStateLockDuration);
            }

            if (!playerMovementController.GetIsGrounded() && attackTriggered && playerInputManager.GetYInput() < 0f)
            {
                InterruptState();
                shouldDoMovementStates = false;
                playerMovementController.AdjustInputMoveStrength(AC.StaffAttackStateLockDuration, AttackMoveMagnitude);
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
                shouldDoMovementStates = false;
                playerMovementController.AdjustInputMoveStrength(AC.StaffAttackStateLockDuration, AttackMoveMagnitude);
                return PlayState(
                    currentState == AC.StaffAttackAir1
                        ? AC.StaffAttackAir2
                        : AC.StaffAttackAir1,
                    AC.StaffAttackDuration,
                    AC.StaffAttackStateLockDuration);
            }

            // If jumped during ground attack animation. Also has other attacks and double jump there to avoid skipping the animation.
            if (!shouldDoMovementStates && (currentState != AC.DoubleJump) && !IsGroundAttackState() &&
                !playerMovementController.GetIsGrounded())
            {
                InterruptState();
                shouldDoMovementStates = true;
            }

            if (playerMovementController.GetIsGrounded() && playerInputManager.IsConcentrating() &&
                playerMovementController.GetXVelocity() == 0f && timeOnGround > CooldownAfterLanding)
            {
                return AC.StaffConcentration;
            }

            if (shouldDoMovementStates || !(currentStateDuration > 0f))
            {
                if (playerMovementController.GetIsGrounded() && playerInputManager.GetXInput() == 0f)
                {
                    if (currentState == AC.Run)
                    {
                        InterruptState();
                        return PlayState(AC.RunStop, AC.RunStopDuration, 0f);
                    }

                    if (currentStateDuration > 0f)
                        return AC.RunStop;

                    return AC.Idle;
                }

                if (playerMovementController.GetIsGrounded() && playerInputManager.GetXInput() != 0f)
                {
                    if (currentState == AC.Idle)
                    {
                        InterruptState();
                        return PlayState(AC.RunStart, AC.RunStartDuration, 0f);
                    }

                    if (currentStateDuration > 0f)
                    {
                        return AC.RunStart;
                    }

                    return AC.Run;
                }

                if (!playerMovementController.GetIsGrounded() && playerMovementController.GetYVelocity() < 0f)
                {
                    InterruptState();
                    return AC.Descent;
                }

                if (!playerMovementController.GetIsGrounded() && playerMovementController.GetYVelocity() >= 0f)
                {
                    InterruptState();
                    return AC.Ascend;
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

        private void ShouldSkipMovementStatesOnBufferedState()
        {
            if (bufferedState == AC.DashEnd)
                shouldDoMovementStates = false;
        }

        private int PlayState(int state, float stateDuration, float stateLockDuration)
        {
            currentStateDuration = stateDuration;
            currentStateLockDuration = stateLockDuration;
            return state;
        }

        private void BufferState(int state, float lockDuration, float stateDuration)
        {
            bufferedStateDuration = stateDuration;
            bufferedStateLockDuration = lockDuration;
            bufferedState = state;
        }

        private int PlayBufferedState()
        {
            currentStateDuration = bufferedStateDuration;
            currentStateLockDuration = bufferedStateLockDuration;
            ShouldSkipMovementStatesOnBufferedState();
            return bufferedState;
        }

        #region event_handling

        private void RegisterEvents()
        {
            playerMovementController.DoubleJumped += () => { doubleJumpedTriggered = true; };
            playerMovementController.Dashed += () => { dashTriggered = true; };
            playerInputManager.Attacked += () => { attackTriggered = true; };
            playerInputManager.InputReceived += () => { inputReceived = true; };
            playerInputManager.HeavyAttack += () => { heavyAttackTriggered = true; };
            
            // TODO the thing below will not be how it will be handled. Ultimately playerSpellController will send
            //  actual spell type.
            playerInputManager.Spellcasted += (spell) => { spellType = (SpellType) spell; };
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

        private bool IsGroundAttackState()
        {
            return (currentState != AC.StaffAttack1) ==
                   (currentState != AC.StaffAttack2);
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