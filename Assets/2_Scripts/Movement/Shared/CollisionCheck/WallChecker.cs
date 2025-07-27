using UnityEngine;

namespace _2_Scripts.Movement.Shared.CollisionCheck
{
    public class WallChecker : MonoBehaviour
    {
        [SerializeField]
        private float wallCheckRay = 0.3f;

        [SerializeField]
        private float maxAngle = 50f;

        [SerializeField]
        private float wallAngleThreshold = 5f;
    
        [SerializeField]
        private BoxCollider2D boxCollider;
    

        private RaycastHit2D hitFrontTop;
        private RaycastHit2D hitFrontBottom;
        private Bounds boxBounds;
        private Vector2 frontTopColliderCorner;
        private Vector2 frontBottomColliderCorner;
        private float coordinateX;

        private RaycastHit2D bottomLeftHit;
        private RaycastHit2D bottomRightHit;
        private RaycastHit2D bottomLeftOffsetHit; //this and below is for correctly stopping character on slope facing a wall
        private RaycastHit2D bottomRightOffsetHit;
        private RaycastHit2D centerLeftHit;
        private RaycastHit2D centerRightHit;
        private RaycastHit2D landingHit;
        private RaycastHit2D topHit;

        private float rightPositionX;
        private float leftPositionX;
        private float centerPositionY;
        private float bottomPositionY;
        private readonly float correction = 0.01f;

        private bool touchingWall;
        private bool touchingWallLeft;
        private bool touchingWallBottom;
        private bool touchingWallBottomOffset;

        public void Initialize()
        {
            touchingWall = false;
            touchingWallLeft = false;
            touchingWallBottomOffset = false;


            rightPositionX = boxCollider.bounds.max.x + correction;
            bottomPositionY = boxCollider.bounds.min.y - correction;
            leftPositionX = boxCollider.bounds.min.x - correction;
        
            centerPositionY = boxCollider.bounds.center.y;
        }

        public void CalculateRays(bool isFacingLeft, bool isPlayer = false)
        {

            Initialize();
            if (isPlayer)
            {
                if (isFacingLeft)
                {
                    bottomLeftHit = Physics2D.Raycast(new Vector2(leftPositionX, bottomPositionY), Vector2.left, wallCheckRay);
                    centerLeftHit = Physics2D.Raycast(new Vector2(leftPositionX, centerPositionY), Vector2.left, wallCheckRay);
                    bottomLeftOffsetHit = Physics2D.Raycast(new Vector2(leftPositionX, bottomPositionY+0.15f), Vector2.left, wallCheckRay * 0.5f);
                    
                    if (bottomLeftHit.collider && bottomLeftHit.collider.CompareTag("Walls") && Mathf.Abs(Vector2.Angle(bottomLeftHit.normal, Vector2.up) - 90) < wallAngleThreshold)
                    {
                        touchingWall = true;
                        touchingWallLeft = true;
                    }
                    else if(centerLeftHit.collider && centerLeftHit.collider.CompareTag("Walls") && Mathf.Abs(Vector2.Angle(centerLeftHit.normal, Vector2.up) - 90) < wallAngleThreshold)
                    {
                        touchingWall = true;
                        touchingWallLeft = true;
                    }
                    
                    if (bottomLeftOffsetHit.collider && bottomLeftOffsetHit.collider.CompareTag("Walls") &&
                        Mathf.Abs(Vector2.Angle(bottomLeftOffsetHit.normal, Vector2.up) - 90) < wallAngleThreshold)
                    {
                        touchingWallBottomOffset = true;
                    }
                }
                else
                {
                    bottomRightHit = Physics2D.Raycast(new Vector2(rightPositionX, bottomPositionY), Vector2.right, wallCheckRay);
                    centerRightHit = Physics2D.Raycast(new Vector2(rightPositionX, centerPositionY), Vector2.right, wallCheckRay);
                    bottomRightOffsetHit = Physics2D.Raycast(new Vector2(rightPositionX, bottomPositionY + 0.15f), Vector2.right, wallCheckRay * 0.5f);

                    if (bottomRightHit.collider && bottomRightHit.collider.CompareTag("Walls") && (Mathf.Abs(Vector2.Angle(bottomRightHit.normal, Vector2.up) - 90) < wallAngleThreshold))
                    {
                        touchingWall = true;
                        touchingWallLeft = false;
                    }
                    else if(centerRightHit.collider && centerRightHit.collider.CompareTag("Walls") && Mathf.Abs(Vector2.Angle(centerRightHit.normal, Vector2.up) - 90) < wallAngleThreshold) 
                    {
                        touchingWall = true;
                        touchingWallLeft = false;
                    }

                    if (bottomRightOffsetHit.collider && bottomRightOffsetHit.collider.CompareTag("Walls") &&
                        Mathf.Abs(Vector2.Angle(bottomRightOffsetHit.normal, Vector2.up) - 90) < wallAngleThreshold)
                    {
                        touchingWallBottomOffset = true;
                    }
                }
            }
            //
            // if (isPlayer)
            // {
            //     Debug.DrawRay(new Vector2(leftPositionX, bottomPositionY), Vector2.left * wallCheckRay, Color.green);
            //     Debug.DrawRay(new Vector2(rightPositionX, bottomPositionY), Vector2.right * wallCheckRay, Color.red);
            //     Debug.DrawRay(new Vector2(leftPositionX, centerPositionY), Vector2.left * wallCheckRay, Color.green);
            //     Debug.DrawRay(new Vector2(rightPositionX, centerPositionY), Vector2.right * wallCheckRay, Color.red);
            //     Debug.DrawRay(new Vector2(rightPositionX, bottomPositionY + 0.15f), Vector2.right * (wallCheckRay * 0.5f), Color.blue);
            //     Debug.DrawRay(new Vector2(leftPositionX, bottomPositionY + 0.15f), Vector2.left * (wallCheckRay * 0.5f), Color.yellow);
            // }
        }

        #region Checkers
        public bool IsTouchingWall()
        {
            return touchingWall;
        }
        public bool IsLeftTouching()
        {
            return touchingWallLeft;
        }
        public bool IsRightTouching()
        {
            return !touchingWallLeft && touchingWall;
        }

        public bool IsTouchingBottomOffset()
        {
            return touchingWallBottomOffset;
        }

        public bool IsAgainstUnwalkableSurface()
        {
            return  !CanWalkUpTheSurface() && (hitFrontTop || hitFrontBottom);
        }

        public bool IsAgainstWallOrSlope()
        {
            return hitFrontTop || hitFrontBottom;
        }

        public bool CanWalkUpTheSurface()
        {
            return Vector2.Angle(hitFrontBottom.normal, Vector2.up) < maxAngle;
        }
        #endregion Checkers
    }
}
