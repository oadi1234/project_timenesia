using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController: MonoBehaviour
{
    //jump force applied while key is being held
    [SerializeField] 
    private float continousJumpForce = 0.9f;

    [SerializeField]
    private float initialJumpForce = 12f;

    [SerializeField]
    private float doubleJumpForce = 3f;

    [SerializeField]
    private float maxFallingSpeed = 175f;

    [Range(0f, 200f)]
    [SerializeField]
    private float wallJumpForce = 50f;

    [Range(0, 1f)] 
    [SerializeField] 
    private float airControl = 0.5f;  //might make it 1 at default if it proves to be a poorly feeling design choice. Could leave it for fun for future.

    [Range(0, 0.8f)]
    [SerializeField]
    private float slideSpeed = 0.8f;

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

    [Range(0, 0.2f)]
    private float ofWallJumpTimeLeft = 0f;

    [SerializeField]
    private float moveSpeed = 350f;


    private bool isGliding;
    private bool usedDoubleJump;

    private bool hasDoubleJumpCollected = false;
    private bool hasGlidingCollected = false;
    private bool hasSpatialDash = false;
    private bool facingLeft = false;
    private bool isGettingKnockedBack = false; 
    private bool afterWallJumpForceFinished = true;
    

    private bool isGrounded;
    private bool isJumping;
    private bool isWallSliding;
    private bool isWallTouching;
    private bool isOnSlope;
    private float slopeDownAngle;
    private float currentCoyoteTime;
    private float currentKnockbackTime;
    private float knockbackStrength;

    private const float groundedCheckRay = 0.1f;
    private const float ceilingCheckRadius = 0.1f;
    private const float wallCheckRay = 0.2f;
    private const float coyoteTime = 0.1f;
    private const float knockbackTime = 0.5f;

    private float hurtTime;
    private float currentMoveSpeed;
    private Rigidbody2D rigidBody2D;
    private Vector2 velocityVector;
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
    }

    private void FixedUpdate()
    {
        flatGroundChecker.CalculateRays();
        IsGrounded();
        wallChecker.CalculateRays(facingLeft, true);
        IsTouchingWall();

        isOnSlope = flatGroundChecker.IsOnSlope();
        slopeDownAngle = flatGroundChecker.GetSlopeAngle();

        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("Hurt", hurtTime);
        Knockback();
    }

    private void Initialize()
    {
        ResetPosition();
    }
    private void Restart()
    {
        playerHealth.Restart();
        StartCoroutine(BlockInputForSeconds(2f));
        Initialize();
    }

    private void IsGrounded()
    {
        if(flatGroundChecker.IsGrounded())
        {
            isGrounded = true;
            isWallTouching = false;
            isWallSliding = false;
            usedDoubleJump = false;
            isJumping = false;
            jumpTime = 0f;
            currentMoveSpeed = moveSpeed;
            currentCoyoteTime = coyoteTime;
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
        if (!isGrounded)
        {
            isWallTouching = wallChecker.IsTouchingWall();  //if this is needed even when onGround, then it have to be moved outside of if statement
            if (isWallTouching && afterWallJumpForceFinished)
            {
                isWallSliding = facingLeft ? Input.GetKey(KeyCode.LeftArrow) : Input.GetKey(KeyCode.RightArrow);
            }
            else
                isWallSliding = false;

            if (isWallSliding)
            {
                isJumping = false;
                jumpTime = 0f;
                //currentMoveSpeed = 0;
                currentCoyoteTime = coyoteTime;
            }
            //Debug.Log("WallSlide: " + isWallSliding);
            //Debug.Log("WallSlide: " + isWallTouching);
        }
    }

    public void Move(float move)
    {
        if (!isGrounded)
        {
            if (isWallSliding)
                velocityVector.Set(move * currentMoveSpeed, -3f);
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
            if (speed > maxFallingSpeed)
            {
                float brakeSpeed = speed - maxFallingSpeed;
                Vector2 normalisedVelocity = rigidBody2D.velocity.normalized;
                Vector2 brakeVelocity = normalisedVelocity * brakeSpeed;
                //rigidBody2D.AddForce(-brakeVelocity);
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

    private void CheckFlipWhenWallJump()
    {
        if ((facingLeft && wallChecker.IsLeftTouching()) || (!facingLeft && wallChecker.IsRightTouching()))
            Flip();
    }

    public void Jump(bool jump, bool keyHeld)
    {
        Debug.Log(keyHeld);
        if (jump && !isGrounded && isWallTouching)
        {
            rigidBody2D.velocity = new Vector2(0, 0);
            isJumping = true;
            isWallSliding = false;
            jumpTime = 0f;
            rigidBody2D.AddForce((transform.up + (wallChecker.IsLeftTouching() ? transform.right/1.2f : -transform.right/1.2f)) * initialJumpForce, ForceMode2D.Impulse);
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
            rigidBody2D.AddForce(transform.up * initialJumpForce, ForceMode2D.Impulse);
        }

        //Debug.Log(rigidBody2D.velocity);

        //if (ofWallJumpTimeLeft > 0f)
        //{
        //    rigidBody2D.AddForce(new Vector2(continousJumpForce * (ofWallJumpTimeLeft * 4), 0), ForceMode2D.Impulse);
        //    ofWallJumpTimeLeft -= Time.fixedDeltaTime;
        //    Debug.Log("JumpTimeLeft: " + ofWallJumpTimeLeft);
        //}

        if (isJumping && keyHeld && rigidBody2D.velocity.y > 0)
        {
            rigidBody2D.AddForce(new Vector2(0f, continousJumpForce * (1 - jumpTime * 4)), ForceMode2D.Impulse);
            jumpTime += Time.fixedDeltaTime;
            //Debug.Log("JumpTimeLeft: " + jumpTime);
        }
        if (isJumping && afterWallJumpForceFinished && !keyHeld && rigidBody2D.velocity.y > 0)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);
        }
        animator.SetFloat("VerticalSpeed", rigidBody2D.velocity.y);
    }

    public void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Hurt damageSource = collider.gameObject.GetComponent<Hurt>();
        
        if(damageSource)
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

    private IEnumerator BlockInputForSeconds(float seconds)
    {
        Debug.Log("WAITNG");
        playerMovement.SetInputEnabled(false);
        yield return new WaitForSecondsRealtime(seconds);
        Debug.Log("WAITED");
        playerMovement.SetInputEnabled(true);
    }

    private IEnumerator ReverseAfterWallJumpAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        afterWallJumpForceFinished = true;
    }
    private void ResetPosition()
    {
        velocityVector = new Vector2();
        gameObject.transform.position = new Vector3(19.5f, 1f, 0);
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
            playerMovement.SetInputEnabled(true);
        }

        if (isGettingKnockedBack)
        {
            velocityVector.Set(knockbackStrength * currentKnockbackTime, knockbackStrength * currentKnockbackTime);
            rigidBody2D.AddForce(velocityVector, ForceMode2D.Impulse);
        }
    }
}
