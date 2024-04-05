using System.Collections;
using System.Collections.Generic;
using _2_Scripts.Player;
using UnityEngine;
using Pathfinding;

public class HunterAgent : MonoBehaviour
{
    [Header("Movement Controls")]
    // public Transform target;
    public LayerMask obstacleLayer;
    public float moveSpeed = 5;
    public float jumpForce = 4;
    public float jumpCooldown = 0.25f; 

    [Header("Gap Calculation")]
    public float calculationInterval = 0.25f; // How often should the path be recalculated
    public int lookAhead = 6; // How many nodes to use when checking for gaps. Larger values may mean gaps are recognised too early.
    public int gapDetection = 3; // the number of unsupported nodes required to recognise a gap. A larger value means that smaller gapes will be ignored.
    public float heightTolerance = 0.5f; // How much higher should a node be to require a jump
    public float unsupportedHeight = 2; // How far down to check before deciding there's nothing there

    public Seeker seeker;
    public Rigidbody2D rb;
    Path currentPath;

    Vector2 moveDirection;
    bool followPath;
    int nextNode;
    float jumpTimer;
    float timer;

    private void Awake()
    {
        timer = calculationInterval + 1; // Forces a first time calculation
    }

    private void Update()
    {
        CalculatePath();
        FollowPath();
    }

    void CalculatePath()
    {
        if (timer > calculationInterval)
        {
            followPath = false; // prevents the object from trying to follow the path before it's ready
            seeker.StartPath(transform.position,  PlayerPosition.GetPlayerPosition().position, OnPathComplete);
            timer -= calculationInterval;
        }
        timer += Time.deltaTime;
    }

    void OnPathComplete(Path path)
    {
        currentPath = path;
        nextNode = 0;
        followPath = true;
    }

    void FollowPath()
    {
        if (followPath && currentPath.path.Count > 0)
        {
            Vector2 targetPosition = (Vector3)currentPath.path[nextNode].position;
            moveDirection = targetPosition - (Vector2)transform.position;

            if (Vector2.Distance(transform.position, targetPosition) < .5f)
            {
                nextNode++;
                if (nextNode >= currentPath.path.Count)
                {
                    followPath = false;
                }
            }

            if (CanJump())
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpTimer = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector2(moveDirection.x, 0) * moveSpeed);
    }

    bool CanJump()
    {
        jumpTimer += Time.deltaTime;
        return (jumpTimer > jumpCooldown && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.01f, obstacleLayer));
    }

    bool ShouldJump()
    {
        int unsupportedNodes = 0;

        for (int i = 0; i < lookAhead; i++)
        {
            bool unsupported = false;

            if (nextNode + i < currentPath.path.Count)
            {
                Vector2 nodePosition = (Vector3)currentPath.path[nextNode + i].position;
                unsupported = !Physics2D.Raycast(nodePosition, Vector2.down, unsupportedHeight, obstacleLayer);
                unsupported = !((nodePosition.y + heightTolerance) < transform.position.y);

                if (unsupported)
                {
                    unsupportedNodes++;
                }
            }
        }

        return (unsupportedNodes >= gapDetection);
    }
}