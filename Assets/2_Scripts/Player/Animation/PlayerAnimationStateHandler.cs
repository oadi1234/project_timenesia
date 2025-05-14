using _2_Scripts.Player.Controllers;
using UnityEngine;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Animation.model.weaponData;
using _2_Scripts.Player.Statistics;

namespace _2_Scripts.Player.Animation
{
    public class PlayerAnimationStateHandler : MonoBehaviour, IStateHandler
    {
        [SerializeField] private PlayerMovementController playerMovementController;

        [SerializeField] private PlayerInputManager playerInputManager;
        [SerializeField] private PlayerSpellController playerSpellController;
        [SerializeField] private PlayerHealth playerHealth;

        [SerializeField] private int currentState;
        [SerializeField] private int hurtLayerState;

        private int bufferedState;
        private int animStage;
        private float currentStateDuration = -1f; //total time of animation
        private float currentStateLockDuration = -1f; // for how long the animation is not changeable
        private float bufferedStateDuration = -1f;
        private float bufferedStateLockDuration = -1f;
        private bool dashTriggered;
        private bool doubleJumpedTriggered;
        private bool wallJumpedTriggered;
        private bool attackTriggered;
        private bool spellTriggered;
        private bool heavyHurtTriggered;
        private bool heavyAttackTriggered;
        private bool inputReceived;
        private bool shouldDoMovementStates = true;
        private bool endDash = true;
        private bool restartAnim = false;
        private bool buffOngoing = false;
        private const float AttackMoveMagnitude = 1f;

        private WeaponStateAnimationData weaponStateAnimationData;
        private AnimationData tempAnimData;
        private AnimationData bufferedData;


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
            ClearBufferedState();
            currentState = GetState();
            ResetLogic();

            // if (currentStateLockDuration < 0f && currentStateDuration > 0f) Debug.Log("Combo window");
        }

        public int GetCurrentState()
        {
            return currentState;
        }

        public int GetCurrentHurtState()
        {
            return hurtLayerState;
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
            if (playerHealth.HasIFrames())
            {
                hurtLayerState = AC.HurtBlink;
            }
            else
            {
                hurtLayerState = AC.HurtNone;
            }

            if (playerMovementController.IsKnockedBackLight() && currentState != AC.HurtLight)
            {
                InterruptState();
                return PlayState(AC.HurtLight, AC.LightHurtDuration, AC.LightHurtStateLockDuration);
            }

            if (heavyHurtTriggered && currentState != AC.HurtHeavy)
            {
                InterruptState();
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
                else if (ShouldInterruptGroundAttackAnimation(
                             weaponStateAnimationData.GetForState(WeaponAnimationState.AttackGround)))
                {
                    InterruptState();
                    playerMovementController.ResetAttackLunge();
                }
                else if (ShouldInterruptDoubleJump())
                {
                    InterruptState();
                    shouldDoMovementStates = true;
                }
                else if (ShouldInterruptBuffAnimation(
                             weaponStateAnimationData.GetForState(WeaponAnimationState.SpellShortBuff)))
                {
                    InterruptState();
                }
                else
                    return currentState;
            }

            if (ShouldInterruptDoubleJump())
            {
                InterruptState();
                shouldDoMovementStates = true;
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

            if (playerMovementController.IsWallSliding() && !attackTriggered)
            {
                InterruptState();
                return AC.Wallslide;
            }

            if (bufferedData != null)
            {
                return PlayBufferedData();
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
                playerInputManager.SetConcentration(false);
                if (playerSpellController.GetSpellType() == SpellType.Bolt)
                {
                    playerSpellController.ClearSpellType();
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
                    playerSpellController.ClearSpellType();
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.SpellHeavy);
                    playerMovementController.AirHangForAttacks(tempAnimData.animationHangDuration, 0f);
                    playerInputManager.BlockMovementSkillInput(tempAnimData.animationLockDuration);
                    return PlayState(tempAnimData);
                }

                if (playerSpellController.GetSpellType() == SpellType.Aoe)
                {
                    playerSpellController.ClearSpellType();
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.SpellAoE);
                    playerMovementController.AirHangForAttacks(tempAnimData.animationLockDuration, 0f);
                    playerInputManager.BlockMovementSkillInput(tempAnimData.animationLockDuration);
                    return PlayState(tempAnimData);
                }

                if (playerSpellController.GetSpellType() == SpellType.Buff)
                {
                    playerSpellController.ClearSpellType();
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.SpellShortBuff);
                    bufferedData = tempAnimData;
                    if (currentState == weaponStateAnimationData.GetForState(WeaponAnimationState.Concentration)
                            .chainsInto.animationStateHash)
                        bufferedData = bufferedData.chainsInto;

                    return PlayBufferedData();
                }

