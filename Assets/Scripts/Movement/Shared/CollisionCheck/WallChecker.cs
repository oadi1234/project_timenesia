using UnityEngine;

public class WallChecker : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private float wallCheckRay = 0.1f;

    [SerializeField]
    private float maxAngle = 50f;

    [SerializeField]
    private float wallAngleThreshold = 5f;

    private Bounds boxBounds;
    private Vector2 frontTopColliderCorner;
    private Vector2 frontBottomColliderCorner;
    private RaycastHit2D hitFrontTop;
    private RaycastHit2D hitFrontBottom;
    private float coordinateX;
    private BoxCollider2D boxCollider;

    private RaycastHit2D landingHit;
    private RaycastHit2D leftHit;
    private RaycastHit2D rightHit;
    private RaycastHit2D topHit;

    float rightPositionX;
    float topPositionY;
    float bottomPositionY;
    float leftPositionX;
    float centerPositionX;
    float centerPositionY;
    bool touchingWall;
    bool touchingWallLeft;

    public void CalculateRays(bool isFacingLeft, bool isPlayer = false)
    {
        //    boxCollider = GetComponent<BoxCollider2D>();
        //    boxBounds = boxCollider.bounds;
        //    Vector2 rayDirection;
        //    if (isFacingLeft)
        //    {
        //        coordinateX = boxBounds.center.x - boxBounds.extents.x;
        //        rayDirection = -GetComponentInParent<Transform>().right;
        //    }
        //    else
        //    {
        //        coordinateX = boxBounds.center.x + boxBounds.extents.x;
        //        rayDirection = GetComponentInParent<Transform>().right;
        //    }
        //    frontTopColliderCorner.Set(coordinateX, boxBounds.center.y + boxBounds.extents.y);
        //    frontBottomColliderCorner.Set(coordinateX, boxBounds.center.y - boxBounds.extents.y);
        //    hitFrontTop = Physics2D.Raycast(frontTopColliderCorner, rayDirection, wallCheckRay, whatIsGround);
        //    hitFrontBottom = Physics2D.Raycast(frontBottomColliderCorner, rayDirection, wallCheckRay, whatIsGround);
        //}
        Initialize();
        float distance = 0.3f; 
        //landingHit = Physics2D.Raycast(new Vector2(this.transform.position.x, bottomPositionY + transform.position.y), new Vector2(transform.position.x, 0.2f));
        leftHit = Physics2D.Raycast(new Vector2(leftPositionX, centerPositionY), Vector2.left, distance);
        rightHit = Physics2D.Raycast(new Vector2(rightPositionX, centerPositionY), Vector2.right, distance);
        //topHit = Physics2D.Raycast(new Vector2(this.transform.position.x, topPositionY + transform.position.y), new Vector2(transform.position.x, 0.2f), 0.2f);
        if (isPlayer)
        {
            Debug.DrawRay(new Vector2(leftPositionX, centerPositionY), Vector2.left * distance, Color.green);
            Debug.DrawRay(new Vector2(rightPositionX, centerPositionY), Vector2.right * distance, Color.red);

            touchingWall = false;
            touchingWallLeft = false;
            if (leftHit.collider != null/* && isFacingLeft*/)
            {
                if (leftHit.collider.tag == "Walls" && Mathf.Abs(Vector2.Angle(leftHit.normal, Vector2.up) - 90) < wallAngleThreshold )
                {
                    touchingWall = true;
                    touchingWallLeft = true;
                }
            }
            else if (rightHit.collider != null/* && !isFacingLeft*/)
            {
                if (rightHit.collider.tag == "Walls" && Mathf.Abs(Vector2.Angle(rightHit.normal, Vector2.up) - 90) < wallAngleThreshold)
                {
                    touchingWall = true; 
                }
            }
            //Debug.Log(touchingWall);
        }
    }

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
        return !touchingWallLeft;
    }

    public void Initialize()
    {
        float correction = 0.01f;
        boxCollider = GetComponent<BoxCollider2D>();
        rightPositionX = boxCollider.bounds.max.x + correction;
        topPositionY = boxCollider.bounds.max.y + correction;
        bottomPositionY = boxCollider.bounds.min.y - correction;
        leftPositionX = boxCollider.bounds.min.x - correction;
        centerPositionX = boxCollider.bounds.center.x;
        centerPositionY = boxCollider.bounds.center.y;
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
}
