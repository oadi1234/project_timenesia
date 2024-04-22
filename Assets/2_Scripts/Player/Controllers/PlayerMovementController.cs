using System;
using System.Collections;
using _2_Scripts.Global;
using _2_Scripts.Player.ScriptableObjects;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerMovementController: MonoBehaviour
    {
        #region variables

        [SerializeField]
        private Transform _wallCheckMarker;

        [SerializeField]
        private PhysicsMaterial2D _noFriction;

        [SerializeField]
        private PhysicsMaterial2D _allFriction;

        private bool isDoubleJumping;
        private bool canDash; //only one dash mid-air.

        private bool facingLeft = false;
        private bool isGrounded;
        private bool isJumping;
        private bool isWallJumping = false;
        private bool isWallSliding;
        private bool isWallTouching;
        private bool isOnSlope;
        private bool isDashing;

        private float currentCoyoteTime;
        private float timeAfterJumpPressed; //cooldown for setting variables after pressing jump.
        private float flipCooldown;
        private float tempValue; //temporary value to avoid constant reassignment or temp value creation on loop.
        private float currentDashCooldown;
        private float jumpTime = 0f;
        private float brakeSpeedY = 0f;
        private float brakeSpeedX = 0f;

        private Rigidbody2D rigidBody2D;
        private Vector2 velocityVector;
        private Vector2 brakeVector = Vector2.zero;
        private Vector2 startingPosition;
        private FlatGroundChecker flatGroundChecker;
        private WallChecker wallChecker;
        private Animator animator;
        private PlayerInputManager playerInputManager;
        //private Dictionary<AbilityName, bool> abilities;
        private PlayerEffort effort;
        private IEnumerator blockInputCoroutine;
        private IEnumerator dashVariableReverserCoroutine;
        private static readonly int doubleJumpTrigger = Animator.StringToHash("doubleJump");
        private static readonly int groundJumpTrigger = Animator.StringToHash("groundJump");
        private static readonly int wallJumpTrigger = Animator.StringToHash("wallJump");
        #endregion

        private void Awake()
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
            wallChecker = GetComponent<WallChecker>();
            flatGroundChecker = GetComponent<FlatGroundChecker>();
            animator = GetComponentInChildren<Animator>();
            playerInputManager = GetComponent<PlayerInputManager>();
            effort = GetComponent<PlayerEffort>();
            velocityVector = new Vector2();
            tempValue = PlayerConstants.Instance.moveSpeed;
            startingPosition = rigidBody2D.position;
            currentCoyoteTime = PlayerConstants.Instance.coyoteTime;
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

            animator.SetBool("isGrounded", isGrounded); // TODO move animator to Update on its own class.
            animator.SetBool("wallSliding", isWallSliding);
            // _animator.SetFloat("Hurt", _hurtTime);

            if (timeAfterJumpPressed > 0)
            {
                timeAfterJumpPressed -= Time.fixedDeltaTime;
            }
            if (flipCooldown>0)
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
            StartCoroutine(BlockInputForSeconds(2f));
            Initialize();
        }

        #region Getters
        public bool IsFacingLeft()
        {
            return facingLeft;
        }

        private bool GetAbilityFlag(AbilityName abilityName)
        {
            return GameDataManager.Instance.IsAbilityUnlocked(abilityName);
        }
        #endregion

        #region Movement
        public void Move(float move)
        {
            if (!isJumping && isGrounded && !isOnSlope)
            {
                velocityVector.Set(
                    move * PlayerConstants.Instance.moveSpeed, 0f);
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
                if (isWallSliding)
                    velocityVector.Set(move * PlayerConstants.Instance.moveSpeed, PlayerConstants.Instance.wallSlideSpeed);
                else if (isWallJumping && facingLeft)
                {
                    velocityVector.Set(Mathf.Min(move * PlayerConstants.Instance.moveSpeed, rigidBody2D.velocity.x), rigidBody2D.velocity.y);
                }
                else if (isWallJumping && !facingLeft)
                {
                    velocityVector.Set(Mathf.Max(move * PlayerConstants.Instance.moveSpeed, rigidBody2D.velocity.x), rigidBody2D.velocity.y);
                }
                else
                    velocityVector.Set(move * PlayerConstants.Instance.moveSpeed, rigidBody2D.velocity.y);
            }

            rigidBody2D.velocity = velocityVector;

            //if (!flatGroundChecker.CornerAhead())
            //{
            //    Debug.Log("Test");
            //    rigidBody2D.AddForce(Vector2.down*2, ForceMode2D.Impulse);
            //}


            if (move > 0 && facingLeft)
            {
                Flip();
            }
            else if (move < 0 && !facingLeft)
            {
                Flip();
            }

        
            HandleSlipperySlopes(move);

            animator.SetInteger("speedX", Math.Sign(move));
        }

        private void Brake()
        {
            brakeSpeedX = 0;
            brakeSpeedY = 0;
            // Speed when falling/ascending. Most likely not ideal but it works.
            if (Math.Abs(rigidBody2D.velocity.y) > PlayerConstants.Instance.maxVerticalSpeed)
            {
                brakeSpeedY = Math.Abs(rigidBody2D.velocity.y) - PlayerConstants.Instance.maxVerticalSpeed + brakeVector.y/16;
            }
            if (Math.Abs(rigidBody2D.velocity.x) > PlayerConstants.Instance.maxHorizontalSpeed)
            {
                brakeSpeedX = Math.Abs(rigidBody2D.velocity.x) - PlayerConstants.Instance.maxHorizontalSpeed + brakeVector.x/16;

            }
            brakeVector.Set(rigidBody2D.velocity.x * brakeSpeedX/2, rigidBody2D.velocity.y * brakeSpeedY/2);
            rigidBody2D.AddForce(-brakeVector);
        }

        public void Jump(bool jump, bool keyHeld)
        {
            if (jump && !isGrounded && isWallTouching && GetAbilityFlag(AbilityName.WallJump))
            {
                WallJump();
            }
            else if (jump && isGrounded)
            {
                GroundJump();
            }
            else if (jump && !isDoubleJumping && GetAbilityFlag(AbilityName.DoubleJump))
            {
                DoubleJump();
            }
            else if ((isJumping || isDoubleJumping) && keyHeld && rigidBody2D.velocity.y > 0 && jumpTime < PlayerConstants.Instance.maxJumpTime)
            {
                KeyHoldAscendWhileJumping();
            }
            else if ((isJumping || isDoubleJumping) && (!keyHeld || jumpTime >= PlayerConstants.Instance.maxJumpTime) && rigidBody2D.velocity.y > 0 && !isDashing) 
            {
                LoseVelocityAfterJumping();
            }

            animator.SetFloat("speedY", rigidBody2D.velocity.y); // TODO move animator events to its own class
        }
        public void Dash(bool dash, float move)
        {
            if (currentDashCooldown <= 0 && canDash && dash && GetAbilityFlag(AbilityName.Dash) && UseEffort(1)) //use effort for testing only
            {
                SetVariablesWhenDashing(move);
                blockInputCoroutine = BlockInputForSeconds(0.25f);
                dashVariableReverserCoroutine = ReverseDashVariablesAfterSeconds(0.25f);
                StartCoroutine(blockInputCoroutine);
                StartCoroutine(dashVariableReverserCoroutine);
            }
        }

        private void GroundJump()
        {
            timeAfterJumpPressed = PlayerConstants.Instance.jumpGroundCheckCooldown; //a cooldown made to avoid setting values on being grounded a short while after jumping.
            isJumping = true;
            isGrounded = false;
            if (!flatGroundChecker.IsGrounded()) //coyote time case, otherwise player just awkwardly floats for a second
            {
                MidAirJumpVerticalSpeedReset();
            }
            rigidBody2D.AddForce(transform.up * PlayerConstants.Instance.initialJumpForce, ForceMode2D.Impulse);
            animator.SetTrigger(groundJumpTrigger);
        }

        private void WallJump()
        {
            SetVariablesWhenWallJumping();
            rigidBody2D.AddForce((transform.up + (wallChecker.IsLeftTouching() ? 
                transform.right * PlayerConstants.Instance.postWallJumpSpeedModifier : 
                -transform.right * PlayerConstants.Instance.postWallJumpSpeedModifier)) * PlayerConstants.Instance.initialJumpForce, ForceMode2D.Impulse);
            CheckFlipWhenWallJump();
            StartCoroutine(ReverseAfterWallJumpAfterSeconds(0.15f));
        }

        private void DoubleJump()
        {
            isWallJumping = false; //reset the flag so wall jumping penalty for KeyHoldAscendWhileJumping does not apply.
            isDoubleJumping = true;
            MidAirJumpVerticalSpeedReset();
            jumpTime = 0;
            rigidBody2D.AddForce(transform.up * PlayerConstants.Instance.initialJumpForce, ForceMode2D.Impulse);
            animator.SetTrigger(doubleJumpTrigger);
        }

        private void KeyHoldAscendWhileJumping()
        {
            jumpTime += Time.fixedDeltaTime + Time.fixedDeltaTime * Convert.ToInt32(isWallJumping) * PlayerConstants.Instance.wallJumpPersistenceModifier + Time.fixedDeltaTime * Convert.ToInt32(isDoubleJumping) * PlayerConstants.Instance.doubleJumpPersistenceModifier;
            velocityVector.Set(rigidBody2D.velocity.x, PlayerConstants.Instance.jumpVerticalSpeed);
            rigidBody2D.velocity = velocityVector;
        }

        private void LoseVelocityAfterJumping()
        {
            // smooth ascend to 0 once key is no longer held. Allows for finer control without jerky movement.
            velocityVector.Set(rigidBody2D.velocity.x, rigidBody2D.velocity.y * 0.8f);
            rigidBody2D.velocity = velocityVector;
            jumpTime = 99f;
        }

        private void MidAirJumpVerticalSpeedReset()
        {
            velocityVector.Set(rigidBody2D.velocity.x, 0);
            rigidBody2D.velocity = velocityVector;
        }

        public void Flip()
        {
            if (flipCooldown <= 0)
            {
                facingLeft = !facingLeft;
                // Vector3 scale = transform.localScale;
                // scale.x *= -1;
                // transform.localScale = scale;
                //TODO: refactor this: flip shouldn't flip whole game object, but only a sprite as this can generate mess
            }
        }

        private void ResetPosition(Vector3 position)
        {
            velocityVector = new Vector2();
            gameObject.transform.position = position;
        }

        public void Knockback(float knockbackStrength)
        {
            StartCoroutine(BlockInputForSeconds(PlayerConstants.Instance.knockbackTime));
            var sign = ((Convert.ToInt32(facingLeft) << 1) - 1);
            velocityVector = Vector2.zero;
            rigidBody2D.velocity = velocityVector;
            rigidBody2D.AddForce(new Vector2(knockbackStrength * sign, knockbackStrength*0.8f), ForceMode2D.Impulse);
            
            // TODO slow time for a while here. Getting pushed around by enemies while 
            
        }


        #endregion Movement

        #region Waiters
        private IEnumerator BlockInputForSeconds(float seconds)
        {
            playerInputManager.SetInputEnabled(false);
            yield return new WaitForSecondsRealtime(seconds);
            playerInputManager.SetInputEnabled(true);
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
            else if (currentCoyoteTime > 0)
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
            isWallTouching = wallChecker.IsTouchingWall();  //if this is needed even when onGround, then it have to be moved outside of if statement

            if (!isGrounded)
            {
                if (CanWallSlide())
                {
                    isWallSliding = facingLeft ? Input.GetKey(KeyCode.LeftArrow) : Input.GetKey(KeyCode.RightArrow);
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
                StopCoroutine(blockInputCoroutine);
                StopCoroutine(dashVariableReverserCoroutine);
                EndDash();
                playerInputManager.SetInputEnabled(true);
           
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
            animator.SetTrigger(wallJumpTrigger);
        }
        private void SetVariablesWhenDashing(float move)
        {
            currentDashCooldown = PlayerConstants.Instance.dashCooldown;
            isDashing = true;
            tempValue = rigidBody2D.gravityScale;
            rigidBody2D.gravityScale = 0f;
            velocityVector.Set(
                PlayerConstants.Instance.dashSpeed * ((Convert.ToInt32(move <0.01f && move>-0.01f ? !facingLeft : move > 0) << 1) - 1),
                0f);
            rigidBody2D.sharedMaterial = _noFriction;
            rigidBody2D.velocity = velocityVector;
            animator.SetBool("dashing", true);
        }

        private void KeepVelocityYAtZeroWhenDashing()
        {
            if(isDashing && !isOnSlope)
            {
                velocityVector.Set(rigidBody2D.velocity.x, 0f);
                rigidBody2D.velocity = velocityVector;
            }
        }

        private void EndDash()
        {
            isDashing = false;
            rigidBody2D.gravityScale = tempValue;
            animator.SetBool("dashing", false);
            rigidBody2D.sharedMaterial = _allFriction;
            canDash = isGrounded || isWallTouching;
            jumpTime = 0f; // set so you can double jump after a dash
        }

        private bool UseEffort(int cost)
        {
            return effort.UseEffort(cost);
        }

        #endregion SETTING-VARIABLES
    }
}
