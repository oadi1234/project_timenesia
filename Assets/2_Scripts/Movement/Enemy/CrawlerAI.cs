using _2_Scripts.Movement.Shared.CollisionCheck;
using UnityEngine;

namespace _2_Scripts.Movement.Enemy
{
    //TODO old and very unreliable. Delete.
    public class CrawlerAI : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 10f;

        [SerializeField]
        private int direction = -1;

        [SerializeField]
        private float scanRadius = 5f;

        [SerializeField]
        private Transform groundCheck;

        private Rigidbody2D rigidBody2D;
        private Vector2 velocityVector;
        private float snapSpeed;
        private Vector2 nearestCollisionPoint;
        private Vector2 rotationPoint;
        private WallChecker wallChecker;
        private BoxCollider2D boxCollider2D;
        private RotatingCollisionChecker rotatingCollisionChecker;
        private Animator animator;
        private bool isRotating;
        private int oldDirection;
        private bool isOnSurface;
        private float angleWithGround;
        private bool rotatesClockwise = false;

        // Start is called before the first frame update
        void Start()
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
            wallChecker = GetComponent<WallChecker>();
            rotatingCollisionChecker = GetComponent<RotatingCollisionChecker>();
            animator = GetComponent<Animator>();
            animator.SetFloat("speed", 1);
            rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
            oldDirection = direction;
            rotatingCollisionChecker.CalculateRays();
            nearestCollisionPoint = rotatingCollisionChecker.FindNearestCollisionPoint(scanRadius);
            groundCheck.position = rotatingCollisionChecker.GetGroundTransformPosition();
            rotationPoint = groundCheck.position;
            snapSpeed = 10f;
            if(direction > 0)
            {
                Flip();
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            rotatingCollisionChecker.CalculateRays();
            if (!isOnSurface)
            {
                velocityVector = (nearestCollisionPoint - new Vector2(rigidBody2D.position.x, rigidBody2D.position.y)).normalized * 0.3f;
                rigidBody2D.velocity = velocityVector;
                float angle = Vector2.Angle(Vector2.down, velocityVector);

                Debug.DrawLine(Vector2.down, velocityVector);

                rigidBody2D.transform.SetPositionAndRotation(rigidBody2D.position, Quaternion.Euler(0, 0, angle));
                if (rotatingCollisionChecker.IsGroundBelow()) isOnSurface = true;
            }
            else
            {
                if(rotatingCollisionChecker.IsGroundAhead())
                {

                    direction = oldDirection;
                }
                if (!rotatingCollisionChecker.IsGroundAhead() && !isRotating)
                {
                    oldDirection = direction;
                    direction = 0;
                    isRotating = true;
                }
                else if (isRotating)
                {
                    ROTET();
                }
                else if (!isRotating)
                {
                    rotationPoint = groundCheck.position;
                    direction = oldDirection;
                }


                Move(direction * Time.fixedDeltaTime * Mathf.Cos(rigidBody2D.rotation * Mathf.Deg2Rad), direction * Time.fixedDeltaTime * Mathf.Sin(rigidBody2D.rotation * Mathf.Deg2Rad));
            }
        }

        private void SnapToNearestSurface()
        {
            isOnSurface = false;
            transform.position = nearestCollisionPoint;
        }

        public void Move(float moveX, float moveY)
        {
            velocityVector.Set(moveSpeed * moveX, moveSpeed * moveY);
            rigidBody2D.velocity = velocityVector;
        }

        private void ROTET()
        {
            //rotate until right angle with ground
            if(rotatesClockwise)
            {
                transform.RotateAround(rotationPoint, new Vector3(0, 0, 1), -moveSpeed * Time.fixedDeltaTime * 1.2f);
            }
            else
            {
                transform.RotateAround(rotationPoint, new Vector3(0, 0, 1), moveSpeed * Time.fixedDeltaTime * 1.2f);
            }

            if(rotatingCollisionChecker.IsGroundAhead() && !rotatingCollisionChecker.IsGroundBelow())
            {
                rotationPoint = rotatingCollisionChecker.GetCollisionPointBelowFrontRaycast();
            }

            else if (rotatingCollisionChecker.IsGroundAhead() && rotatingCollisionChecker.IsGroundBelow())
            {
                isRotating = false;
                angleWithGround = rotatingCollisionChecker.GetGroundAngle();
                if (angleWithGround != rigidBody2D.rotation)
                {
                    if(rigidBody2D.rotation<0)
                    {
                        rigidBody2D.transform.SetPositionAndRotation(rigidBody2D.position, Quaternion.Euler(0, 0, -angleWithGround));
                    }
                    else
                    {
                        rigidBody2D.transform.SetPositionAndRotation(rigidBody2D.position, Quaternion.Euler(0, 0, angleWithGround));
                    }
                }
            }
        }

        private void Flip()
        {
            rotatesClockwise = !rotatesClockwise;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
