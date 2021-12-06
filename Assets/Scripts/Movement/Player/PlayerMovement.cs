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
    }

    private void FixedUpdate()
    {
        if (_isInputEnabled)
        {
            playerController.Jump(_jumpPressed, _keyHeld);
            playerController.Move(_xInput * Time.fixedDeltaTime);

            _jumpPressed = false;
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
}
