using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController playerController;

    float xInput = 0f;
    bool jumpPressed = false;
    bool keyHeld = false;
    float maximumPressTime = 0.25f;
    float currentPressTime = 0f;

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
            if(currentPressTime < maximumPressTime)
            {
                keyHeld = true;
            }
            else
            {
                keyHeld = false;
            }
            currentPressTime += Time.deltaTime;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            keyHeld = false;
        }

    }

    private void FixedUpdate()
    {
        playerController.Move(xInput * Time.fixedDeltaTime);
        playerController.Jump(jumpPressed, keyHeld);

        jumpPressed = false;
    }
}
