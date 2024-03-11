using System;
using System.Collections;
using _2___Scripts.Global;
using _2___Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController: MonoBehaviour
{
    public Spellbook _spellbook;
    #region variables

    [SerializeField]
    private float _initialJumpForce = 10f;

    [SerializeField]
    private float _doubleJumpForce = 3f;

    [SerializeField]
    private float _maxVerticalSpeed = 15f;

    [SerializeField]
    private float _maxHorizontalSpeed = 15f;

    [Range(0f, 200f)]
    [SerializeField]
    private float _wallJumpForce = 50f;

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
    private float _dashCooldown = 0.6f;

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

    [SerializeField]
    private float _wallJumpPersistenceModifier = 1.5f; //increases time counted after jumping, so holding the key is a bit shorter. To count by how much it decreases do 1/(this variable)

    [SerializeField]
    private float _postWallJumpSpeedModifier = 1.1f;

    [SerializeField]
    private float _doubleJumpPersistenceModifier = 2f;


    private bool _isGliding;
    private bool _isDoubleJumping;
    private bool _canDash; //only one dash mid-air.
    private bool _hasDoubleJumpCollected = false;
    private bool _hasGlidingCollected = false;
    private bool _hasSpatialDash = false;
    private float _slopeDownAngle;
    private const float _groundedCheckRay = 0.1f;
    private const float _ceilingCheckRadius = 0.1f;
    private const float _wallCheckRay = 0.2f;
    private const float _maxJumpTime = 0.11f;
    private const float _jumpVerticalSpeed = 22f;


    private bool _facingLeft = false;
    private bool _isGettingKnockedBack = false;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isWallJumping = false;
    private bool _isWallSliding;
    private bool _isWallTouching;
    private bool _isOnSlope;
    private bool _isDashing;

    private float _currentCoyoteTime;
    private float _timeAfterJumpPressed; //cooldown for setting variables after pressing jump.
    private float _currentKnockbackTime;
    private float _knockbackStrength;
    private float _flipCooldown;
    private float _hurtTime;
    private float _currentMoveSpeed;
    private float _currentDashCooldown;

    private const float _coyoteTime = 0.1f;
    private const float _knockbackTime = 0.5f;
    private const float _jumpGroundCheckCooldown = 0.1f;
    private const float _minJumpTimeBeforeWallSlidingEnabled = 0.15f;



    private Rigidbody2D _rigidBody2D;
    private Vector2 _velocityVector;
    private Vector2 _brakeVector = Vector2.zero;
    private Vector3 _startingPosition;
    private FlatGroundChecker _flatGroundChecker;
    private WallChecker _wallChecker;
    private Animator _animator;
    private PlayerMovement _playerMovement;
    private PlayerHealth _playerHealth;
    private IEnumerator blockInputCoroutine;
    private IEnumerator dashVariableReverserCoroutine;
    private static readonly int AttackBoltTrigger = Animator.StringToHash("AttackBoltTrigger");
    private static readonly int AttackSelfAoETrigger = Animator.StringToHash("AttackSelfAoETrigger");
    private static readonly int doubleJumpTrigger = Animator.StringToHash("doubleJump");
    private static readonly int groundJumpTrigger = Animator.StringToHash("groundJump");
    private static readonly int wallJumpTrigger = Animator.StringToHash("wallJump");

    #endregion

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _wallChecker = GetComponent<WallChecker>();
        _flatGroundChecker = GetComponent<FlatGroundChecker>();
        _animator = GetComponentInChildren<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerHealth = GetComponent<PlayerHealth>();
        _velocityVector = new Vector2();
        _currentMoveSpeed = _moveSpeed;
        _currentCoyoteTime = _coyoteTime;
        _currentKnockbackTime = _knockbackTime;
        _hurtTime = 0f;
        _startingPosition = _rigidBody2D.position;
        GameDataManager.Instance.LoadFromSave(new SaveDataSchema{Coins = 10, CurrentHealth = 2, MaxHealth = 4, 
            MaxMana = 8, MaxConcentrationSlots = 2, SavePoint = "lol"});
    }

    private void FixedUpdate()
    {
        IsGrounded();
        IsTouchingWall();
        IsOnSlope();
        CheckIfShouldStopDash();

        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetBool("wallSliding", _isWallSliding);
        // _animator.SetFloat("Hurt", _hurtTime);

        Knockback();

        if (_timeAfterJumpPressed > 0)
        {
            _timeAfterJumpPressed -= Time.fixedDeltaTime;
        }
        if (_flipCooldown>0)
        {
            _flipCooldown -= Time.fixedDeltaTime;
        }
        if (_currentDashCooldown > 0)
        {
            _currentDashCooldown -= Time.fixedDeltaTime;
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

    // private void OnTriggerEnter2D(Collider2D collider)
    // {
    //     Hurt damageSource = collider.gameObject.GetComponent<Hurt>();
    //
    //     if (damageSource)
    //     {
    //         _playerHealth.TakeDamage(damageSource.damageDealt, damageSource.iFramesGiven);
    //         if (_playerHealth.currentHealth > 0)
    //         {
    //             _hurtTime = 0.5f;
    //             _currentKnockbackTime = _knockbackTime;
    //             _knockbackStrength = damageSource.knockbackStrength;
    //             _playerMovement.SetInputEnabled(!damageSource.blocksMovement);
    //         }
    //         else
    //         {
    //             Restart();
    //         }
    //     }
    // }

    #region Movement
    public void Move(float move)
    {
        if (!_isGrounded)
        {
            if (_isWallSliding)
                _velocityVector.Set(move * _currentMoveSpeed, _slideSpeed);

            else if (_isWallJumping && _facingLeft)   //if jumped of the wall recently {
            {
                _velocityVector.Set(Mathf.Min(move * _currentMoveSpeed, _rigidBody2D.velocity.x), _rigidBody2D.velocity.y);
            }
            else if (_isWallJumping && !_facingLeft)
            {
                _velocityVector.Set(Mathf.Max(move * _currentMoveSpeed, _rigidBody2D.velocity.x), _rigidBody2D.velocity.y);
            }
            else
                _velocityVector.Set(move * _currentMoveSpeed, _rigidBody2D.velocity.y);
        }
        else if (_isGrounded && !_isOnSlope)
        {
            _velocityVector.Set(move * _currentMoveSpeed, _rigidBody2D.velocity.y);
        }
        else if (_isOnSlope)
        {
            _velocityVector.Set(
                move * _currentMoveSpeed,
                _moveSpeed * _flatGroundChecker.slopeNormalPerpendicular.y * -move);
        }

        _rigidBody2D.velocity = _velocityVector;

        if (move > 0 && _facingLeft)
        {
            Flip();
        }
        else if (move < 0 && !_facingLeft)
        {
            Flip();
        }
        float brakeSpeedY = 0f;
        float brakeSpeedX = 0f;
        // Speed when falling/ascending. Most likely not ideal but it works.
        if (Math.Abs(_rigidBody2D.velocity.y) > _maxVerticalSpeed)
        {
            brakeSpeedY = Math.Abs(_rigidBody2D.velocity.y) - _maxVerticalSpeed;
        }
        if (Math.Abs(_rigidBody2D.velocity.x) > _maxHorizontalSpeed)
        {
            brakeSpeedX = Math.Abs(_rigidBody2D.velocity.x) - _maxHorizontalSpeed;

        }
        _brakeVector.Set(_rigidBody2D.velocity.x*brakeSpeedX, _rigidBody2D.velocity.y*brakeSpeedY);
        _rigidBody2D.AddForce(-_brakeVector);

        if (_isOnSlope && move == 0f)
        {
            _rigidBody2D.sharedMaterial = _allFriction;
        }
        else
        {
            _rigidBody2D.sharedMaterial = _noFriction;
        }

        _animator.SetInteger("speedX", Math.Sign(move));
    }
    public void Jump(bool jump, bool keyHeld)
    {
        if (jump && !_isGrounded && _isWallTouching)
        {
            WallJump();
        }
        else if (jump && _isGrounded)
        {
            GroundJump();
        }
        else if ((_isJumping || _isDoubleJumping) && keyHeld && _rigidBody2D.velocity.y > 0 && _jumpTime < _maxJumpTime)
        {
            KeyHoldAscendWhileJumping();
        }
        else if (_isJumping && !keyHeld && _rigidBody2D.velocity.y > 0) 
        {
            LoseVelocityAfterJumping();
        }
        else if (jump && !_isDoubleJumping)
        {
            DoubleJump();
        }

        _animator.SetFloat("speedY", _rigidBody2D.velocity.y); // TODO move animator events to its own class
    }

    private void GroundJump()
    {
        _timeAfterJumpPressed = _jumpGroundCheckCooldown; //a cooldown made to avoid setting values on being grounded a short while after jumping.
        _isJumping = true;
        _isGrounded = false;
        if (!_flatGroundChecker.IsGrounded()) //coyote time case, otherwise player just awkwardly floats for a second
        {
            MidAirJumpVerticalSpeedReset();
        }
        _rigidBody2D.AddForce(transform.up * _initialJumpForce, ForceMode2D.Impulse);
        _animator.SetTrigger(groundJumpTrigger);
    }

    private void WallJump()
    {
        SetVariablesWhenWallJumping();
        _rigidBody2D.AddForce((transform.up + (_wallChecker.IsLeftTouching() ? transform.right * _postWallJumpSpeedModifier : -transform.right * _postWallJumpSpeedModifier)) * _initialJumpForce, ForceMode2D.Impulse);
        CheckFlipWhenWallJump();
        StartCoroutine(ReverseAfterWallJumpAfterSeconds(0.15f));
    }

    private void DoubleJump()
    {
        _isWallJumping = false; //reset the flag so wall jumping penalty for KeyHoldAscendWhileJumping does not apply.
        _isDoubleJumping = true;
        MidAirJumpVerticalSpeedReset();
        _jumpTime = 0;
        _rigidBody2D.AddForce(transform.up * _initialJumpForce, ForceMode2D.Impulse);
        _animator.SetTrigger(doubleJumpTrigger);
    }

    private void KeyHoldAscendWhileJumping()
    {
        _jumpTime += Time.fixedDeltaTime + Time.fixedDeltaTime * Convert.ToInt32(_isWallJumping) * _wallJumpPersistenceModifier + Time.fixedDeltaTime * Convert.ToInt32(_isDoubleJumping) * _doubleJumpPersistenceModifier;
        _velocityVector.Set(_rigidBody2D.velocity.x, _jumpVerticalSpeed);
        _rigidBody2D.velocity = _velocityVector;
    }

    private void LoseVelocityAfterJumping()
    {
        // smooth ascend to 0 once key is no longer held. Allows for finer control without jerky movement.
        _velocityVector.Set(_rigidBody2D.velocity.x, _rigidBody2D.velocity.y * 0.8f);
        _rigidBody2D.velocity = _velocityVector;
        _jumpTime = 99f;
    }

    private void MidAirJumpVerticalSpeedReset()
    {
        _velocityVector.Set(_rigidBody2D.velocity.x, 0);
        _rigidBody2D.velocity = _velocityVector;
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
    public void Dash(bool dash, float move)
    {
        if (_currentDashCooldown <= 0 && _canDash && dash)
        {
            SetVariablesWhenDashing(move);
            blockInputCoroutine = BlockInputForSeconds(0.25f);
            dashVariableReverserCoroutine = ReverseDashVariablesAfterSeconds(0.25f);
            StartCoroutine(blockInputCoroutine);
            StartCoroutine(dashVariableReverserCoroutine);
        }
    }

    #endregion Movement

    #region Actions

    public void Attack(int spellIndex)
    {
        switch (spellIndex)
        {
            case 0:
                Attack_Bolt();
                break;
            case 1:
                Attack_SelfAoE();
                break;
        }
    }

    private void Attack_Bolt()
    {
        _animator.SetTrigger(AttackBoltTrigger);
    }

    private void Attack_SelfAoE()
    {
        _animator.SetTrigger(AttackSelfAoETrigger);
    }
    
    public void Test(int spellIndex)
    {
        switch (spellIndex)
        {
            case 0:
                _spellbook.CastFireBall(_facingLeft ? -1  : 1);
                break;
            case 1:
                _spellbook.CastEyeBall(_facingLeft ? -1  : 1);
                break;
        }
        _playerMovement.CastingAnimationFinished();
        _playerMovement.SetInputEnabled(true);
    }

    #endregion

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
        _isWallJumping = false;
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
    
    private void CheckIfShouldStopDash()
    {
        if (_isWallTouching && _isDashing)
        {
            StopCoroutine(blockInputCoroutine);
            StopCoroutine(dashVariableReverserCoroutine);
            EndDash();
            _playerMovement.SetInputEnabled(true);
           
        }
    }

    private bool CanWallSlide()
    {
        return _isWallTouching && !_isWallJumping && _rigidBody2D.velocity.y <= 0 && (_jumpTime <= 0f || _jumpTime > _minJumpTimeBeforeWallSlidingEnabled);
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
        if (_timeAfterJumpPressed <= 0)
        {
            _isGrounded = true;
            _isWallTouching = false;
            _isWallSliding = false;
            _isWallJumping = false;
            _isDoubleJumping = false;
            _isJumping = false;
            _canDash = true;
            _jumpTime = 0f;
            _currentMoveSpeed = _moveSpeed;
            _currentCoyoteTime = _coyoteTime;
        }
    }
    private void SetVariablesWhenWallSliding()
    {
        _isJumping = false;
        _jumpTime = 0f;
        //currentMoveSpeed = 0;
        _currentCoyoteTime = _coyoteTime;
        _canDash = true;
    }
    private void SetVariablesWhenWallJumping()
    {
        _rigidBody2D.velocity = new Vector2(0, 0);
        _isWallJumping = true;
        _isJumping = true;
        _isWallSliding = false;
        _isDoubleJumping = false;
        _jumpTime = 0f;
        _currentMoveSpeed = _moveSpeed;
        _animator.SetTrigger(wallJumpTrigger);
    }
    private void SetVariablesWhenDashing(float move)
    {
        _currentDashCooldown = _dashCooldown;
        _isDashing = true;
        _rigidBody2D.gravityScale = 0f;
        _velocityVector.Set(0, 0);
        _rigidBody2D.velocity = _velocityVector;
        _velocityVector.Set(25f * (Convert.ToInt32(move==0 ? !_facingLeft : move > 0) * 2 - 1), _rigidBody2D.velocity.y);
        _rigidBody2D.velocity = _velocityVector;
        _animator.SetBool("dashing", true);
    }
    private void EndDash()
    {
        _isDashing = false;
        _rigidBody2D.gravityScale = 4f;
        _animator.SetBool("dashing", false);
        _canDash = _isGrounded || _isWallTouching;
        _jumpTime = 0f; // set so you can double jump after a dash
    }
    #endregion SETTING-VARIABLES
}
