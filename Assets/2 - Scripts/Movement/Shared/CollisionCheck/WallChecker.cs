using UnityEngine;

public class WallChecker : MonoBehaviour
{
    [SerializeField]
    private LayerMask _whatIsGround;

    [SerializeField]
    private float _wallCheckRay = 0.1f;

    [SerializeField]
    private float _maxAngle = 50f;

    [SerializeField]
    private float _wallAngleThreshold = 5f;

    private RaycastHit2D _hitFrontTop;
    private RaycastHit2D _hitFrontBottom;
    private BoxCollider2D _boxCollider;
    private Bounds boxBounds;
    private Vector2 frontTopColliderCorner;
    private Vector2 frontBottomColliderCorner;
    private float coordinateX;

    private RaycastHit2D _leftHit;
    private RaycastHit2D _rightHit;
    private RaycastHit2D landingHit;
    private RaycastHit2D topHit;

    private float _rightPositionX;
    private float _leftPositionX;
    private float _centerPositionY;
    private float centerPositionX;
    private float topPositionY;
    private float bottomPositionY;

    private bool _touchingWall;
    private bool _touchingWallLeft;

    public void Initialize()
    {
        _touchingWall = false;
        _touchingWallLeft = false;

        float correction = 0.01f;

        _boxCollider = GetComponent<BoxCollider2D>();

        _rightPositionX = _boxCollider.bounds.max.x + correction;
        topPositionY = _boxCollider.bounds.max.y + correction;
        bottomPositionY = _boxCollider.bounds.min.y - correction;
        _leftPositionX = _boxCollider.bounds.min.x - correction;

        centerPositionX = _boxCollider.bounds.center.x;
        _centerPositionY = _boxCollider.bounds.center.y;
    }

    public void CalculateRays(bool isFacingLeft, bool isPlayer = false)
    {
        #region OldCalculations
        //    _boxCollider = GetComponent<BoxCollider2D>();
        //    boxBounds = _boxCollider.bounds;
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
        //    _hitFrontTop = Physics2D.Raycast(frontTopColliderCorner, rayDirection, _wallCheckRay, _whatIsGround);
        //    _hitFrontBottom = Physics2D.Raycast(frontBottomColliderCorner, rayDirection, _wallCheckRay, _whatIsGround);
        //}
        #endregion OldCalculations

        Initialize();
        float distance = 0.3f;

        //landingHit = Physics2D.Raycast(new Vector2(this.transform.position.x, bottomPositionY + transform.position.y), new Vector2(transform.position.x, 0.2f));
        //topHit = Physics2D.Raycast(new Vector2(this.transform.position.x, topPositionY + transform.position.y), new Vector2(transform.position.x, 0.2f), 0.2f);

        if (isPlayer)
        {
            if (isFacingLeft)
            {
                _leftHit = Physics2D.Raycast(new Vector2(_leftPositionX, bottomPositionY), Vector2.left, distance);

                if (_leftHit.collider != null)
                {
                    if (_leftHit.collider.tag == "Walls" && Mathf.Abs(Vector2.Angle(_leftHit.normal, Vector2.up) - 90) < _wallAngleThreshold)
                    {
                        _touchingWall = true;
                        _touchingWallLeft = true;
                    }
                }
            }
            else
            {
                _rightHit = Physics2D.Raycast(new Vector2(_rightPositionX, bottomPositionY), Vector2.right, distance);

                if (_rightHit.collider != null/* && !isFacingLeft*/)
                {
                    if (_rightHit.collider.tag == "Walls" && Mathf.Abs(Vector2.Angle(_rightHit.normal, Vector2.up) - 90) < _wallAngleThreshold)
                    {
                        _touchingWall = true;
                        _touchingWallLeft = false;
                    }
                }
            }
        }
        
        if (isPlayer)
        {
            Debug.DrawRay(new Vector2(_leftPositionX, bottomPositionY), Vector2.left * distance, Color.green);
            Debug.DrawRay(new Vector2(_rightPositionX, bottomPositionY), Vector2.right * distance, Color.red);
        }
    }

    #region Checkers
    public bool IsTouchingWall()
    {
        return _touchingWall;
    }
    public bool IsLeftTouching()
    {
        return _touchingWallLeft;
    }
    public bool IsRightTouching()
    {
        return !_touchingWallLeft;
    }

    public bool IsAgainstUnwalkableSurface()
    {
        return  !CanWalkUpTheSurface() && (_hitFrontTop || _hitFrontBottom);
    }

    public bool IsAgainstWallOrSlope()
    {
        return _hitFrontTop || _hitFrontBottom;
    }

    public bool CanWalkUpTheSurface()
    {
        return Vector2.Angle(_hitFrontBottom.normal, Vector2.up) < _maxAngle;
    }
    #endregion Checkers
}
