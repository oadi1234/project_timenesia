using System;
using System.Collections;
using _2___Scripts.Global;
using _2___Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

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
    private bool isGettingKnockedBack = false;
    private bool isGrounded;
    private bool isJumping;
    private bool isWallJumping = false;
    private bool isWallSliding;
    private bool isWallTouching;
    private bool isOnSlope;
    private bool isDashing;

    private float currentCoyoteTime;
    private float timeAfterJumpPressed; //cooldown for setting variables after pressing jump.
    private float currentKnockbackTime;
    private float knockbackStrength;
    private float flipCooldown;
    private float hurtTime;
    private float tempValue; //temporary value to avoid constant reassignment or temp value creation on loop.
    private float currentDashCooldown;
    private float jumpTime = 0f;
    private float brakeSpeedY = 0f;
    private float brakeSpeedX = 0f;

    private Rigidbody2D rigidBody2D;
    private Vector2 velocityVector;
    private Vector2 brakeVector = Vector2.zero;
    private Vector3 startingPosition;
    private FlatGroundChecker flatGroundChecker;
    private WallChecker wallChecker;
    private Animator animator;
    private PlayerInputManager playerInputManager;
    private PlayerHealth playerHealth;
    private PlayerAbilityAndStats stats;
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
        playerHealth = GetComponent<PlayerHealth>();
        effort = GetComponent<PlayerEffort>();
        velocityVector = new Vector2();
        tempValue = PlayerConstants.instance.moveSpeed;
        hurtTime = 0f;
        startingPosition = rigidBody2D.position;
        currentCoyoteTime = PlayerConstants.instance.coyoteTime;
        currentKnockbackTime = PlayerConstants.instance.knockbackTime;
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
        Brake(); //might be moved to "Move()" so it becomes less costly, but the operations don't seem too intensive.

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("wallSliding", isWallSliding);
        // _animator.SetFloat("Hurt", _hurtTime);

        Knockback();

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

    private void Restart()
    {
        playerHealth.Restart();
        StartCoroutine(BlockInputForSeconds(2f));
        Initialize();
    }

    #region Getters
    public bool IsFacingLeft()
    {
        return facingLeft;
    }
    #endregion

    #region Movement
    public void Move(float move)
    {
        if (!isJumping && isGrounded && !isOnSlope)
        {
            velocityVector.Set(
                move * PlayerConstants.instance.moveSpeed, 0f);
            //PlayerConstants.instance.moveSpeed * flatGroundChecker.slopeNormalPerpendicular.y * -move * Convert.ToInt32(isOnSlope));
        }
        else if (!isJumping && isOnSlope && !isWallTouching)
        {
            velocityVector.Set(
                move * PlayerConstants.instance.moveSpeed,
                PlayerConstants.instance.moveSpeed * flatGroundChecker.slopeNormalPerpendicular.y * -move * Convert.ToInt32(isOnSlope));
        }
        else if (!isJumping && isOnSlope && isWallTouching)
        {
            velocityVector.Set(
                move * PlayerConstants.instance.moveSpeed, 0f);
        }

        else if (!isGrounded)
        {
            if (isWallSliding)
                velocityVector.Set(move * PlayerConstants.instance.moveSpeed, PlayerConstants.instance.wallSlideSpeed);

            else if (isWallJumping && facingLeft)   //if jumped of the wall recently {
            {
                velocityVector.Set(Mathf.Min(move * PlayerConstants.instance.moveSpeed, rigidBody2D.velocity.x), rigidBody2D.velocity.y);
            }
            else if (isWallJumping && !facingLeft)
            {
                velocityVector.Set(Mathf.Max(move * PlayerConstants.instance.moveSpeed, rigidBody2D.velocity.x), rigidBody2D.velocity.y);
            }
            else
                velocityVector.Set(move * PlayerConstants.instance.moveSpeed, rigidBody2D.velocity.y);
        }
        
        rigidBody2D.velocity = velocityVector;

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
        if (Math.Abs(rigidBody2D.velocity.y) > PlayerConstants.instance.maxVerticalSpeed)
        {
            brakeSpeedY = Math.Abs(rigidBody2D.velocity.y) - PlayerConstants.instance.maxVerticalSpeed + brakeVector.y/16;
        }
        if (Math.Abs(rigidBody2D.velocity.x) > PlayerConstants.instance.maxHorizontalSpeed)
        {
            brakeSpeedX = Math.Abs(rigidBody2D.velocity.x) - PlayerConstants.instance.maxHorizontalSpeed + brakeVector.x/16;

        }
        brakeVector.Set(rigidBody2D.velocity.x * brakeSpeedX/2, rigidBody2D.velocity.y * brakeSpeedY/2);
        rigidBody2D.AddForce(-brakeVector);
    }

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

    public void Jump(bool jump, bool keyHeld)
    {
        if (jump && !isGrounded && isWallTouching && stats.GetAbilityFlag(AbilityName.WallJump))
        {
            WallJump();
        }
        else if (jump && isGrounded)
        {
            GroundJump();
        }
        else if ((isJumping || isDoubleJumping) && keyHeld && rigidBody2D.velocity.y > 0 && jumpTime < PlayerConstants.instance.maxJumpTime)
        {
            KeyHoldAscendWhileJumping();
        }
        else if ((isJumping || isDoubleJumping) && (!keyHeld || jumpTime >= PlayerConstants.instance.maxJumpTime) && rigidBody2D.velocity.y > 0 && !isDashing) 
        {
            LoseVelocityAfterJumping();
        }
        else if (jump && !isDoubleJumping && stats.GetAbilityFlag(AbilityName.DoubleJump))
        {
            DoubleJump();
        }

        animator.SetFloat("speedY", rigidBody2D.velocity.y); // TODO move animator events to its own class
    }

    private void GroundJump()
    {
        timeAfterJumpPressed = PlayerConstants.instance.jumpGroundCheckCooldown; //a cooldown made to avoid setting values on being grounded a short while after jumping.
        isJumping = true;
        isGrounded = false;
        if (!flatGroundChecker.IsGrounded()) //coyote time case, otherwise player just awkwardly floats for a second
        {
            MidAirJumpVerticalSpeedReset();
        }
        rigidBody2D.AddForce(transform.up * PlayerConstants.instance.initialJumpForce, ForceMode2D.Impulse);
        animator.SetTrigger(groundJumpTrigger);
    }

    private void WallJump()
    {
        SetVariablesWhenWallJumping();
        rigidBody2D.AddForce((transform.up + (wallChecker.IsLeftTouching() ? 
            transform.right * PlayerConstants.instance.postWallJumpSpeedModifier : 
            -transform.right * PlayerConstants.instance.postWallJumpSpeedModifier)) * PlayerConstants.instance.initialJumpForce, ForceMode2D.Impulse);
        CheckFlipWhenWallJump();
        StartCoroutine(ReverseAfterWallJumpAfterSeconds(0.15f));
    }

    private void DoubleJump()
    {
        isWallJumping = false; //reset the flag so wall jumping penalty for KeyHoldAscendWhileJumping does not apply.
        isDoubleJumping = true;
        MidAirJumpVerticalSpeedReset();
        jumpTime = 0;
        rigidBody2D.AddForce(transform.up * PlayerConstants.instance.initialJumpForce, ForceMode2D.Impulse);
        animator.SetTrigger(doubleJumpTrigger);
    }

    private void KeyHoldAscendWhileJumping()
    {
        jumpTime += Time.fixedDeltaTime + Time.fixedDeltaTime * Convert.ToInt32(isWallJumping) * PlayerConstants.instance.wallJumpPersistenceModifier + Time.fixedDeltaTime * Convert.ToInt32(isDoubleJumping) * PlayerConstants.instance.doubleJumpPersistenceModifier;
        velocityVector.Set(rigidBody2D.velocity.x, PlayerConstants.instance.jumpVerticalSpeed);
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
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    private void ResetPosition(Vector3 position)
    {
        velocityVector = new Vector2();
        gameObject.transform.position = position;
    }
    private void Knockback()
    {
        if (hurtTime > 0)
        {
            hurtTime -= Time.fixedDeltaTime;
        }
        if (currentKnockbackTime > 0)
        {
            isGettingKnockedBack = true;
            currentKnockbackTime -= Time.fixedDeltaTime;
        }
        else if (isGettingKnockedBack)
        {
            isGettingKnockedBack = false;
            playerInputManager.SetInputEnabled(true);
        }

        if (isGettingKnockedBack)
        {
            velocityVector.Set(knockbackStrength * currentKnockbackTime, knockbackStrength * currentKnockbackTime);
            rigidBody2D.AddForce(velocityVector, ForceMode2D.Impulse);
        }
    }
    public void Dash(bool dash, float move)
    {
        if (currentDashCooldown <= 0 && canDash && dash && stats.GetAbilityFlag(AbilityName.Dash) && UseEffort(1)) //use effort for testing only
        {
            SetVariablesWhenDashing(move);
            blockInputCoroutine = BlockInputForSeconds(0.25f);
            dashVariableReverserCoroutine = ReverseDashVariablesAfterSeconds(0.25f);
            StartCoroutine(blockInputCoroutine);
            StartCoroutine(dashVariableReverserCoroutine);
        }
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
        if (isWallTouching && isDashing)
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
            (jumpTime <= 0f || jumpTime > PlayerConstants.instance.minJumpTimeBeforeWallSlidingEnabled) &&
            stats.GetAbilityFlag(AbilityName.WallJump);
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

    internal void SetVariablesOnLoad(ref PlayerAbilityAndStats stats)
    {
        this.stats = stats;
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
        currentCoyoteTime = PlayerConstants.instance.coyoteTime;
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
        currentDashCooldown = PlayerConstants.instance.dashCooldown;
        isDashing = true;
        tempValue = rigidBody2D.gravityScale;
        rigidBody2D.gravityScale = 0f;
        velocityVector.Set(
            PlayerConstants.instance.dashSpeed * ((Convert.ToInt32(move == 0 ? !facingLeft : move > 0) << 1) - 1),
            0f);
        rigidBody2D.velocity = velocityVector;
        animator.SetBool("dashing", true);
    }

    private void KeepVelocityYAtZeroWhenDashing()
    {
        if(isDashing && !isOnSlope)
        {
            velocityVector.Set(rigidBody2D.velocity.x, 0);
            rigidBody2D.velocity = velocityVector;
        }
    }

    private void EndDash()
    {
        isDashing = false;
        rigidBody2D.gravityScale = tempValue;
        animator.SetBool("dashing", false);
        canDash = isGrounded || isWallTouching;
        jumpTime = 0f; // set so you can double jump after a dash
    }

    private bool UseEffort(int cost)
    {
        return effort.UseEffort(cost);
    }

    #endregion SETTING-VARIABLES
}
