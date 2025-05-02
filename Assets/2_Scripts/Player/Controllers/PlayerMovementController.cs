using System;
using System.Collections;
using _2_Scripts.Global;
using _2_Scripts.Model;
using _2_Scripts.Movement.Shared.CollisionCheck;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.ScriptableObjects;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        #region variables

        [SerializeField] private Transform wallCheckMarker;

        private bool isDoubleJumping;
        private bool canDash; //only one dash midair.

        private bool facingLeft = false;
        private bool isGrounded;
        private bool nonCoyoteIsGrounded;
        private bool isJumping;
        private bool isWallJumping = false;
        private bool isWallSliding;
        private bool isWallTouching;
        private bool isOnSlope;
        private bool isDashing;
        private bool startedDashFromWallSlide = false;
        private bool airHangOngoing = false;
        private bool isLightHurtKnockedback = false;

        private bool inputJump;
        private bool inputJumpHeld;
        private bool inputMovementSkillEnabled;
        private bool isOnVerticalLunge = false;

        private float currentCoyoteTime;
        private float timeAfterJumpPressed; //cooldown for setting variables after pressing jump.
        private float currentDashCooldown;
        private float jumpTime = 0f;
        private float movementMagnitudeMultiplier = 1f;
        private float attackTimer;
        private float preLungeTimer;
        private float lungeSpeedX = 1f;
        private float lungeSpeedY = 0f;
        private float normalGravity;

        private float inputX;
        private float moveX;
        private float moveY;

        private float attackKnockbackX;
        private float attackKnockbackY;

        private float hurtKnockbackX;
        private float hurtKnockbackY;

        private Rigidbody2D rigidBody2D;
        private Vector2 velocityVector = Vector2.zero;
        private Vector2 startingPosition;
        private GroundChecker groundChecker;

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

        public event Action WallJumped;

        #endregion

        private void Awake()
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
            wallChecker = GetComponent<WallChecker>();
            groundChecker = GetComponent<GroundChecker>();
            //animator = GetComponentInChildren<Animator>(); //TODO remove. unfeasible as there might be many children with animators
            playerInputManager = GetComponent<PlayerInputManager>();
            effort = GetComponent<PlayerEffort>();
            startingPosition = rigidBody2D.position;
            currentCoyoteTime = PlayerConstants.Instance.coyoteTime;
            normalGravity = rigidBody2D.gravityScale;
            //GameDataManager.Instance.LoadFromSave(new SaveDataSchema{Coins = 10, CurrentHealth = 2, MaxHealth = 4, 
            //    MaxMana = 8, MaxConcentrationSlots = 2, SavePoint = "lol"});
        }

        private void FixedUpdate()
        {
            CalculateIsGrounded();
            IsTouchingWall();
            IsOnSlope();
            CheckIfShouldStopDash();
            KeepVelocityYAtZeroWhenDashing();
            Lunge();

            if (timeAfterJumpPressed > 0)
            {
                timeAfterJumpPressed -= Time.fixedDeltaTime;
            }

            if (currentDashCooldown > 0)
            {
                currentDashCooldown -= Time.fixedDeltaTime;
            }

            isOnVerticalLunge = !Mathf.Approximately(lungeSpeedY, 0);

            AssignVelocityVector();
        }

        #region Getters

        // UNRELIABLE FOR CHECKING ACTUAL SPRITE DIRECTION. Consult things like AnimatorHandler.cs:SetFacingDirection() 
        // to check how to find where the sprite is looking.
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

        public bool IsGrounded()
        {
            return isGrounded;
        }

        public bool IsWallSliding()
        {
            return isWallSliding;
        }

        public bool IsKnockedBackLight()
        {
            return isLightHurtKnockedback;
        }

        private bool GetAbilityFlag(AbilityName abilityName)
        {
            return GameDataManager.Instance.IsAbilityUnlocked(abilityName);
        }

        #endregion

        #region Input

        public void Move(float move)
        {
            move *= movementMagnitudeMultiplier;
            inputX = move;
        }

        public void Jump(bool jump, bool keyHeld, bool moveSkillInputEnabled)
        {
            inputJump = jump;
            inputJumpHeld = keyHeld;
            inputMovementSkillEnabled = moveSkillInputEnabled;
        }

        public void Dash(bool dash)
        {
            if (currentDashCooldown <= 0 && canDash && dash && GetAbilityFlag(AbilityName.Dash) &&
                UseEffort(1))
            {
                Dashed?.Invoke();
                SetVariablesWhenDashing();
                playerInputManager.BlockInput(AC.DashDuration);
                dashVariableReverserCoroutine = ReverseDashVariablesAfterSeconds(AC.DashDuration);
                StartCoroutine(dashVariableReverserCoroutine);
            }
        }

        private void SetPosition(Vector3 position)
        {
            velocityVector = Vector2.zero;
            gameObject.transform.position = position;
        }

        public void HurtKnockback(float knockbackStrength, Vector3 damageSourcePosition)
        {
            playerInputManager.BlockInput(PlayerConstants.Instance.knockbackTime);
            hurtKnockbackX = (transform.position.x - damageSourcePosition.x) * knockbackStrength * 4;
            hurtKnockbackY = (transform.position.y - damageSourcePosition.y) * knockbackStrength + 1f;
            playerInputManager.BlockInput(AC.LightHurtDuration);
            StartCoroutine(PerformLightHurtKnockback(AC.LightHurtDuration));
        }

        public void AttackKnockback(float knockbackStrength, Vector2 direction)
        {
            direction *= knockbackStrength;
            attackKnockbackX = direction.x * 0.4f;
            attackKnockbackY = direction.y;
        }

        public void AttackLunge(float speedX, float timer, float speedY = 0f)
        {
            if (attackTimer <= 0f)
            {
                lungeSpeedX = speedX;
                attackTimer = timer;
                lungeSpeedY = speedY;
            }
        }

        public void ResetAttackLunge()
        {
            attackTimer = 0f;
            lungeSpeedY = 0f;
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

        #endregion

        #region Waiters

        private IEnumerator PerformLightHurtKnockback(float seconds)
        {
            isLightHurtKnockedback = true;
            yield return new WaitForSeconds(seconds);
            isLightHurtKnockedback = false;
            hurtKnockbackX = 0;
            hurtKnockbackY = 0;
        }

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
            rigidBody2D.gravityScale = gravity;
            yield return new WaitForSecondsRealtime(seconds);
            rigidBody2D.gravityScale = normalGravity;
            airHangOngoing = false;
        }

        #endregion Waiters

        #region Checkers

        private void IsOnSlope()
        {
            isOnSlope = groundChecker.IsOnSlope();
        }

        private void CalculateIsGrounded()
        {
            groundChecker.CalculateRays();
            nonCoyoteIsGrounded = groundChecker.IsGrounded();

            if (nonCoyoteIsGrounded)
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

        private void Lunge()
        {
            if (attackTimer > 0f)
            {
                var sign = (Convert.ToInt32(!facingLeft) << 1) - 1;
                Move(lungeSpeedX * sign * attackTimer * 2);
                attackTimer -= Time.fixedDeltaTime;
            }
            else
                lungeSpeedY = 0;
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
            }
        }

        #endregion Checkers

        #region set_variables

        private void HandleSlipperySlopes()
        {
            if (isOnSlope || airHangOngoing)
                rigidBody2D.gravityScale = 0f;
            else
            {
                rigidBody2D.gravityScale = normalGravity;
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
                inputJump = inputJumpHeld;
            }
        }

        private void SetVariablesWhenWallSliding()
        {
            isJumping = false;
            jumpTime = 0f;
            currentCoyoteTime = PlayerConstants.Instance.coyoteTime;
            canDash = true;
        }


        private void SetVariablesWhenDashing()
        {
            currentDashCooldown = PlayerConstants.Instance.dashCooldown;
            isDashing = true;
            rigidBody2D.gravityScale = 0f;
            attackTimer = 0f;
            startedDashFromWallSlide = isWallSliding;
            moveX = PlayerConstants.Instance.dashSpeed *
                    ((Convert.ToInt32(inputX is < 0.01f and > -0.01f ? !facingLeft : (inputX > 0) == !isWallSliding) <<
                      1) - 1);
            moveY = 0f;
            DoFlip(moveX);
        }

        private void KeepVelocityYAtZeroWhenDashing()
        {
            if (isDashing && !isOnSlope)
            {
                moveY = 0f;
            }
        }

        private void EndDash()
        {
            isDashing = false;
            rigidBody2D.gravityScale = normalGravity;
            canDash = isGrounded || isWallTouching;
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

        private void AssignVelocityVector()
        {
            if (!isDashing && !isLightHurtKnockedback)
            {
                if (!isWallJumping)
                    HandleMovement();
                if (!airHangOngoing)
                    HandleJumping();
            }
            else if (isLightHurtKnockedback)
            {
                HandleHurtKnockback();
            }

            velocityVector.Set(
                moveX, moveY);
            rigidBody2D.velocity = velocityVector;
            ResetAttackKnockback();
        }

        private void HandleHurtKnockback()
        {
            moveX = hurtKnockbackX = Mathf.MoveTowards(hurtKnockbackX, 0, PlayerConstants.Instance.moveSpeed * 0.2f);
            moveY = hurtKnockbackY = Mathf.MoveTowards(hurtKnockbackY, -6f, PlayerConstants.Instance.moveSpeed * 0.2f);
            DoFlip(-moveX);
        }

        private void HandleMovement()
        {
            if (attackKnockbackY > 0)
                moveY = attackKnockbackY;
            else if (isOnVerticalLunge)
            {
                moveY = lungeSpeedY*attackTimer*2;
            }
            else if (airHangOngoing)
                moveY = Mathf.MoveTowards(moveY, -1f, PlayerConstants.Instance.maxVerticalSpeed * 0.1f);
            else
                moveY = rigidBody2D.velocity.y;

            if (!Mathf.Approximately(inputX, 0))
                moveX = Mathf.MoveTowards(moveX, inputX * PlayerConstants.Instance.moveSpeed,
                    PlayerConstants.Instance.moveSpeed * (attackTimer > 0 ? 1 : 0.25f)) + attackKnockbackX;
            else
            {
                moveX = Mathf.MoveTowards(moveX, 0, PlayerConstants.Instance.moveSpeed * 0.1f) + attackKnockbackX;
            }

            if (!isJumping && isGrounded && !isOnSlope)
            {
                moveY = !nonCoyoteIsGrounded && rigidBody2D.velocity.y > 0.001f && !isOnVerticalLunge ? -10 : moveY;
            }
            else if (!isJumping && isOnSlope && isGrounded)
            {
                bool movingTowardsWallOnSlope = (inputX > 0f && wallChecker.IsTouchingBottomOffset() &&
                                                 wallChecker.IsRightTouching())
                                                || (inputX < 0f && wallChecker.IsTouchingBottomOffset() &&
                                                    wallChecker.IsLeftTouching());
                moveX = movingTowardsWallOnSlope
                    ? 0f
                    : moveX;
                moveY = movingTowardsWallOnSlope ? -1f : groundChecker.slopeNormalPerpendicular.y * -moveX;
            }

            else if (!isGrounded && isWallSliding && !airHangOngoing)
            {
                moveY = PlayerConstants.Instance.wallSlideSpeed;
            }


            DoFlip(inputX);


            HandleSlipperySlopes();
        }

        private void HandleJumping()
        {
            if (inputJump && !isGrounded && isWallTouching && GetAbilityFlag(AbilityName.WallJump) &&
                inputMovementSkillEnabled)
            {
                WallJump();
            }
            else if (inputJump && isGrounded)
            {
                GroundJump();
            }
            else if (inputJump && !isDoubleJumping && GetAbilityFlag(AbilityName.DoubleJump) &&
                     inputMovementSkillEnabled)
            {
                DoubleJump();
            }
            else if ((isJumping || isDoubleJumping) && inputJumpHeld && rigidBody2D.velocity.y > 0 &&
                     jumpTime < PlayerConstants.Instance.maxJumpTime && !airHangOngoing)
            {
                KeyHoldAscendWhileJumping();
            }
            else if ((isJumping || isDoubleJumping) &&
                     (!inputJumpHeld || jumpTime >= PlayerConstants.Instance.maxJumpTime) &&
                     rigidBody2D.velocity.y > 0 && !isDashing)
            {
                LoseVelocityAfterJumping();
            }
        }


        private void KeyHoldAscendWhileJumping()
        {
            jumpTime += Time.fixedDeltaTime +
                        Time.fixedDeltaTime * Convert.ToInt32(isWallJumping) *
                        PlayerConstants.Instance.wallJumpPersistenceModifier + Time.fixedDeltaTime *
                        Convert.ToInt32(isDoubleJumping) * PlayerConstants.Instance.doubleJumpPersistenceModifier;
            moveY = PlayerConstants.Instance.jumpVerticalSpeed;
        }

        private void GroundJump()
        {
            currentCoyoteTime = 0f;
            timeAfterJumpPressed =
                PlayerConstants.Instance
                    .jumpGroundCheckCooldown; //a cooldown made to avoid setting values on being grounded a short while after jumping.
            isJumping = true;
            isGrounded = false;

            moveY = PlayerConstants.Instance.initialJumpForce;
        }

        private void WallJump()
        {
            WallJumped?.Invoke();
            SetVariablesWhenWallJumping();
            moveX = wallChecker.IsLeftTouching()
                ? PlayerConstants.Instance.moveSpeed * PlayerConstants.Instance.postWallJumpSpeedModifier
                : PlayerConstants.Instance.moveSpeed * -PlayerConstants.Instance.postWallJumpSpeedModifier;
            moveY = PlayerConstants.Instance.initialJumpForce;
            CheckFlipWhenWallJump();
            StartCoroutine(ReverseAfterWallJumpAfterSeconds(0.15f));
        }

        private void SetVariablesWhenWallJumping()
        {
            currentCoyoteTime = 0f;
            isWallJumping = true;
            isJumping = true;
            isWallSliding = false;
            isDoubleJumping = false;
            jumpTime = 0f;
        }

        private void DoubleJump()
        {
            currentCoyoteTime = 0f;
            isWallJumping =
                false; //reset the flag so wall jumping penalty for KeyHoldAscendWhileJumping does not apply.
            isDoubleJumping = true;
            jumpTime = 0;
            moveY = PlayerConstants.Instance.initialJumpForce;
            DoubleJumped?.Invoke();
        }

        private void LoseVelocityAfterJumping()
        {
            moveY = rigidBody2D.velocity.y * 0.8f; //reduces floatiness.
            jumpTime = PlayerConstants.Instance.maxJumpTime;
        }


        private void DoFlip(float direction)
        {
            if (direction > 0 && facingLeft)
            {
                Flip();
            }
            else if (direction < 0 && !facingLeft)
            {
                Flip();
            }
        }

        private void Flip()
        {
            facingLeft = !facingLeft;
            Flipped?.Invoke(facingLeft);
        }

        private void ResetAttackKnockback()
        {
            attackKnockbackX = 0;
            attackKnockbackY = 0;
        }

        #endregion
    }
}