using System.Collections.Generic;
using _2_Scripts.Player.Controllers;
using UnityEngine;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Animation.model.weaponData;
using _2_Scripts.Player.model;

namespace _2_Scripts.Player.Animation
{
    public class PlayerAnimationStateHandler : MonoBehaviour, IStateHandler
    {
        [SerializeField] private PlayerMovementController playerMovementController;

        [SerializeField] private PlayerInputManager playerInputManager;
        [SerializeField] private PlayerSpellController playerSpellController;

        [SerializeField] private int currentState;
        [SerializeField] private int hurtLayerState;

        private int bufferedState;
        private int animStage;
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
        private bool spellTriggered;
        private bool heavyHurtTriggered;
        private bool heavyAttackTriggered;
        private bool inputReceived;
        private bool shouldDoMovementStates = true;
        private bool endDash = true;
        private bool restartAnim = false;
        private const float AttackMoveMagnitude = 1f;
        private const float CooldownAfterLanding = 0.2f;

        private WeaponStateAnimationData weaponStateAnimationData;
        private AnimationData tempAnimData;


        void Awake()
        {
            weaponStateAnimationData = new StaffAnimationData();
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

        public bool ShouldRestartAnim()
        {
            return restartAnim;
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

            if (dashTriggered && (currentState != AC.HurtHeavy) == (currentState != AC.HurtLight) && !endDash)
            {
                InterruptState();
                BufferState(AC.DashEnd, AC.DashEndDuration, AC.DashEndStateLockDuration);
                return PlayState(AC.Dash, AC.DashDuration, AC.DashStateLockDuration);
            }

            if (dashTriggered && (currentState != AC.HurtHeavy) == (currentState != AC.HurtLight) && endDash)
            {
                if (currentState == AC.DashEnd)
                    restartAnim = true;
                else
                    return PlayState(AC.DashEnd, AC.DashEndDuration, AC.DashEndStateLockDuration);
            }

            if (currentStateLockDuration > 0f)
            {
                if (currentState == AC.Dash && endDash)
                {
                    InterruptState();
                    BufferState(AC.DashEnd, AC.DashEndDuration, AC.DashEndStateLockDuration);
                }
                else if (currentState == AC.DoubleJump)
                {
                    Debug.Log("beb");
                }
                else
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
                tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.HeavyAttack);
                shouldDoMovementStates = false;
                playerMovementController.AirHangForAttacks(tempAnimData.animationDuration, 0f);
                heavyAttackTriggered = false;
                playerInputManager.SetChargeCooldown(tempAnimData.animationLockDuration);
                return PlayState(tempAnimData);
            }

            if (playerMovementController.GetIsWallSliding())
            {
                InterruptState();
                return AC.Wallslide;
            }

            if (bufferedState != AC.None)
            {
                return PlayBufferedState();
            }

            if (currentStateDuration > 0f && !inputReceived)
            {
                return currentState;
            }

            if (spellTriggered)
            {
                if (playerSpellController.GetSpellType() == SpellType.Bolt)
                {
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.SpellBolt);
                    InterruptState();

                    if (playerInputManager.GetYInput() > 0f)
                    {
                        tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.SpellBoltUp);
                    }

                    if (playerInputManager.GetYInput() < 0f)
                    {
                        tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.SpellBoltDown);
                    }

                    playerMovementController.AirHangForAttacks(tempAnimData.animationLockDuration, 0.15f);
                    playerInputManager.BlockMovementSkillInput(tempAnimData.animationLockDuration);
                    return PlayState(tempAnimData);
                }

                if (playerSpellController.GetSpellType() == SpellType.Heavy)
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.SpellHeavy);
                    playerMovementController.AirHangForAttacks(tempAnimData.animationLockDuration, 0f);
                    playerMovementController.AttackLunge(40f);
                    playerInputManager.BlockMovementSkillInput(tempAnimData.animationLockDuration);
                    return PlayState(tempAnimData);
                }

                if (playerSpellController.GetSpellType() == SpellType.Aoe)
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.SpellAoE);
                    playerMovementController.AirHangForAttacks(tempAnimData.animationLockDuration, 0f);
                    playerInputManager.BlockMovementSkillInput(tempAnimData.animationLockDuration);
                    return PlayState(tempAnimData);
                }
            }

            if (attackTriggered)
            {
                if (playerInputManager.GetYInput() > 0f)
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.AttackUp);
                    shouldDoMovementStates = false;
                    playerMovementController.AdjustInputMoveStrength(tempAnimData.animationLockDuration,
                        AttackMoveMagnitude);
                    return PlayState(tempAnimData);
                }

                if (playerMovementController.GetIsGrounded() && playerInputManager.GetYInput() == 0f)
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.AttackGround);
                    shouldDoMovementStates = false;
                    playerMovementController.AdjustInputMoveStrength(tempAnimData.animationLockDuration,
                        AttackMoveMagnitude);
                    playerMovementController.AttackLunge(15f);
                    return PlayState(tempAnimData);
                }

                if (!playerMovementController.GetIsGrounded() && playerInputManager.GetYInput() < 0f)
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.AttackDown);
                    shouldDoMovementStates = false;
                    playerMovementController.AdjustInputMoveStrength(tempAnimData.animationLockDuration,
                        AttackMoveMagnitude);
                    return PlayState(tempAnimData);
                }

                if (!playerMovementController.GetIsGrounded())
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.AttackAir);
                    shouldDoMovementStates = false;
                    playerMovementController.AdjustInputMoveStrength(tempAnimData.animationLockDuration,
                        AttackMoveMagnitude);
                    return PlayState(tempAnimData);
                }
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
                tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.Concentration);
                return tempAnimData.animationStateHash;
            }

            if (shouldDoMovementStates || !(currentStateDuration > 0f))
            {
                animStage = 0;
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

                    if (currentState == AC.RunStop)
                    {
                        InterruptState();
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
                    return AC.Descend;
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

        private int PlayState(AnimationData data)
        {
            var state = data.animationStateHash;
            var i = 0;

            while (true)
            {
                if (data.chainsInto == null || data.chainsInto.animationStateHash == state)
                {
                    animStage = 0;
                    break;
                }

                if (i == animStage)
                {
                    animStage++;
                    break;
                }

                i++;
                data = data.chainsInto;
            }

            return PlayState(data.animationStateHash, data.animationDuration, data.animationLockDuration, false);
        }

        private int PlayState(int state, float stateDuration, float stateLockDuration, bool resetStageData = true)
        {
            if (resetStageData)
                animStage = 0;
            currentStateDuration = stateDuration;
            currentStateLockDuration = stateLockDuration;
            return state;
        }

        private void BufferState(int state, float stateDuration, float lockDuration)
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
            playerMovementController.DashEnd += () => { endDash = true; };
            playerSpellController.Spellcasted += () => { spellTriggered = true; };
            playerInputManager.Attacked += () => { attackTriggered = true; };
            playerInputManager.InputReceived += () => { inputReceived = true; };
            playerInputManager.HeavyAttack += () => { heavyAttackTriggered = true; };
        }

        private void ResetLogic()
        {
            attackTriggered = false;
            dashTriggered = false;
            doubleJumpedTriggered = false;
            hurtTriggered = false;
            inputReceived = false;
            spellTriggered = false;
            endDash = false;
            restartAnim = false;
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
    }
}