using System;
using System.Collections;
using _2_Scripts.Global;
using _2_Scripts.Model;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.ScriptableObjects;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        #region variables

        [SerializeField] private Transform _wallCheckMarker;

        [SerializeField] private PhysicsMaterial2D _noFriction;

        [SerializeField] private PhysicsMaterial2D _allFriction;

        private bool isDoubleJumping;
        private bool canDash; //only one dash midair.

        private bool facingLeft = false;
        private bool isGrounded;
        private bool isJumping;
        private bool isWallJumping = false;
        private bool isWallSliding;
        private bool isWallTouching;
        private bool isOnSlope;
        private bool isDashing;
        private bool startedDashFromWallSlide = false;
        private bool airHangOngoing = false;

        private float currentCoyoteTime;
        private float timeAfterJumpPressed; //cooldown for setting variables after pressing jump.
        private float flipCooldown;
        private float tempValue; //temporary value to avoid constant reassignment or temp value creation on loop.
        private float currentDashCooldown;
        private float jumpTime = 0f;
        private float brakeSpeedY = 0f;
        private float brakeSpeedX = 0f;
        private float movementMagnitudeMultiplier = 1f;
        private float groundAttackTimer;
        private float lungeSpeed = 15f;
        private float normalGravity;

        private Rigidbody2D rigidBody2D;
        private Vector2 velocityVector;
        private Vector2 brakeVector = Vector2.zero;
        private Vector2 startingPosition;
        private FlatGroundChecker flatGroundChecker;

        private WallChecker wallChecker;

        //private Animator animator;
        private PlayerInputManager playerInputManager;

        //private Dictionary<AbilityName, bool> abilities;
        private PlayerEffort effort;
        private IEnumerator dashVariableReverserCoroutine;
        private IEnumerator inputStrengthCoroutine;
        private IEnumerator airHangCoroutine;

        #endregion

        #region movement_events

        public event Action Dashed;
        public event Action DoubleJumped;
        public event Action<bool> Flipped;
        public event Action DashEnd;

        #endregion

        private void Awake()
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
            wallChecker = GetComponent<WallChecker>();
            flatGroundChecker = GetComponent<FlatGroundChecker>();
            //animator = GetComponentInChildren<Animator>(); //TODO remove. unfeasible as there might be many children with animators
            playerInputManager = GetComponent<PlayerInputManager>();
            effort = GetComponent<PlayerEffort>();
            velocityVector = new Vector2();
            tempValue = PlayerConstants.Instance.moveSpeed;
            startingPosition = rigidBody2D.position;
            currentCoyoteTime = PlayerConstants.Instance.coyoteTime;
            normalGravity = rigidBody2D.gravityScale;
            //GameDataManager.Instance.LoadFromSave(new SaveDataSchema{Coins = 10, CurrentHealth = 2, MaxHealth = 4, 
            //    MaxMana = 8, MaxConcentrationSlots = 2, SavePoint = "lol"});
        }

        private void FixedUpdate()
        {
            IsGrounded();
            IsTouchingWall();
            IsOnSlope();
            CheckIfShouldStopDash();
            KeepVelocityYAtZeroWhenDashing();
            Brake();
            DoAttackLunge();

            if (timeAfterJumpPressed > 0)
            {
                timeAfterJumpPressed -= Time.fixedDeltaTime;
            }

            if (flipCooldown > 0)
            {
                flipCooldown -= Time.fixedDeltaTime;
            }

            if (currentDashCooldown > 0)
            {
                currentDashCooldown -= Time.fixedDeltaTime;
            }
        }

        private void Initialize()
        {
            ResetPosition(startingPosition);
        }

        private void Restart() // TODO remove from here - movement controller should not be doing a full restart. Might require something like a PlayerEventController.
        {
            //playerHealth.Restart();
            // StartCoroutine(BlockInputForSeconds(2f));
            Initialize();
        }

        #region Getters

        // UNRELIABLE FOR CHECKING ACTUAL SPRITE DIRECTION. Consult things like AnimationHandler.cs:43 to check how to
        // find where the sprite is looking.
        public bool IsFacingLeft()
        {
            return facingLeft;
        }

        public float GetXVelocity()
        {
            return rigidBody2D.velocity.x;
        }

        public float GetXVelocityVector()
        {
            return velocityVector.x;
        }

        public float GetYVelocity()
        {
            return rigidBody2D.velocity.y;
        }

        public float GetTotalVelocity()
        {
            return rigidBody2D.velocity.magnitude;
        }

        public bool GetIsGrounded()
        {
            return isGrounded;
        }

        public bool GetIsWallSliding()
        {
            return isWallSliding;
        }

        private bool GetAbilityFlag(AbilityName abilityName)
        {
            return GameDataManager.Instance.IsAbilityUnlocked(abilityName);
        }

        #endregion

        #region Movement

        public void Move(float move)
        {
            move *= movementMagnitudeMultiplier;
            if (!isJumping && isGrounded && !isOnSlope)
            {
                velocityVector.Set(
                    move * PlayerConstants.Instance.moveSpeed, rigidBody2D.velocity.y);
            }
            else if (!isJumping && isOnSlope && !isWallTouching)
            {
                velocityVector.Set(
                    move * PlayerConstants.Instance.moveSpeed,
                    flatGroundChecker.slopeNormalPerpendicular.y * -move * PlayerConstants.Instance.moveSpeed);
            }
            else if (!isJumping && isOnSlope && isWallTouching)
            {
                velocityVector.Set(
                    move * PlayerConstants.Instance.moveSpeed, 0f);
            }

            else if (!isGrounded)
            {
                if (isWallSliding && !airHangOngoing)
                    velocityVector.Set(move * PlayerConstants.Instance.moveSpeed,
                        PlayerConstants.Instance.wallSlideSpeed);
                else if (isWallJumping && facingLeft)
                {
                    velocityVector.Set(Mathf.Min(move * PlayerConstants.Instance.moveSpeed, rigidBody2D.velocity.x),
                        rigidBody2D.velocity.y);
                }
                else if (isWallJumping && !facingLeft)
                {
                    velocityVector.Set(Mathf.Max(move * PlayerConstants.Instance.moveSpeed, rigidBody2D.velocity.x),
                        rigidBody2D.velocity.y);
                }
                else
                    velocityVector.Set(move * PlayerConstants.Instance.moveSpeed, rigidBody2D.velocity.y);
            }

            rigidBody2D.velocity = velocityVector;
            
            DoFlip(move);

            HandleSlipperySlopes(move);
        }

        private void Brake()
        {
            //TODO I may have typed it weird in the past. Use Mathf.MoveTowards(current speed value, target value, maxdelta) instead of adding negative force to rigidbody.
            // I mean, the solution below works but it is a bit weird. Hard to adjust it. At the same time it allows for momentary burst of speed so maybe it is fine...
            // Might need to think about it.
            brakeSpeedX = 0;
            brakeSpeedY = 0;
            // Speed when falling/ascending. Most likely not ideal but it works.
            if (Math.Abs(rigidBody2D.velocity.y) > PlayerConstants.Instance.maxVerticalSpeed)
            {
                brakeSpeedY = Math.Abs(rigidBody2D.velocity.y) - PlayerConstants.Instance.maxVerticalSpeed +
                              brakeVector.y / 16;
            }

            if (Math.Abs(rigidBody2D.velocity.x) > PlayerConstants.Instance.maxHorizontalSpeed)
            {
                brakeSpeedX = Math.Abs(rigidBody2D.velocity.x) - PlayerConstants.Instance.maxHorizontalSpeed +
                              brakeVector.x / 16;
            }

            brakeVector.Set(rigidBody2D.velocity.x * brakeSpeedX / 2, rigidBody2D.velocity.y * brakeSpeedY / 2);
            rigidBody2D.AddForce(-brakeVector);
        }

        public void Jump(bool jump, bool keyHeld, bool moveSkillInputEnabled)
        {
            //TODO: It would be good to make it so that the jump buffers slightly or even that when jump is held the character keeps jumping.
            // Double jump takes precedence over the buffer.
            if (jump && !isGrounded && isWallTouching && GetAbilityFlag(AbilityName.WallJump) && moveSkillInputEnabled)
            {
                WallJump();
            }
            else if (jump && isGrounded)
            {
                GroundJump();
            }
            else if (jump && !isDoubleJumping && GetAbilityFlag(AbilityName.DoubleJump) && moveSkillInputEnabled)
            {
                DoubleJump();
            }
            else if ((isJumping || isDoubleJumping) && keyHeld && rigidBody2D.velocity.y > 0 &&
                     jumpTime < PlayerConstants.Instance.maxJumpTime)
            {
                KeyHoldAscendWhileJumping();
            }
            else if ((isJumping || isDoubleJumping) && (!keyHeld || jumpTime >= PlayerConstants.Instance.maxJumpTime) &&
                     rigidBody2D.velocity.y > 0 && !isDashing)
            {
                LoseVelocityAfterJumping();
            }
        }

        public void Dash(bool dash, float move)
        {
            if (currentDashCooldown <= 0 && canDash && dash && GetAbilityFlag(AbilityName.Dash) &&
                UseEffort(1))
            {
                Dashed?.Invoke();
                SetVariablesWhenDashing(move);
                playerInputManager.BlockInput(AC.DashDuration);
                dashVariableReverserCoroutine = ReverseDashVariablesAfterSeconds(AC.DashDuration);
                StartCoroutine(dashVariableReverserCoroutine);
            }
        }

        private void GroundJump()
        {
            timeAfterJumpPressed =
                PlayerConstants.Instance
                    .jumpGroundCheckCooldown; //a cooldown made to avoid setting values on being grounded a short while after jumping.
            isJumping = true;
            isGrounded = false;
            if (!flatGroundChecker.IsGrounded()) //coyote time case, otherwise player just awkwardly floats for a second
            {
                MidAirJumpVerticalSpeedReset();
            }

            rigidBody2D.AddForce(transform.up * PlayerConstants.Instance.initialJumpForce, ForceMode2D.Impulse);
        }

        private void WallJump()
        {
            SetVariablesWhenWallJumping();
            rigidBody2D.AddForce(
                (transform.up + (wallChecker.IsLeftTouching()
                    ? transform.right * PlayerConstants.Instance.postWallJumpSpeedModifier
                    : -transform.right * PlayerConstants.Instance.postWallJumpSpeedModifier)) *
                PlayerConstants.Instance.initialJumpForce, ForceMode2D.Impulse);
            CheckFlipWhenWallJump();
            StartCoroutine(ReverseAfterWallJumpAfterSeconds(0.15f));
        }

        private void DoubleJump()
        {
            isWallJumping =
                false; //reset the flag so wall jumping penalty for KeyHoldAscendWhileJumping does not apply.
            isDoubleJumping = true;
            MidAirJumpVerticalSpeedReset();
            jumpTime = 0;
            rigidBody2D.AddForce(transform.up * PlayerConstants.Instance.initialJumpForce, ForceMode2D.Impulse);
            DoubleJumped?.Invoke();
        }

        private void KeyHoldAscendWhileJumping()
        {
            jumpTime += Time.fixedDeltaTime +
                        Time.fixedDeltaTime * Convert.ToInt32(isWallJumping) *
                        PlayerConstants.Instance.wallJumpPersistenceModifier + Time.fixedDeltaTime *
                        Convert.ToInt32(isDoubleJumping) * PlayerConstants.Instance.doubleJumpPersistenceModifier;
            velocityVector.Set(rigidBody2D.velocity.x, PlayerConstants.Instance.jumpVerticalSpeed);
            rigidBody2D.velocity = velocityVector;
        }

        private void LoseVelocityAfterJumping()
        {
            // smooth ascend to 0 once key is no longer held. Allows for finer control without jerky movement.
            velocityVector.Set(rigidBody2D.velocity.x, rigidBody2D.velocity.y * 0.8f);
            rigidBody2D.velocity = velocityVector;
            jumpTime = PlayerConstants.Instance.maxJumpTime; //TODO tbh not sure why its here. Safe to delete?
        }

        private void MidAirJumpVerticalSpeedReset()
        {
            velocityVector.Set(rigidBody2D.velocity.x, 0);
            rigidBody2D.velocity = velocityVector;
        }

        private void DoFlip(float move)
        {
            if (move > 0 && facingLeft)
            {
                Flip();
            }
            else if (move < 0 && !facingLeft)
            {
                Flip();
            }
        }

        private void Flip()
        {
            if (flipCooldown <= 0)
            {
                facingLeft = !facingLeft;
                Flipped?.Invoke(facingLeft);
            }
        }

        private void ResetPosition(Vector3 position)
        {
            velocityVector = new Vector2();
            gameObject.transform.position = position;
        }

        public void Knockback(float knockbackStrength)
        {
            //TODO: Change the script to work sort of like RigidBody.AddExplosionForce, so the knockback happens away from its source always
            // https://stackoverflow.com/questions/34250868/unity-addexplosionforce-to-2d
            playerInputManager.BlockInput(PlayerConstants.Instance.knockbackTime);
            var sign = ((Convert.ToInt32(facingLeft) << 1) - 1);
            velocityVector = Vector2.zero;
            rigidBody2D.velocity = velocityVector;
            rigidBody2D.AddForce(new Vector2(knockbackStrength * sign, knockbackStrength * 0.8f), ForceMode2D.Impulse);

            // TODO slow time for a while here when hit
        }

        public void AttackLunge(float speed)
        {
            if (groundAttackTimer <= 0f)
            {
                lungeSpeed = speed;
                groundAttackTimer = AC.StaffAttackStateLockDuration;
            }
        }
        
        //Not really used right now, but might have some potential in the future?
        // I.e. "your attacks slow you down but deal 20% more damage" or something.
        public void AdjustInputMoveStrength(float seconds, float multiplier)
        {
            inputStrengthCoroutine = ReduceInputStrength(seconds, multiplier);
            StartCoroutine(inputStrengthCoroutine);
        }

        public void AirHangForAttacks(float seconds, float gravity)
        {
            if (airHangOngoing)
            {
                StopCoroutine(airHangCoroutine);
                StopCoroutine(inputStrengthCoroutine);
            }
            airHangCoroutine = AirHang(seconds, gravity);
            inputStrengthCoroutine = ReduceInputStrength(seconds, 0.1f);
            StartCoroutine(inputStrengthCoroutine);
            StartCoroutine(airHangCoroutine);
        }

        #endregion Movement

        #region Waiters

        private IEnumerator ReverseAfterWallJumpAfterSeconds(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            isWallJumping = false;
        }

        private IEnumerator ReverseDashVariablesAfterSeconds(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            EndDash();
        }

        private IEnumerator ReduceInputStrength(float seconds, float multiplier)
        {
            movementMagnitudeMultiplier = multiplier;
            yield return new WaitForSecondsRealtime(seconds);
            movementMagnitudeMultiplier = 1f;
        }

        private IEnumerator AirHang(float seconds, float gravity)
        {
            airHangOngoing = true;
            velocityVector = Vector2.zero;
            rigidBody2D.velocity = velocityVector;
            rigidBody2D.gravityScale = gravity;
            yield return new WaitForSecondsRealtime(seconds);
            rigidBody2D.gravityScale = normalGravity;
            airHangOngoing = false;
        }

        #endregion Waiters

        #region Checkers

        private void IsOnSlope()
        {
            isOnSlope = flatGroundChecker.IsOnSlope();
        }

        private void IsGrounded()
        {
            flatGroundChecker.CalculateRays();

            if (flatGroundChecker.IsGrounded())
            {
                SetVariablesWhenGrounded();
            }
            else if (currentCoyoteTime > 0f)
            {
                currentCoyoteTime -= Time.fixedDeltaTime;
            }
            else
            {
                isGrounded = false;
            }
        }

        private void IsTouchingWall()
        {
            wallChecker.CalculateRays(facingLeft, true);
            isWallTouching =
                wallChecker
                    .IsTouchingWall(); //if this is needed even when onGround, then it have to be moved outside of if statement

            if (!isGrounded)
            {
                if (CanWallSlide())
                {
                    isWallSliding =
                        facingLeft
                            ? playerInputManager.GetXInput() < 0f
                            : playerInputManager.GetXInput() > 0f;
                }
                else
                {
                    isWallSliding = false;
                }

                if (isWallSliding)
                {
                    SetVariablesWhenWallSliding();
                }
            }
        }

        private void CheckIfShouldStopDash()
        {
            if (isDashing && isWallTouching)
            {
                playerInputManager.StopBlockInput();
                StopCoroutine(dashVariableReverserCoroutine);
                EndDash();
                playerInputManager.SetInputEnabled(true);
            }
        }

        private void DoAttackLunge()
        {
            if (groundAttackTimer > 0f)
            {
                rigidBody2D.velocity = new Vector2(0, 0);
                var sign = ((Convert.ToInt32(!facingLeft) << 1) - 1);
                rigidBody2D.AddForce(new Vector2(groundAttackTimer * lungeSpeed * sign, 0f),
                    ForceMode2D.Impulse);
                groundAttackTimer -= Time.fixedDeltaTime;
            }
        }

        private bool CanWallSlide()
        {
            return isWallTouching &&
                   !isWallJumping &&
                   rigidBody2D.velocity.y <= 0 &&
                   (jumpTime <= 0f || jumpTime > PlayerConstants.Instance.minJumpTimeBeforeWallSlidingEnabled) &&
                   GetAbilityFlag(AbilityName.WallJump);
        }

        private void CheckFlipWhenWallJump()
        {
            if ((facingLeft && wallChecker.IsLeftTouching()) || (!facingLeft && wallChecker.IsRightTouching()))
            {
                Flip();
                flipCooldown = 0.2f;
            }
        }

        #endregion Checkers

        #region SETTING-VARIABLES

        private void HandleSlipperySlopes(float move)
        {
            if (isOnSlope && move == 0f)
            {
                rigidBody2D.sharedMaterial = _allFriction;
            }
            else
            {
                rigidBody2D.sharedMaterial = _noFriction;
            }
        }

        private void SetVariablesWhenGrounded()
        {
            if (timeAfterJumpPressed <= 0)
            {
                isGrounded = true;
                isWallTouching = false;
                isWallSliding = false;
                isWallJumping = false;
                isDoubleJumping = false;
                isJumping = false;
                canDash = true;
                jumpTime = 0f;
                currentCoyoteTime = PlayerConstants.Instance.coyoteTime;
            }
        }

        private void SetVariablesWhenWallSliding()
        {
            isJumping = false;
            jumpTime = 0f;
            currentCoyoteTime = PlayerConstants.Instance.coyoteTime;
            canDash = true;
        }

        private void SetVariablesWhenWallJumping()
        {
            rigidBody2D.velocity = new Vector2(0, 0);
            isWallJumping = true;
            isJumping = true;
            isWallSliding = false;
            isDoubleJumping = false;
            jumpTime = 0f;
        }

        private void SetVariablesWhenDashing(float move)
        {
            currentDashCooldown = PlayerConstants.Instance.dashCooldown;
            isDashing = true;
            tempValue = rigidBody2D.gravityScale;
            rigidBody2D.gravityScale = 0f;
            flipCooldown = 0f;
            groundAttackTimer = 0f;
            startedDashFromWallSlide = isWallSliding;
            // if (startedDashFromWallSlide) facingLeft = !facingLeft;
            velocityVector.Set(
                PlayerConstants.Instance.dashSpeed *
                ((Convert.ToInt32(move < 0.01f && move > -0.01f ? !facingLeft : (move > 0) == !isWallSliding) << 1) - 1),
                0f);
            DoFlip(velocityVector.x);
            rigidBody2D.sharedMaterial = _noFriction;
            rigidBody2D.velocity = velocityVector;
        }

        private void KeepVelocityYAtZeroWhenDashing()
        {
            if (isDashing && !isOnSlope)
            {
                velocityVector.Set(rigidBody2D.velocity.x, 0f);
                rigidBody2D.velocity = velocityVector;
            }
        }

        private void EndDash()
        {
            isDashing = false;
            rigidBody2D.gravityScale = tempValue;
            rigidBody2D.sharedMaterial = _allFriction;
            canDash = isGrounded || isWallTouching;
            currentDashCooldown = 0f;
            if (startedDashFromWallSlide)
            {
                isDoubleJumping = false;
                startedDashFromWallSlide = false;
            }
            jumpTime = 0f; // set so you can double jump after a dash
            DashEnd?.Invoke();
        }

        private bool UseEffort(int cost)
        {
            return effort.UseEffort(cost);
        }

        #endregion SETTING-VARIABLES
    }
}