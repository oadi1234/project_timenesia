using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Animation
{
    public class AnimationPivot : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationStateHandler playerAnimationStateHandler;
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private Animator bodyAnimator;
        private RopeSpriteStateHandler ropeSpriteStateHandler;

        private Vector2 pivot;
        private int frame;

        void Awake()
        {
            pivot = Vector2.zero;
            ropeSpriteStateHandler = GetComponent<RopeSpriteStateHandler>();
        }

        void FixedUpdate()
        {
            frame = (int)(bodyAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length *
                          (bodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) *
                          bodyAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
            if (ropeSpriteStateHandler.GetHingeSpriteType() == 1)
            {
                pivot = PointPerState.Instance.GetHairPivotLocation(playerAnimationStateHandler.GetCurrentState(),
                    frame);
            }
            else
                pivot = PointPerState.Instance.GetCloakPivotLocation(playerAnimationStateHandler.GetCurrentState(),
                    frame);

            if (!playerMovementController.IsFacingLeft())
            {
                pivot.x = -pivot.x;
            }
            
        }
        
        public Vector2 GetPivot()
        {
            return (Vector2)transform.position + pivot;
        }

        public bool IsInNonRopeState()
        {
            return PointPerState.Instance.IsNonRopeState(playerAnimationStateHandler.GetCurrentState());
        }

        public float GetExitAngle()
        {
            return PointPerState.Instance.GetExitAngle(playerAnimationStateHandler.GetCurrentState());
        }
    }
}