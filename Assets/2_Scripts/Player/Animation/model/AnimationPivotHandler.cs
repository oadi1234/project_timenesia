using System;
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
        private SpriteRenderer spriteRenderer; //The sprite which it affects

        private Dictionary<Vector2, int>
            coordinatesPerFrame; //coordinates of the pivot point relative to the sprite area for a given frame in order. Probably a percentage.

        private int state; //affected animation state
        private Vector2 pivot;
        private int frame;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            pivot = Vector2.zero;
        }

        void Update()
        {
            spriteRenderer.transform.localEulerAngles =
                new Vector3(0f, 0f, hingeSpriteStateHandler.GetRotationAngle());


            if (playerAnimationStateHandler.GetCurrentFrame() < 0f) frame = 0;
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

            spriteRenderer.transform.localPosition = pivot;
        }
    }
}