using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController: MonoBehaviour
{
    //jump force applied while key is being held
    [SerializeField] 
    private float _continousJumpForce = 0.9f;

    [SerializeField]
    private float _initialJumpForce = 12f;

    [SerializeField]
    private float _doubleJumpForce = 3f;

    [SerializeField]
    private float _wallJumpForce = 5f;

    [SerializeField]
    private float maxFallingSpeed = 175f;

    [Range(0f, 200f)]
    [SerializeField]
    private float wallJumpForce = 50f;

    [Range(0, 1f)] 
    [SerializeField] 
    private float airControl = 0.5f;  //might make it 1 at default if it proves to be a poorly feeling design choice. Could leave it for fun for future.

    [Range(-8f, 0f)]
    [SerializeField]
    private float slideSpeed = -4.5f;

    [Range(0, 0.3f)]
    [SerializeField]
    private float crouchSpeed = 0.3f;

    [Range(0, 1f)]
    [SerializeField]
    private float glideSpeed = 0f;

    [SerializeField]
    private int spatialDashCooldown = 99;

    [SerializeField]
    private Transform wallCheckMarker;

    [SerializeField]
    private PhysicsMaterial2D noFriction;

    [SerializeField]
    private PhysicsMaterial2D allFriction;

    [Range(0, 0.25f)]
    private float jumpTime = 0f;

    [SerializeField]
    private float moveSpeed = 350f;


    private bool isGliding;
    private bool usedDoubleJump;
    private bool hasDoubleJumpCollected = false;
    private bool hasGlidingCollected = false;
    private bool hasSpatialDash = false;
    private float slopeDownAngle;
    private const float groundedCheckRay = 0.1f;
    private const float ceilingCheckRadius = 0.1f;
    private const float wallCheckRay = 0.2f;


    private bool facingLeft = false;
    private bool isGettingKnockedBack = false; 
    private bool afterWallJumpForceFinished = true;
    private bool isGrounded;
    private bool isJumping;
    private bool isWallSliding;
    private bool _isWallTouching;
    private bool isOnSlope;

    private float currentCoyoteTime;
    private float currentKnockbackTime;
    private float knockbackStrength;
    private float flipCooldown;

    private const float coyoteTime = 0.1f;
    private const float knockbackTime = 0.5f;
    private const float minJumpTimeBeforeWallSlidingEnabled = 0.15f;    

    private float hurtTime;
    private float currentMoveSpeed;

    private Rigidbody2D rigidBody2D;
    private Vector2 velocityVector;
    private Vector3 _startingPosition;
    private FlatGroundChecker flatGroundChecker;
    private WallChecker wallChecker;
    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;


    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        wallChecker = GetComponent<WallChecker>();
        flatGroundChecker = GetComponent<FlatGroundChecker>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        velocityVector = new Vector2();
        currentMoveSpeed = moveSpeed;
        currentCoyoteTime = coyoteTime;
        currentKnockbackTime = knockbackTime;
        hurtTime = 0f;
        _startingPosition = rigidBody2D.position;
    }

    private void FixedUpdate()
    {
        IsGrounded();
        IsTouchingWall();
        IsOnSlope();

        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("Hurt", hurtTime);

        Knockback();
        if(flipCooldown>0)
        {
            flipCooldown -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Hurt damageSource = collider.gameObject.GetComponent<Hurt>();

        if (damageSource)
        {
            playerHealth.TakeDamage(damageSource.damageDealt, damageSource.iFramesGiven);
            if (playerHealth.currentHealth > 0)
            {
                hurtTime = 0.5f;
                currentKnockbackTime = knockbackTime;
                knockbackStrength = damageSource.knockbackStrength;
                playerMovement.SetInputEnabled(false);
            }
            else
            {
                Restart();
            }
        }
    }

    private void Initialize()
    {
        ResetPosition(_startingPosition);
    }

    private void Restart()
    {
        playerHealth.Restart();
        StartCoroutine(BlockInputForSeconds(2f));
        Initialize();
    }

    #region Movement
    public void Move(float move)
    {
        if (!isGrounded)
        {
            if (isWallSliding && rigidBody2D.velocity.y < 0)
                velocityVector.Set(move * currentMoveSpeed, slideSpeed);
            //    //else /*if (afterWallJumpForceFinished)*/
            else if (afterWallJumpForceFinished)
                velocityVector.Set(move * currentMoveSpeed, rigidBody2D.velocity.y);
            else
                velocityVector.Set(rigidBody2D.velocity.x, rigidBody2D.velocity.y);
        }
        else if (isGrounded && !isOnSlope)
        {
            velocityVector.Set(move * currentMoveSpeed, 0.0f);
        }
        else if (isOnSlope)
        {
            velocityVector.Set(
                -move * currentMoveSpeed * flatGroundChecker.slopeNormalPerpendicular.x,
                moveSpeed * flatGroundChecker.slopeNormalPerpendicular.y * -move);
        }

        rigidBody2D.velocity = velocityVector;
        if (/*afterWallJumpForceFinished*/true)
        {
            float speed = Vector2.SqrMagnitude(rigidBody2D.velocity);
            if (move > 0 && facingLeft)
            {
                Flip();
                currentMoveSpeed *= airControl;
            }
            else if (move < 0 && !facingLeft)
            {
                Flip();
                currentMoveSpeed *= airControl;
            }
            //TODO: change to speed in y axis, not speed of whole rigidBody
            if (speed > maxFallingSpeed)
            {
                float brakeSpeed = speed - maxFallingSpeed;
                Vector2 normalisedVelocity = rigidBody2D.velocity.normalized;
                Vector2 brakeVelocity = normalisedVelocity * brakeSpeed;
                rigidBody2D.AddForce(-brakeVelocity);
            }

            if (isOnSlope && move == 0f)
            {
                rigidBody2D.sharedMaterial = allFriction;
            }
            else
            {
                rigidBody2D.sharedMaterial = noFriction;
            }
        }

        animator.SetFloat("Speed", Mathf.Abs(move));

        //Debug.Log("Velo: " + rigidBody2D.velocity);

    }
    public void Jump(bool jump, bool keyHeld)
    {
        if (jump && !isGrounded && _isWallTouching)
        {
            flipCooldown = 0f;
            rigidBody2D.velocity = new Vector2(0, 0);
            isJumping = true;
            isWallSliding = false;
            jumpTime = 0f;
            rigidBody2D.AddForce((transform.up + (wallChecker.IsLeftTouching() ? transform.right/1.2f : -transform.right/1.2f)) * _wallJumpForce, ForceMode2D.Impulse);
            CheckFlipWhenWallJump();
            afterWallJumpForceFinished = false; 
            currentMoveSpeed = moveSpeed;

            StartCoroutine(ReverseAfterWallJumpAfterSeconds(0.15f));
        }

        if (jump && isGrounded)
        {
            isJumping = true;
            isGrounded = false;
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
            rigidBody2D.AddForce(transform.up * _initialJumpForce, ForceMode2D.Impulse);
        }

        //if (ofWallJumpTimeLeft > 0f)
        //{
        //    rigidBody2D.AddForce(new Vector2(_continousJumpForce * (ofWallJumpTimeLeft * 4), 0), ForceMode2D.Impulse);
        //    ofWallJumpTimeLeft -= Time.fixedDeltaTime;
        //    Debug.Log("JumpTimeLeft: " + ofWallJumpTimeLeft);
        //}

        if (isJumping && keyHeld && rigidBody2D.velocity.y > 0)
        {
            rigidBody2D.AddForce(new Vector2(0f, _continousJumpForce * (1 - jumpTime * 4)), ForceMode2D.Impulse);
            jumpTime += Time.fixedDeltaTime;
            //Debug.Log("JumpTimeLeft: " + jumpTime);
        }
        if (isJumping && afterWallJumpForceFinished && !keyHeld && rigidBody2D.velocity.y > 0)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
        }
        if (isJumping && rigidBody2D.velocity.y <= 0)
        {
            jumpTime = -1;
        }
        animator.SetFloat("VerticalSpeed", rigidBody2D.velocity.y);
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
    #endregion Movement

    #region Waiters
    private IEnumerator BlockInputForSeconds(float seconds)
    {
        playerMovement.SetInputEnabled(false);
        yield return new WaitForSecondsRealtime(seconds);
        playerMovement.SetInputEnabled(true);
    }

    private IEnumerator ReverseAfterWallJumpAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        afterWallJumpForceFinished = true;
    }
    #endregion Waiters

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
            playerMovement.SetInputEnabled(true);
        }

        if (isGettingKnockedBack)
        {
            velocityVector.Set(knockbackStrength * currentKnockbackTime, knockbackStrength * currentKnockbackTime);
            rigidBody2D.AddForce(velocityVector, ForceMode2D.Impulse);
        }
    }

    #region Checkers
    private void IsOnSlope()
    {
        isOnSlope = flatGroundChecker.IsOnSlope();
        slopeDownAngle = flatGroundChecker.GetSlopeAngle();
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

        if (!isGrounded)
        {
            _isWallTouching = wallChecker.IsTouchingWall();  //if this is needed even when onGround, then it have to be moved outside of if statement

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

    private bool CanWallSlide()
    {
        return _isWallTouching && afterWallJumpForceFinished && (jumpTime <= 0f || jumpTime > minJumpTimeBeforeWallSlidingEnabled);
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
    private void SetVariablesWhenGrounded()
    {
        isGrounded = true;
        _isWallTouching = false;
        isWallSliding = false;
        usedDoubleJump = false;
        isJumping = false;
        jumpTime = 0f;
        currentMoveSpeed = moveSpeed;
        currentCoyoteTime = coyoteTime;
    }
    private void SetVariablesWhenWallSliding()
    {
        isJumping = false;
        jumpTime = 0f;
        //currentMoveSpeed = 0;
        currentCoyoteTime = coyoteTime;
    }



    #endregion SETTING-VARIABLES
}
