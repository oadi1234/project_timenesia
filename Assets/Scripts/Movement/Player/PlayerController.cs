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
    private float _maxFallingSpeed = 175f;

    [Range(0f, 200f)]
    [SerializeField]
    private float _wallJumpForce = 50f;

    [Range(0, 1f)] 
    [SerializeField] 
    private float _airControl = 1f;  //might make it 1 at default if it proves to be a poorly feeling design choice. Could leave it for fun for future.

    [Range(-8f, 0f)]
    [SerializeField]
    private float _slideSpeed = -4.5f;

    [Range(0, 0.3f)]
    [SerializeField]
    private float _crouchSpeed = 0.3f;

    [Range(0, 10f)]
    [SerializeField]
    private float _glideSpeed = 5f;

    [SerializeField]
    private int _spatialDashCooldown = 99;

    [SerializeField]
    private Transform _wallCheckMarker;

    [SerializeField]
    private PhysicsMaterial2D _noFriction;

    [SerializeField]
    private PhysicsMaterial2D _allFriction;

    [Range(0, 0.25f)]
    private float _jumpTime = 0f;

    [SerializeField]
    private float _moveSpeed = 350f;


    private bool _isGliding;
    private bool _usedDoubleJump;
    private bool _hasDoubleJumpCollected = false;
    private bool _hasGlidingCollected = false;
    private bool _hasSpatialDash = false;
    private float _slopeDownAngle;
    private const float _groundedCheckRay = 0.1f;
    private const float _ceilingCheckRadius = 0.1f;
    private const float _wallCheckRay = 0.2f;


    private bool _facingLeft = false;
    private bool _isGettingKnockedBack = false; 
    private bool _afterWallJumpForceFinished = true;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isWallSliding;
    private bool _isWallTouching;
    private bool _isOnSlope;

    private float _currentCoyoteTime;
    private float _currentKnockbackTime;
    private float _knockbackStrength;
    private float _flipCooldown;

    private const float _coyoteTime = 0.1f;
    private const float _knockbackTime = 0.5f;
    private const float _minJumpTimeBeforeWallSlidingEnabled = 0.15f;    

    private float _hurtTime;
    private float _currentMoveSpeed;

    private Rigidbody2D _rigidBody2D;
    private Vector2 _velocityVector;
    private Vector3 _startingPosition;
    private FlatGroundChecker _flatGroundChecker;
    private WallChecker _wallChecker;
    private Animator _animator;
    private PlayerMovement _playerMovement;
    private PlayerHealth _playerHealth;


    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _wallChecker = GetComponent<WallChecker>();
        _flatGroundChecker = GetComponent<FlatGroundChecker>();
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerHealth = GetComponent<PlayerHealth>();
        _velocityVector = new Vector2();
        _currentMoveSpeed = _moveSpeed;
        _currentCoyoteTime = _coyoteTime;
        _currentKnockbackTime = _knockbackTime;
        _hurtTime = 0f;
        _startingPosition = _rigidBody2D.position;
    }

    private void FixedUpdate()
    {
        IsGrounded();
        IsTouchingWall();
        IsOnSlope();

        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetFloat("Hurt", _hurtTime);

        Knockback();
        if(_flipCooldown>0)
        {
            _flipCooldown -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Hurt damageSource = collider.gameObject.GetComponent<Hurt>();

        if (damageSource)
        {
            _playerHealth.TakeDamage(damageSource.damageDealt, damageSource.iFramesGiven);
            if (_playerHealth.currentHealth > 0)
            {
                _hurtTime = 0.5f;
                _currentKnockbackTime = _knockbackTime;
                _knockbackStrength = damageSource.knockbackStrength;
                _playerMovement.SetInputEnabled(false);
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
        _playerHealth.Restart();
        StartCoroutine(BlockInputForSeconds(2f));
        Initialize();
    }

    #region Movement
    public void Move(float move)
    {
        if (!_isGrounded)
        {
            if (_isWallSliding)
                _velocityVector.Set(move * _currentMoveSpeed, _slideSpeed);

            else if (_afterWallJumpForceFinished)   //if jumped of the wall recently
                _velocityVector.Set(move * _currentMoveSpeed, _rigidBody2D.velocity.y);

            else
                _velocityVector.Set(_rigidBody2D.velocity.x, _rigidBody2D.velocity.y);
        }
        else if (_isGrounded && !_isOnSlope)
        {
            _velocityVector.Set(move * _currentMoveSpeed, 0.0f);
        }
        else if (_isOnSlope)
        {
            _velocityVector.Set(
                -move * _currentMoveSpeed * _flatGroundChecker.slopeNormalPerpendicular.x,
                _moveSpeed * _flatGroundChecker.slopeNormalPerpendicular.y * -move);
        }

        _rigidBody2D.velocity = _velocityVector;

        if (/*_afterWallJumpForceFinished*/true) //no idea why _afterWallJumpForceFinished was here, currently leaving this as it is, maybe will delete if later
        {
            float speed = Vector2.SqrMagnitude(_rigidBody2D.velocity);
            if (move > 0 && _facingLeft)
            {
                Flip();
                _currentMoveSpeed *= _airControl;
            }
            else if (move < 0 && !_facingLeft)
            {
                Flip();
                _currentMoveSpeed *= _airControl;
            }

            //TODO: change to speed in y axis, not speed of whole rigidBody
            if (speed > _maxFallingSpeed)
            {
                float brakeSpeed = speed - _maxFallingSpeed;
                Vector2 normalisedVelocity = _rigidBody2D.velocity.normalized;
                Vector2 brakeVelocity = normalisedVelocity * brakeSpeed;
                _rigidBody2D.AddForce(-brakeVelocity);
            }

            if (_isOnSlope && move == 0f)
            {
                _rigidBody2D.sharedMaterial = _allFriction;
            }
            else
            {
                _rigidBody2D.sharedMaterial = _noFriction;
            }
        }

        _animator.SetFloat("Speed", Mathf.Abs(move));
    }
    public void Jump(bool jump, bool keyHeld)
    {
        if (jump && !_isGrounded && _isWallTouching)    //of the wall jumping
        {
            SetVariablesWhenWallJumping();
            _rigidBody2D.AddForce((transform.up + (_wallChecker.IsLeftTouching() ? transform.right/1.2f : -transform.right/1.2f)) * _initialJumpForce, ForceMode2D.Impulse);
            CheckFlipWhenWallJump();
            StartCoroutine(ReverseAfterWallJumpAfterSeconds(0.15f));
        }
        else if (jump && _isGrounded)    //classic jumping from the ground
        {
            _isJumping = true;
            _isGrounded = false;
            _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, 0);

            _rigidBody2D.AddForce(transform.up * _initialJumpForce, ForceMode2D.Impulse);
        }
        else if (_isJumping && keyHeld && _rigidBody2D.velocity.y > 0)    //longer jumping when holding key; velo > 0 to ommit falling
        {
            _rigidBody2D.AddForce(new Vector2(0f, _continousJumpForce * (1 - _jumpTime * 4)), ForceMode2D.Impulse);
            _jumpTime += Time.fixedDeltaTime;
        }
        else if (_isJumping && _afterWallJumpForceFinished && !keyHeld && _rigidBody2D.velocity.y > 0)
        {
            _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, 0);
        }
        else if (_isJumping && _rigidBody2D.velocity.y <= 0) //falling
        {
            _jumpTime = -1;
        }

        //no idea what it is
        //if (ofWallJumpTimeLeft > 0f)
        //{
        //    rigidBody2D.AddForce(new Vector2(_continousJumpForce * (ofWallJumpTimeLeft * 4), 0), ForceMode2D.Impulse);
        //    ofWallJumpTimeLeft -= Time.fixedDeltaTime;
        //    Debug.Log("JumpTimeLeft: " + ofWallJumpTimeLeft);
        //}
        _animator.SetFloat("VerticalSpeed", _rigidBody2D.velocity.y);
    }
    public void Flip()
    {
        if (_flipCooldown <= 0)
        {
            _facingLeft = !_facingLeft;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    private void ResetPosition(Vector3 position)
    {
        _velocityVector = new Vector2();
        gameObject.transform.position = position;
    }
    private void Knockback()
    {
        if (_hurtTime > 0)
        {
            _hurtTime -= Time.fixedDeltaTime;
        }
        if (_currentKnockbackTime > 0)
        {
            _isGettingKnockedBack = true;
            _currentKnockbackTime -= Time.fixedDeltaTime;
        }
        else if (_isGettingKnockedBack)
        {
            _isGettingKnockedBack = false;
            _playerMovement.SetInputEnabled(true);
        }

        if (_isGettingKnockedBack)
        {
            _velocityVector.Set(_knockbackStrength * _currentKnockbackTime, _knockbackStrength * _currentKnockbackTime);
            _rigidBody2D.AddForce(_velocityVector, ForceMode2D.Impulse);
        }
    }

    #endregion Movement

    #region Waiters
    private IEnumerator BlockInputForSeconds(float seconds)
    {
        _playerMovement.SetInputEnabled(false);
        yield return new WaitForSecondsRealtime(seconds);
        _playerMovement.SetInputEnabled(true);
    }

    private IEnumerator ReverseAfterWallJumpAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        _afterWallJumpForceFinished = true;
    }
    #endregion Waiters

    #region Checkers
    private void IsOnSlope()
    {
        _isOnSlope = _flatGroundChecker.IsOnSlope();
        _slopeDownAngle = _flatGroundChecker.GetSlopeAngle();
    }

    private void IsGrounded()
    {
        _flatGroundChecker.CalculateRays();

        if (_flatGroundChecker.IsGrounded())
        {
            SetVariablesWhenGrounded();
        }
        else if (_currentCoyoteTime > 0)
        {
            _currentCoyoteTime -= Time.fixedDeltaTime;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void IsTouchingWall()
    {
        _wallChecker.CalculateRays(_facingLeft, true);

        if (!_isGrounded)
        {
            _isWallTouching = _wallChecker.IsTouchingWall();  //if this is needed even when onGround, then it have to be moved outside of if statement

            if (CanWallSlide())
            {
                _isWallSliding = _facingLeft ? Input.GetKey(KeyCode.LeftArrow) : Input.GetKey(KeyCode.RightArrow);
            }
            else
            {
                _isWallSliding = false;
            }

            if (_isWallSliding)
            {
                SetVariablesWhenWallSliding();
            }
        }
    }

    private bool CanWallSlide()
    {
        return _isWallTouching && _afterWallJumpForceFinished && (_jumpTime <= 0f || _jumpTime > _minJumpTimeBeforeWallSlidingEnabled);
    }

    private void CheckFlipWhenWallJump()
    {
        if ((_facingLeft && _wallChecker.IsLeftTouching()) || (!_facingLeft && _wallChecker.IsRightTouching()))
        {
            Flip();
            _flipCooldown = 0.2f;
        }
    }
    #endregion Checkers

    #region SETTING-VARIABLES
    private void SetVariablesWhenGrounded()
    {
        _isGrounded = true;
        _isWallTouching = false;
        _isWallSliding = false;
        _usedDoubleJump = false;
        _isJumping = false;
        _jumpTime = 0f;
        _currentMoveSpeed = _moveSpeed;
        _currentCoyoteTime = _coyoteTime;
    }
    private void SetVariablesWhenWallSliding()
    {
        _isJumping = false;
        _jumpTime = 0f;
        //currentMoveSpeed = 0;
        _currentCoyoteTime = _coyoteTime;
    }
    private void SetVariablesWhenWallJumping()
    {
        _rigidBody2D.velocity = new Vector2(0, 0);
        _isJumping = true;
        _isWallSliding = false;
        _jumpTime = 0f;
        _afterWallJumpForceFinished = false;
        _currentMoveSpeed = _moveSpeed;
    }
    #endregion SETTING-VARIABLES
}
