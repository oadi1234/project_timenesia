using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    [SerializeField]
    private Transform followX;

    [SerializeField]
    private Transform followY;

    private float smooth = 5f;

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
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.position = new Vector3(player.position.x, player.position.y, -10);
        if (followX && followY)
        {
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, followX.position.x, Time.deltaTime * smooth),
                Mathf.Lerp(transform.position.y, followY.position.y, Time.deltaTime * smooth),
                -10);
        }
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

    public void Detach()
    {
        followX = null;
        followY = null;
    }
}
