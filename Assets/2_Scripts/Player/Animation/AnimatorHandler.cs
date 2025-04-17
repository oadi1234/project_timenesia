using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _2_Scripts.Player.Animation
{
    public class AnimatorHandler : MonoBehaviour
    {
        [SerializeField, OfType(typeof(IStateHandler))] private Object _stateHandler;
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private bool shouldHandleRotation = true;
        private SpriteRenderer spriteRenderer;

        private Animator animator;
        private bool facingLeft;
        private int lastState;

        public IStateHandler stateHandler
        {
            get => _stateHandler as IStateHandler;
            set => _stateHandler = value as Object;
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (playerMovementController)
            {
                playerMovementController.Flipped += (facingLeftParam) => { this.facingLeft = facingLeftParam; };
            }
        }
        
        void Update()
        {
            if (shouldHandleRotation)
                SetFacingDirection();
            if (animator.HasState(0, stateHandler.GetCurrentState()))
                animator.CrossFade(stateHandler.GetCurrentState(), 0, 0);
            else animator.CrossFade(AC.None, 0, 0);

            if (animator.HasState(1, stateHandler.GetCurrentHurtState()))
                animator.Play(stateHandler.GetCurrentHurtState(), 1, 0);
            
            if (stateHandler.ShouldRestartAnim())
                animator.Play(stateHandler.GetCurrentState(), 0, 0);
            
            lastState = stateHandler.GetCurrentState();
        }
        
        private void SetFacingDirection()
        {
            if (!stateHandler.LockXFlip())
            {
                spriteRenderer.flipX = !facingLeft;
            }

            // force flip on state change if character was actually flipped
            if (lastState != stateHandler.GetCurrentState() && spriteRenderer.flipX == facingLeft)
            {
                spriteRenderer.flipX = !facingLeft;
            }
        }
    }
}
