using _2_Scripts.Player.Animation;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using UnityEngine;

public class PlayerAnimationRotater : MonoBehaviour
{
    // TODO implement rotation towards enemy on hurt

    [SerializeField] private PlayerMovementController playerMovementController;
    [SerializeField] private float maxJumpRotationDuration = 0.2f;
    [SerializeField] private float maxIdleRotationDuration = 0.05f;
    [SerializeField] private float maxJumpRotation = 30f;

    private PlayerAnimationStateHandler playerAnimationStateHandler;
    private float currentVelocity = 0f;

    // smooth damp
    //  current value
    //  target value - where we want it to go
    //  current velocity - reference value, that should probably be used with ref keyword.
    //  smoothTime - how long it should take, so the max rotation duration
    private void Awake()
    {
        playerAnimationStateHandler = GetComponent<PlayerAnimationStateHandler>();
    }

    private void FixedUpdate()
    {
        if (playerAnimationStateHandler.GetCurrentState() == AC.Ascend)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.SmoothDampAngle(transform.eulerAngles.z,
                GetRotationTargetOnJump(), ref currentVelocity,
                maxJumpRotationDuration));
        }

        else if (playerAnimationStateHandler.GetCurrentState() == AC.Descend)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.SmoothDampAngle(transform.eulerAngles.z,
                GetRotationTargetOnFall(), ref currentVelocity,
                maxJumpRotationDuration));
        }

        else
        {
            transform.rotation = Quaternion.Euler(0, 0,
                Mathf.SmoothDampAngle(transform.eulerAngles.z, 0f, ref currentVelocity, maxIdleRotationDuration));
        }
    }

    private float GetRotationTargetOnJump()
    {
        var velocity = Mathf.Abs(playerMovementController.GetXVelocity());
        // at velocity 0 on ascend - 30 degrees
        // at velocity 10 on ascend - 0 degrees
        // max angles - velocity * 3?
        return Mathf.Clamp(maxJumpRotation - velocity * 3f, 0, maxJumpRotation) *
               (playerMovementController.IsFacingLeft() ? -1 : 1f);
    }

    private float GetRotationTargetOnFall()
    {
        var velocity = Mathf.Abs(playerMovementController.GetXVelocity());
        // at velocity 0 on descend - -30 degrees
        // at velocity 10 on descend - 0 degrees
        // -maxrotation + velocity *3
        return Mathf.Clamp(-maxJumpRotation + velocity * 3f, -maxJumpRotation, 0f) *
               (playerMovementController.IsFacingLeft() ? -1 : 1f);
    }
}