using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController playerController;

    float xInput = 0f;
    bool isInputEnabled = true;
    bool jumpPressed = false;
    bool keyHeld = false;
    //float maximumPressTime = 0.25f;
    //float currentPressTime = 0f;

    // Update is called once per frame
    void Update()
    {
            xInput = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Jump"))
            {
                jumpPressed = true;
                keyHeld = true;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                keyHeld = false;
            }
    }

    public void SetInputEnabled(bool enable)
    {
        isInputEnabled = enable;
    }

    public bool IsInputEnabled => isInputEnabled;

    private void FixedUpdate()
    {
        if (isInputEnabled)
        {
            playerController.Move(xInput * Time.fixedDeltaTime);
            playerController.Jump(jumpPressed, keyHeld);

            jumpPressed = false;
        }
        else
        {
            xInput = 0;
            keyHeld = false;
            jumpPressed = false;
        }
    }
}
