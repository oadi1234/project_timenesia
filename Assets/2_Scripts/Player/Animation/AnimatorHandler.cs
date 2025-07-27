using _2_Scripts.Global.Animation.Model;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _2_Scripts.Player.Animation
{
    public class AnimatorHandler : MonoBehaviour
    {
        [SerializeField, OfType(typeof(IStateHandler))] private Object stateHandler;
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private bool shouldHandleRotation = true;
        private SpriteRenderer spriteRenderer;

        private Animator animator;
        private bool facingLeft;
        private int lastState;
        private bool containsMoreLayers;

        public IStateHandler StateHandler
        {
            get => stateHandler as IStateHandler;
            set => stateHandler = value as Object;
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (playerMovementController)
            {
                playerMovementController.Flipped += (facingLeftParam) => { facingLeft = facingLeftParam; };
            }

            containsMoreLayers = animator.layerCount > 1;
        }
        
        void Update()
        {
            if (shouldHandleRotation)
                SetFacingDirection();
            
            if (animator.HasState(0, StateHandler.GetCurrentState()))
                animator.CrossFade(StateHandler.GetCurrentState(), 0, 0);
            else
                animator.CrossFade(AC.None, 0, 0);
            

            if (containsMoreLayers && animator.HasState(1, StateHandler.GetCurrentHurtState()))
                animator.CrossFade(StateHandler.GetCurrentHurtState(), 0, 1);
            
            if (StateHandler.ShouldRestartAnim())
                animator.Play(StateHandler.GetCurrentState(), 0, 0);
            
            lastState = StateHandler.GetCurrentState();
        }
        
        private void SetFacingDirection()
        {
            if (!StateHandler.LockXFlip())
            {
                spriteRenderer.flipX = !facingLeft;
            }

            // force flip on state change if character was actually flipped
            if (lastState != StateHandler.GetCurrentState() && spriteRenderer.flipX == facingLeft)
            {
                spriteRenderer.flipX = !facingLeft;
            }
        }
    }
}
