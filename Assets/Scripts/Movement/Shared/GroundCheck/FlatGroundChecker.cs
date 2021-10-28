using UnityEngine;

public class FlatGroundChecker : MonoBehaviour
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

    public Vector2 slopeNormalPerpendicular;

    private Bounds boxBounds;
    private Vector2 bottomLeftColliderCorner;
    private Vector2 bottomRightColliderCorner;
    private RaycastHit2D hitBottomLeft;
    private RaycastHit2D hitBottomRight;
    private float slopeDownAngle;


    public bool CheckIfGrounded()
    {
        boxBounds = boxCollider.bounds;
        bottomLeftColliderCorner.Set(boxBounds.center.x - boxBounds.extents.x, boxBounds.center.y - boxBounds.extents.y);
        bottomRightColliderCorner.Set(boxBounds.center.x + boxBounds.extents.x, boxBounds.center.y - boxBounds.extents.y);
        hitBottomLeft = Physics2D.Raycast(bottomLeftColliderCorner, Vector2.down, groundedCheckRay, whatIsGround);
        hitBottomRight = Physics2D.Raycast(bottomRightColliderCorner, Vector2.down, groundedCheckRay, whatIsGround);

        return IsGrounded();

    }

    public bool IsOnSlope()
    {
        VerticalSlopeCalculation();

        return slopeDownAngle != 0 || HorizontalSlopeCheck();
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

    private bool IsGrounded()
    {
        if (hitBottomRight || hitBottomLeft)
        {
            return true;
        }
        else
        {
            return false;
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
        RaycastHit2D hitBottomLeft = Physics2D.Raycast(groundCheckMarker.position, groundCheckMarker.transform.right, wallCheckRay, whatIsGround);
        RaycastHit2D hitBottomRight = Physics2D.Raycast(groundCheckMarker.position, -groundCheckMarker.transform.right, wallCheckRay, whatIsGround);
        Debug.DrawRay(hitBottomLeft.point, hitBottomLeft.normal, Color.red);
        Debug.DrawRay(hitBottomRight.point, hitBottomRight.normal, Color.red);

        if (hitBottomRight)
        {
            return true;
        }
        else if (hitBottomLeft)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private float CalculateSlope(RaycastHit2D raycast)
    {
        slopeNormalPerpendicular = Vector2.Perpendicular(raycast.normal).normalized;
        return Vector2.Angle(raycast.normal, Vector2.up);
    }


}

