using UnityEngine;

namespace _2_Scripts.Movement.Shared.CollisionCheck
{
    public class GroundChecker : MonoBehaviour
    {

        [SerializeField]
        private LayerMask whatIsGround;

        [SerializeField]
        private BoxCollider2D boxCollider;

        [SerializeField]
        private Transform groundCheckMarker;

        [SerializeField]
        private float groundedCheckRay = 0.1f;

        [SerializeField]
        private float wallCheckRay = 0.1f;

        [SerializeField]
        private float cornerCheckRay = 0.5f;

        public Vector2 slopeNormalPerpendicular;

        private Bounds boxBounds;
        private Vector2 bottomLeftColliderCorner;
        private Vector2 bottomRightColliderCorner;
        private readonly Vector3 originOffset = new Vector3(0, 0.1f, 0);
        private RaycastHit2D hitBottomLeft;
        private RaycastHit2D hitBottomRight;
        private float slopeDownAngle;


        public void CalculateRays()
        {
            boxBounds = boxCollider.bounds;
            bottomLeftColliderCorner.Set(boxBounds.min.x, boxBounds.min.y);
            bottomRightColliderCorner.Set(boxBounds.max.x, boxBounds.min.y);
            hitBottomLeft = Physics2D.Raycast(bottomLeftColliderCorner, Vector2.down, groundedCheckRay, whatIsGround);
            hitBottomRight = Physics2D.Raycast(bottomRightColliderCorner, Vector2.down, groundedCheckRay, whatIsGround);
            Debug.DrawRay(hitBottomLeft.point, hitBottomLeft.normal, Color.cyan);
            Debug.DrawRay(hitBottomRight.point, hitBottomRight.normal, Color.magenta);
        }

        public bool IsGrounded()
        {
            if (hitBottomRight || hitBottomLeft)
            {
                return true;
            }
            return false;
        }

        public bool IsOnSlope()
        {
            VerticalSlopeCalculation();

            return slopeDownAngle != 0 || HorizontalSlopeCheck();
        }

        public float FrontSlopeAngle(bool isFacingLeft)
        {
            if(isFacingLeft)
                return Vector2.Angle(hitBottomLeft.normal, Vector2.up);
            else 
                return Vector2.Angle(hitBottomRight.normal, Vector2.up);
        }

        public float GetSlopeAngle()
        {
            return slopeDownAngle;
        }

        public bool IsGroundAhead(bool isFacingLeft)
        {
            if (!isFacingLeft)
            {
                return hitBottomRight;
            }
            else
            {
                return hitBottomLeft;
            }
        }

        private void VerticalSlopeCalculation()
        {
            if (hitBottomLeft && !hitBottomRight)
            {
                slopeDownAngle = CalculateSlope(hitBottomLeft);
            }
            else if (hitBottomRight && !hitBottomLeft)
            {
                slopeDownAngle = CalculateSlope(hitBottomRight);
            }
            else if (hitBottomLeft.point.y > hitBottomRight.point.y)
            {
                slopeDownAngle = CalculateSlope(hitBottomLeft);
            }
            else if (hitBottomLeft.point.y < hitBottomRight.point.y)
            {
                slopeDownAngle = CalculateSlope(hitBottomRight);
            }
            else
            {
                slopeDownAngle = 0f;
            }
        }

        private bool HorizontalSlopeCheck()
        {
            RaycastHit2D hitForwardLeft = Physics2D.Raycast(groundCheckMarker.position + originOffset, groundCheckMarker.transform.right, wallCheckRay, whatIsGround);
            RaycastHit2D hitForwardRight = Physics2D.Raycast(groundCheckMarker.position + originOffset, -groundCheckMarker.transform.right, wallCheckRay, whatIsGround);
            
            return hitForwardLeft || hitForwardRight;
        }

        private float CalculateSlope(RaycastHit2D raycast)
        {
            slopeNormalPerpendicular = Vector2.Perpendicular(raycast.normal).normalized;
            return Vector2.Angle(raycast.normal, Vector2.up);
        }
    }
}

