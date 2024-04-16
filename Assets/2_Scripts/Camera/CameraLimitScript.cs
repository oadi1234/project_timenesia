using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLimitScript : MonoBehaviour
{
    [SerializeField]
    private Transform follow;

    [SerializeField]
    private Transform followX;

    [SerializeField]
    private Transform followY;

    [SerializeField]
    private Transform limitTop;

    [SerializeField]
    private Transform limitBottom;

    [SerializeField]
    private Transform limitLeft;

    [SerializeField]
    private Transform limitRight;

    public bool resetsToPlayerOnLeave = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (follow)
                CameraScript.Instance.SetFollow(follow);
            if (followX)
                CameraScript.Instance.SetFollowX(followX);
            if (followY)
                CameraScript.Instance.SetFollowY(followY);
            CameraScript.Instance.SetLimitTop(limitTop);
            CameraScript.Instance.SetLimitBottom(limitBottom);
            CameraScript.Instance.SetLimitLeft(limitLeft);
            CameraScript.Instance.SetLimitRight(limitRight);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (resetsToPlayerOnLeave && collision.gameObject.CompareTag("Player"))
        {
            CameraScript.Instance.SetFollow(collision.gameObject.transform);
            CameraScript.Instance.ClearLimits();
        }
    }
}
