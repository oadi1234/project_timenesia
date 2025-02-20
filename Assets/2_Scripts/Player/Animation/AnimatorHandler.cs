using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Animation
{
    public class AnimatorHandler : MonoBehaviour
    {
        [SerializeField, OfType(typeof(IStateHandler))] private Object _stateHandler;
        [SerializeField] private PlayerMovementController playerMovementController;
        private SpriteRenderer spriteRenderer;

        private Animator animator;
        private bool facingLeft;

        public IStateHandler stateHandler
        {
            get => _stateHandler as IStateHandler;
            set => _stateHandler = value as Object;
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerMovementController.Flipped += (facingLeft) => { this.facingLeft = facingLeft; };
        }

        // Update is called once per frame
        void Update()
        {
            SetFacingDirection();
            if (animator.HasState(0, stateHandler.GetCurrentState()))
                animator.CrossFade(stateHandler.GetCurrentState(), 0, 0);
            else animator.CrossFade(AC.None, 0, 0);
        }
        
        private void SetFacingDirection()
        {
            if (!stateHandler.LockXFlip())
            {
                spriteRenderer.flipX = !facingLeft;
            }
        }
    }
}
