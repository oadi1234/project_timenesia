using UnityEngine;


public class CameraScript : MonoBehaviour
{

    [SerializeField] private Transform followX;

    [SerializeField] private Transform followY;

    [SerializeField] private Transform limitTop;

    [SerializeField] private Transform limitBottom;

    [SerializeField] private Transform limitLeft;

    [SerializeField] private Transform limitRight;

    private float smooth = 5f;

    private float x = 0;
    private float y = 0;
    private bool instantSnap = false; // TODO unused for now, might be useful in the future?
    private Vector3 workingVector = Vector3.zero;
    private float cameraWidth;
    private float cameraHeight;

    public static CameraScript Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        cameraHeight = GetComponent<Camera>().orthographicSize;
        cameraWidth = cameraHeight * ((float)Screen.width / Screen.height); //this is deranged but without casting it does not work.

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (followX)
        {
            x = followX.position.x;
        }
        if (followY)
        {
            y = followY.position.y;
        }

        if (instantSnap)
        {
            workingVector.Set(x, y, -10);
        }
        else
        {
            if (limitTop)
            {
                y = Mathf.Min(y + cameraHeight, limitTop.position.y) - cameraHeight;
            }
            if (limitBottom)
            {
                y = Mathf.Max(y - cameraHeight, limitBottom.position.y) + cameraHeight;
            }
            if (limitLeft)
            {
                x = Mathf.Max(x - cameraWidth, limitLeft.position.x) + cameraWidth;
                //Debug.Log("X: " + (x - cameraWidth) + " x limit: " + limitLeft.position.x + "camera width: " + cameraWidth + "camera height: " + cameraHeight + "width/height" + cameraWidth/cameraHeight);
            }
            if (limitRight)
            {
                x = Mathf.Min(x + cameraWidth, limitRight.position.x) - cameraWidth;
            }
            workingVector.Set(
                Mathf.Lerp(transform.position.x, x, Time.deltaTime * smooth),
                Mathf.Lerp(transform.position.y, y, Time.deltaTime * smooth),
                -10);
        }
        transform.position = workingVector;

    }

    public void SetFollowX(Transform follow)
    {
        followX = follow;
    }

    public void SetFollowY(Transform follow)
    {
        followY = follow;
    }

    public void SetFollow(Transform follow)
    {
        followX = follow;
        followY = follow;
    }

    public void SetLimitTop(Transform top)
    {
        limitTop = top;
    }

    public void SetLimitBottom(Transform bottom)
    {
        limitBottom = bottom;
    }

    public void SetLimitLeft(Transform left)
    {
        limitLeft = left;
    }

    public void SetLimitRight(Transform right)
    {
        limitRight = right;
    }

    public void ClearLimits()
    {
        limitBottom = null;
        limitLeft = null;
        limitRight = null;
        limitTop = null;
    }

    public void Detach()
    {
        followX = null;
        followY = null;
    }
}

