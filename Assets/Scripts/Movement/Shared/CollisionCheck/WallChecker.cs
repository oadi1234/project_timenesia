using UnityEngine;

public class WallChecker : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private BoxCollider2D boxCollider;

    [SerializeField]
    private float wallCheckRay = 0.1f;

    private Bounds boxBounds;
    private Vector2 frontTopColliderCorner;
    private Vector2 frontBottomColliderCorner;
    private RaycastHit2D hitFrontTop;
    private RaycastHit2D hitFrontBottom;
    private float coordinateX;

    public void CalculateRays(bool isFacingLeft)
    {
        boxBounds = boxCollider.bounds;
        Vector2 rayDirection;
        if (isFacingLeft)
        {
            coordinateX = boxBounds.center.x - boxBounds.extents.x;
            rayDirection = Vector2.left;
        }
        else
        {
            coordinateX = boxBounds.center.x + boxBounds.extents.x;
            rayDirection = Vector2.right;
        }

        frontTopColliderCorner.Set(coordinateX, boxBounds.center.y + boxBounds.extents.y);
        frontBottomColliderCorner.Set(coordinateX, boxBounds.center.y - boxBounds.extents.y);
        hitFrontTop = Physics2D.Raycast(frontTopColliderCorner, rayDirection, wallCheckRay, whatIsGround);
        hitFrontBottom = Physics2D.Raycast(frontBottomColliderCorner, rayDirection, wallCheckRay, whatIsGround);
        Debug.DrawRay(hitFrontBottom.point, hitFrontBottom.normal, Color.blue);
        Debug.DrawRay(hitFrontTop.point, hitFrontTop.normal, Color.red);
    }
    public bool IsAgainstStraightWall()
    {
        if (hitFrontBottom && !hitFrontTop)
        {
            if (IsNextToSlope())
            {
                return false;
            }
        }
        return hitFrontTop || hitFrontBottom;
    }

    public bool IsAgainstWallOrSlope()
    {
        return hitFrontTop || hitFrontBottom;
    }

    private bool IsNextToSlope()
    {
        Debug.Log(Vector2.Angle(hitFrontBottom.normal, Vector2.up));
        return Vector2.Angle(hitFrontBottom.normal, Vector2.up) != 0f;
    }
}