                return PlayState(AC.None, 0, 0);
            }

            if (attackTriggered)
            {
                if (playerMovementController.IsWallSliding())
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.Wallsliding);
                    shouldDoMovementStates = false;
                    playerMovementController.AdjustInputMoveStrength(tempAnimData.animationLockDuration,
                        AttackMoveMagnitude);
                    return PlayState(tempAnimData);
                }

                if (playerInputManager.GetYInput() > 0f)
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.AttackUp);
                    shouldDoMovementStates = false;
                    playerMovementController.AdjustInputMoveStrength(tempAnimData.animationLockDuration,
                        AttackMoveMagnitude);
                    return PlayState(tempAnimData);
                }

                if (playerMovementController.IsGrounded() && playerInputManager.GetYInput() == 0f)
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.AttackGround);
                    shouldDoMovementStates = false;
                    playerMovementController.AdjustInputMoveStrength(tempAnimData.animationLockDuration,
                        AttackMoveMagnitude);
                    playerMovementController.AttackLunge(1f, AC.StaffAttackStateLockDuration);
                    return PlayState(tempAnimData);
                }

                if (!playerMovementController.IsGrounded() && playerInputManager.GetYInput() < 0f)
                {
                    InterruptState();
                    tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.AttackDown);
                    shouldDoMovementStates = false;
                    playerMovementController.AdjustInputMoveStrength(tempAnimData.animationLockDuration,
                        AttackMoveMagnitude);
                    return PlayState(tempAnimData);
                }

                if (!playerMovementController.IsGrounded())
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
            if (!shouldDoMovementStates && currentState != AC.DoubleJump && IsGroundAttackState() &&
                !playerMovementController.IsGrounded())
            {
                InterruptState();
                shouldDoMovementStates = true;
            }

            if (playerMovementController.IsGrounded() && playerInputManager.IsConcentrating() &&
                Mathf.Abs(playerMovementController.GetXVelocity()) < 0.05f)
            {
                tempAnimData = weaponStateAnimationData.GetForState(WeaponAnimationState.Concentration);
                if ((currentState == tempAnimData.animationStateHash && currentStateDuration <= 0f) || animStage == 1)
                    animStage = 1;

                else
                    animStage = 0;

                return PlayStateWithSetStage(tempAnimData);
            }

            if (!playerInputManager.IsConcentrating() && tempAnimData == weaponStateAnimationData
                    .GetForState(WeaponAnimationState.Concentration))
            {
                tempAnimData = weaponStateAnimationData.GetLastInChain(WeaponAnimationState.Concentration);
                animStage = 0;
                return PlayState(tempAnimData);
            }

            if (shouldDoMovementStates || !(currentStateDuration > 0f))
            {
                animStage = 0;
                if (playerMovementController.IsGrounded() && playerInputManager.GetXInput() == 0f)
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

                if (playerMovementController.IsGrounded() && playerInputManager.GetXInput() != 0f)
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

                if (!playerMovementController.IsGrounded() && playerMovementController.GetYVelocity() < 0f)
                {
                    InterruptState();
                    return AC.Descend;
                }

                if (!playerMovementController.IsGrounded() && playerMovementController.GetYVelocity() >= 0f)
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
            bufferedData = null;
            currentStateDuration = -1f;
            currentStateLockDuration = -1f;
            bufferedState = AC.None;
        }

        private void ShouldSkipMovementStatesOnBufferedState()
        {
            if (bufferedState == AC.DashEnd)
                shouldDoMovementStates = false;
        }

        private bool ShouldInterruptBuffAnimation(AnimationData animationData)
        {
            return IsStateHashInAnimationDataChain(currentState, animationData)
                   && (
                       !playerMovementController.IsGrounded()
                       || playerMovementController.GetTotalVelocity() > 0.05f
                       || attackTriggered
                       || !buffOngoing
                   );
        }

        private bool ShouldInterruptGroundAttackAnimation(AnimationData animData)
        {
            return IsStateHashInAnimationDataChain(currentState, animData) &&
                   !playerMovementController.IsGrounded();
        }

        private bool ShouldInterruptDoubleJump()
        {
            return currentState == AC.DoubleJump && wallJumpedTriggered;
        }

        private bool IsStateHashInAnimationDataChain(int animationStateHash, AnimationData animData)
        {
            while (animData != null)
            {
                if (animationStateHash == animData.animationStateHash)
                    return true;
                animData = animData.chainsInto;
            }

            return false;
        }

        private int PlayStateWithSetStage(AnimationData data)
        {
            // this method plays the current animation stage, found in the chain of AnimationData
            var i = 0;
            while (i != animStage)
            {
                if (data.chainsInto == null)
                    break;
                data = data.chainsInto;
                i++;
            }

            return PlayState(data.animationStateHash, data.animationDuration, data.animationLockDuration, false);
        }

        private int PlayState(AnimationData data)
        {
            // this method automatically advances stage once a button related to chained animation is pressed
            // var state = data.animationStateHash;
            var i = 0;

            while (true)
            {
                if (data.chainsInto == null) // || data.chainsInto.animationStateHash == state)
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

        // Buffering state only works with locked animations.
        private void BufferState(int state, float stateDuration, float lockDuration)
        {
            bufferedStateDuration = stateDuration;
            bufferedStateLockDuration = lockDuration;
            bufferedState = state;
        }

        private int PlayBufferedData()
        {
            if (currentStateDuration > 0f)
                return bufferedData.animationStateHash;
            tempAnimData = bufferedData;
            bufferedData = bufferedData.chainsInto;
            return PlayState(tempAnimData.animationStateHash, tempAnimData.animationDuration,
                tempAnimData.animationLockDuration);
        }

        private int PlayBufferedState()
        {
            currentStateDuration = bufferedStateDuration;
            currentStateLockDuration = bufferedStateLockDuration;
            ShouldSkipMovementStatesOnBufferedState();
            return bufferedState;
        }

        #region event_registering

        private void RegisterEvents()
        {
            playerMovementController.DoubleJumped += () => { doubleJumpedTriggered = true; };
            playerMovementController.Dashed += () => { dashTriggered = true; };
            playerMovementController.DashEnd += () => { endDash = true; };
            playerMovementController.WallJumped += () => { wallJumpedTriggered = true; };
            playerSpellController.Spellcasted += (spellType) =>
            {
                spellTriggered = true;
                buffOngoing = spellType == SpellType.Buff;
            };
            playerInputManager.Attacked += () => { attackTriggered = true; };
            playerInputManager.InputReceived += () => { inputReceived = true; };
            playerInputManager.HeavyAttack += () => { heavyAttackTriggered = true; };
        }

        private void ResetLogic()
        {
            attackTriggered = false;
            dashTriggered = false;
            doubleJumpedTriggered = false;
            wallJumpedTriggered = false;
            inputReceived = false;
            spellTriggered = false;
            endDash = false;
            restartAnim = false;
        }

        private bool IsGroundAttackState()
        {
            return IsStateHashInAnimationDataChain(currentState,
                weaponStateAnimationData.GetForState(WeaponAnimationState.AttackGround));
        }

        #endregion
    }
}