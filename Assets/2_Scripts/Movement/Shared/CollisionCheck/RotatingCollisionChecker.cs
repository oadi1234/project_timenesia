using UnityEngine;

namespace _2_Scripts.Movement.Shared.CollisionCheck
{
    //TODO unused, delete.
    public class RotatingCollisionChecker : MonoBehaviour
    {
        [SerializeField]
        private LayerMask whatIsGround;

        [SerializeField]
        private float collisionRayLength = 0.2f;

        [SerializeField]
        private Vector2 nearestCollisionPoint = Vector2.zero;

        private Vector2 boxBottom;
        private Vector2 frontCorner;
        private float rotationAngle;
        private RaycastHit2D hitFromBoxCenter;
        private RaycastHit2D hitFromBoxBottomFront;

        private Vector2 direction;
        private BoxCollider2D boxCollider2D;

        void Awake()
        {
            direction = -transform.up;
            boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        }

        public void CalculateRays()
        {
            rotationAngle = boxCollider2D.attachedRigidbody.rotation * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Sin(rotationAngle), -Mathf.Cos(rotationAngle));

            Debug.DrawLine(Vector2.zero, direction);
            float left = boxCollider2D.offset.x - (boxCollider2D.size.x / 2f);
            float bottom = boxCollider2D.offset.y - (boxCollider2D.size.y / 2f);

            boxBottom = transform.TransformPoint(new Vector3(boxCollider2D.offset.x, bottom, 0f));
            frontCorner = transform.TransformPoint(new Vector3(left, bottom, 0f));
            Debug.DrawLine(boxBottom, frontCorner);
            hitFromBoxCenter = Physics2D.Raycast(boxBottom, direction, collisionRayLength, whatIsGround);
            hitFromBoxBottomFront = Physics2D.Raycast(frontCorner, direction, collisionRayLength, whatIsGround);

            Debug.DrawRay(hitFromBoxCenter.point, hitFromBoxCenter.normal, Color.blue);
            Debug.DrawRay(hitFromBoxBottomFront.point, hitFromBoxBottomFront.normal, Color.red);

        }

        public bool IsGroundBelow()
        {
            return hitFromBoxCenter;
        }

        public bool IsGroundAhead()
        {
            return hitFromBoxBottomFront;
        }

        public Vector2 FindNearestCollisionPoint(float radius)
        {
            Collider2D[] colliders = new Collider2D[1];
            Physics2D.OverlapCircleNonAlloc(boxBottom, radius, colliders, whatIsGround);
            if (colliders[0] != null)
            {
                nearestCollisionPoint = colliders[0].ClosestPoint(boxBottom);
            }

            return nearestCollisionPoint;
        }

        public Vector2 GetCollisionPointBelowFrontRaycast()
        {
            return hitFromBoxBottomFront.point;
        }

        public float GetGroundAngle()
        {
            return Vector2.Angle(Vector2.up, hitFromBoxBottomFront.normal.normalized);
        }

        public Vector2 GetGroundTransformPosition()
        {
            return new Vector2(boxBottom.x, boxBottom.y - Vector2.Distance(boxBottom, frontCorner));
        }

    }
}
