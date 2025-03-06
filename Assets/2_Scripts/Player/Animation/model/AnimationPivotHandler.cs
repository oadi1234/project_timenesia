using System.Collections.Generic;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Animation.model
{
    public class AnimationPivotHandler : MonoBehaviour
    {
        [SerializeField] HingeSpriteStateHandler hingeSpriteStateHandler;
        [SerializeField] PlayerAnimationStateHandler playerAnimationStateHandler;
        [SerializeField] PlayerMovementController playerMovementController;
        [SerializeField] private Animator bodyAnimator;

        private Dictionary<Vector2, int>
            coordinatesPerFrame; //coordinates of the pivot point relative to the sprite area for a given frame in order. Probably a percentage.

        private int state; //affected animation state
        private Vector2 pivot;
        private int frame;

        void Awake()
        {
            pivot = Vector2.zero;
        }

        void Update()
        {
            frame = (int)(bodyAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length *
                          (bodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) *
                          bodyAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
            if (hingeSpriteStateHandler.GetHingeSpriteType() == 1)
            {
                pivot = PivotPerFrame.Instance.GetHairPivotLocation(playerAnimationStateHandler.GetCurrentState(),
                    frame);
            }
            else
                pivot = PivotPerFrame.Instance.GetCloakPivotLocation(playerAnimationStateHandler.GetCurrentState(),
                    frame);

            if (!playerMovementController.IsFacingLeft())
            {
                pivot.x = -pivot.x;
            }

            // transform.localPosition = pivot;
        }

        // The function below predicts where the pivot will be in the next frame. It is necessary to work when adjusting
        // position of a rigidbody, otherwise any rigidbody trying to overlap the point will appear as if it was
        // "lagging behind".
        public Vector2 GetPivot()
        {
            float x = pivot.x;
            float y = pivot.y;
            x += playerMovementController.GetXVelocity() * Time.fixedDeltaTime;
            y += playerMovementController.GetYVelocity() * Time.fixedDeltaTime;
            return (Vector2)transform.position + new Vector2(x, y);
        }

        public float GetRotationAngle()
        {
            return hingeSpriteStateHandler.GetRotationAngle();
        }
    }
}