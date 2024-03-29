using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public PlayerMovementController playerMovementController;
    public PlayerAttackController playerAttackController;

    float _xInput = 0f;
    bool _isInputEnabled = true;
    bool _jumpPressed = false;
    bool _jumpKeyHold = false;
    bool _spellAttack = false;
    bool _dashPressed = false;
    int _spellIndex = -1;

    private bool _castingAnimationInProgress = false;
    public bool IsInputEnabled => _isInputEnabled;

    // Update is called once per frame
    void Update()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
            _jumpKeyHold = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            _jumpKeyHold = false;
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
        if(Input.GetButtonDown("Dash"))
        {
            _dashPressed = true;
        }
    }

    private void FixedUpdate()
    {
        if (_isInputEnabled)
        { 
            if (_spellAttack)
            {
                playerAttackController.Attack(_spellIndex);
                _castingAnimationInProgress = true;
                _spellAttack = false;
                _spellIndex = -1;
            }
            else
            {
                //time.fixeddeltatime below is used to adjust movement for occasional lag, if it ever happens. It might be unnecessary.
                //on normal run xInput*fixedDeltaTime is 0.02, so I multiply it by 50 to normalize it to 1. Makes movement speed easier to set in constants.
                // TODO: This will most likely require adjustment if a time slow effects are implemented.
                playerMovementController.Move(_xInput * Time.fixedDeltaTime * 50);
                playerMovementController.Jump(_jumpPressed, _jumpKeyHold);
                playerMovementController.Dash(_dashPressed, _xInput);

                _jumpPressed = false;
                _dashPressed = false;
            }
        }
        else
        {
            _xInput = 0;
            _jumpKeyHold = false;
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
