using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Animation
{
    public class PlayerSpriteRotater : MonoBehaviour
    {
        // TODO implement rotation towards enemy on hurt
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private PlayerInputManager playerInputManager;
        [SerializeField] private float maxJumpRotationDuration = 0.2f;
        [SerializeField] private float maxIdleRotationDuration = 0.05f;
        [SerializeField] private float maxJumpRotation = 30f;

        private PlayerAnimationStateHandler playerAnimationStateHandler;
        private float currentVelocity = 0f;
        
        private void Awake()
        {
            playerAnimationStateHandler = GetComponent<PlayerAnimationStateHandler>();
        }

        private void FixedUpdate()
        {
            if (playerInputManager.IsAngleMode())
            {
                transform.rotation = Quaternion.Euler(0, 0, Mathf.SmoothDampAngle(transform.eulerAngles.z, 
                    playerInputManager.GetAngle() * (playerMovementController.IsFacingLeft() ? -1 : 1f), ref currentVelocity, maxIdleRotationDuration));
            }
            else
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
}