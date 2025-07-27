using UnityEngine;

namespace _2_Scripts.Player.model
{
    public class SpriteFacingDirection : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        
        protected int Direction = 1;
        
        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        protected void CalculateDirection()
        {
            Direction = spriteRenderer.flipX ? -1 : 1;
        }
        
    }
}