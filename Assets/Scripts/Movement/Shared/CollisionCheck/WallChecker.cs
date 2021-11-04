using UnityEngine;

public class WallChecker : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private float wallCheckRay = 0.1f;

    [SerializeField]
    private float maxAngle = 50f;

    private Bounds boxBounds;
    private Vector2 frontTopColliderCorner;
    private Vector2 frontBottomColliderCorner;
    private RaycastHit2D hitFrontTop;
    private RaycastHit2D hitFrontBottom;
    private float coordinateX;
    private BoxCollider2D boxCollider;

    public void CalculateRays(bool isFacingLeft)
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxBounds = boxCollider.bounds;
        Vector2 rayDirection;
        if (isFacingLeft)
        {
            coordinateX = boxBounds.center.x - boxBounds.extents.x;
            rayDirection = -GetComponentInParent<Transform>().right;
        }
        else
        {
            coordinateX = boxBounds.center.x + boxBounds.extents.x;
            rayDirection = GetComponentInParent<Transform>().right;
        }
        frontTopColliderCorner.Set(coordinateX, boxBounds.center.y + boxBounds.extents.y);
        frontBottomColliderCorner.Set(coordinateX, boxBounds.center.y - boxBounds.extents.y);
        hitFrontTop = Physics2D.Raycast(frontTopColliderCorner, rayDirection, wallCheckRay, whatIsGround);
        hitFrontBottom = Physics2D.Raycast(frontBottomColliderCorner, rayDirection, wallCheckRay, whatIsGround);
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
