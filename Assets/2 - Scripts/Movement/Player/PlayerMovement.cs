using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController playerController;

    float _xInput = 0f;
    bool _isInputEnabled = true;
    bool _jumpPressed = false;
    bool _keyHeld = false;
    bool _spellAttack = false;
    int _spellIndex = -1;

    private bool _castingAnimationInProgress = false;
    //float maximumPressTime = 0.25f;
    //float currentPressTime = 0f;
    public bool IsInputEnabled => _isInputEnabled;

    // Update is called once per frame
    void Update()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
            _keyHeld = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            _keyHeld = false;
        }

        if (Input.GetButtonDown("Fire1") && !_castingAnimationInProgress)
        {
            _spellAttack = true;
            _spellIndex = 0;
        }
        if (Input.GetButtonDown("Fire2") && !_castingAnimationInProgress)
        {
            _spellAttack = true;
            _spellIndex = 1;
        }
    }

    private void FixedUpdate()
    {
        if (_isInputEnabled)
        { 
            if (_spellAttack)
            {
                playerController.Attack(_spellIndex);
                _castingAnimationInProgress = true;
                _spellAttack = false;
                _spellIndex = -1;
            }
            else
            {
                playerController.Jump(_jumpPressed, _keyHeld);
                playerController.Move(_xInput * Time.fixedDeltaTime);

                _jumpPressed = false;
            }
        }
        else
        {
            _xInput = 0;
            _keyHeld = false;
            _jumpPressed = false;
        }
    }

    public void SetInputEnabled(bool enable)
    {
        _isInputEnabled = enable;
    }

    public void CastingAnimationFinished()
    {
        _castingAnimationInProgress = false;
    }
}
