using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerAI : MonoBehaviour
{
    [SerializeField]
    private FlatGroundChecker flatGroundChecker;

    [SerializeField]
    private float moveSpeed = 100f;

    [SerializeField]
    private PhysicsMaterial2D noFriction;

    [SerializeField]
    private PhysicsMaterial2D allFriction;

    [SerializeField]
    private float initialTurnTime = 2f;


    private Rigidbody2D rigidBody2D;
    private Vector2 velocityVector;
    private bool facingLeft = true;
    private bool isOnSlope;
    private bool isGrounded;
    private int direction;
    private int oldDirection;
    private float turnTime;
    private bool encounteredNoGroundOrWall;

    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        direction = 1;
        turnTime = initialTurnTime;
        encounteredNoGroundOrWall = false;
        oldDirection = direction;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (flatGroundChecker.CheckIfGrounded() && !flatGroundChecker.IsGroundAhead(facingLeft))
        {
            Flip();
            oldDirection = direction;
            encounteredNoGroundOrWall = true;
            turnTime = initialTurnTime;
        }
        else if (encounteredNoGroundOrWall && turnTime > 0)
        {
            direction = 0;
            turnTime -= 0.1f;
        }
        else
        {
            direction = oldDirection * -1;
            encounteredNoGroundOrWall = false;
        }

        Move(direction * Time.fixedDeltaTime);
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
