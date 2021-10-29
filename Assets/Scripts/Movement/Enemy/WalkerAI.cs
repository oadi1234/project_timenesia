using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerAI : MonoBehaviour
{
    [SerializeField]
    private FlatGroundChecker flatGroundChecker;

    [SerializeField]
    private WallChecker wallChecker;

    [SerializeField]
    private float moveSpeed = 100f;

    [SerializeField]
    private PhysicsMaterial2D noFriction;

    [SerializeField]
    private PhysicsMaterial2D allFriction;

    [SerializeField]
    private float initialTurnTime = 1f;

    [SerializeField]
    private int speed = 1;


    private Rigidbody2D rigidBody2D;
    private Vector2 velocityVector;
    private bool facingLeft = true;
    private bool isOnSlope;
    private bool isGrounded;
    private int oldSpeed;
    private float turnTime;
    private bool encounteredNoGroundOrAWall;

    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        turnTime = initialTurnTime;
        encounteredNoGroundOrAWall = false;
        oldSpeed = speed;
        if(speed < 0)
        {
            Flip();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        wallChecker.CalculateRays(facingLeft);
        if (flatGroundChecker.CheckIfGrounded() && !flatGroundChecker.IsGroundAhead(facingLeft))
        {
            isOnSlope = flatGroundChecker.IsOnSlope();
            encounteredNoGroundOrAWall = wallChecker.IsAgainstStraightWall();
            if (!isOnSlope || encounteredNoGroundOrAWall)
            {
                Flip();
                oldSpeed = speed;
                turnTime = initialTurnTime;
            }
        }
        else if (encounteredNoGroundOrAWall && turnTime > 0)
        {
            speed = 0;
            turnTime -= Time.fixedDeltaTime;
        }
        else
        {
            turnTime = initialTurnTime;
            speed = oldSpeed * -1;
            encounteredNoGroundOrAWall = false;
        }

        Move(speed * Time.fixedDeltaTime);
    }

    public void Move(float move)
    {
        if (!isGrounded)
        {
            velocityVector.Set(moveSpeed * move, rigidBody2D.velocity.y);
        }
        else if (isGrounded && !isOnSlope)
        {
            velocityVector.Set(moveSpeed * move, 0.0f);
        }
        else if (isOnSlope)
        {
            velocityVector.Set(
                -moveSpeed * flatGroundChecker.slopeNormalPerpendicular.x * move,
                -moveSpeed * flatGroundChecker.slopeNormalPerpendicular.y * move);
        }

        if (isOnSlope && move == 0f)
        {
            rigidBody2D.sharedMaterial = allFriction;
        }
        else
        {
            rigidBody2D.sharedMaterial = noFriction;
        }

        rigidBody2D.velocity = velocityVector;
    }

    public void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
