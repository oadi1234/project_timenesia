using _2_Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class SafeGroundCheckpointChecker : MonoBehaviour
    {
        public Vector2 SafeGroundLocation { get; private set; }
        private Collider2D playerCollider;
        private float SafeSpotYOffset;
    
        // Start is called before the first frame update
        void Start()
        {
            // Time.timeScale = 0.15f;
            SafeGroundLocation = transform.position;
            playerCollider = GetComponent<Collider2D>();
            SafeSpotYOffset = 0.7f; //playerCollider.bounds.max.y / 2; TODO: uncomment this line after player sprite is set (and tween this if necessary)
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == (int) Layers.SafeGroundCheckpoint)
            {
                var bounds = other.bounds;
            
                SafeGroundLocation = new Vector2(bounds.center.x, bounds.min.y + SafeSpotYOffset);
            }
            else if (other.gameObject.layer == (int) Layers.Hazard)
                TeleportPlayerToLastSafeGround();
        }

        private void TeleportPlayerToLastSafeGround()
        {
            transform.position = SafeGroundLocation;
        }
    }
}
