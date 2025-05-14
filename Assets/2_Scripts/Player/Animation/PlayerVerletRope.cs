using System.Collections.Generic;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Animation
{
    public class PlayerVerletRope : MonoBehaviour
    {
        [SerializeField] List<Segment> segments = new();
        [SerializeField] private AnimationPivot animationPivot;
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private int constraintSteps = 10;
        [SerializeField] private float changeMagnitude = 0.5f;
        [SerializeField] private float gravityStrength = -1.5f;
        [SerializeField] private float idleDampeningMultiplier = 0.5f;
        [SerializeField] private float timerBeforeDampening = 0.5f;
        [SerializeField] private float ambientRotationMax = 2f;
        [SerializeField] private float ambientRotationFrequency = 0.3f;

        private Vector2 gravityVector = Vector2.zero;
        private float currentTimer = 0f;
        private bool overrideGravity = false;
        private float gravityTimer = 0f;

        private void Awake()
        {
            for (int i = 1; i < segments.Count; i++)
            {
                segments[i].distanceToParent = Vector2.Distance(segments[i].transform.position,
                    segments[i - 1].transform.position);
            }
        }

        private void FixedUpdate()
        {
            if (playerMovementController.GetTotalVelocity() < 0.5f)
                currentTimer += Time.fixedDeltaTime;
            else
                currentTimer = 0f;
            overrideGravity = gravityTimer > 0;
            if (overrideGravity) gravityTimer -= Time.fixedDeltaTime;

            if (!animationPivot.IsInNonRopeState())
            {
                SimulateRope();
                DoSegmentRotation();
                AttachPosAndRotationToTransform();
                CalculateGravityVector();
                HandleFlip();
            }
            else
            {
                SetExitAngle();
                ResetDistances();
                ResetVelocity();
            }
        }

        public void OverrideGravity(Vector2 gravityDirection, float time)
        {
            if (gravityTimer > 0) return;
            gravityVector = gravityDirection * gravityStrength;
            gravityTimer = time;
        }

        private void SimulateRope()
        {
            for (int i = 1; i < segments.Count; i++)
            {
                Vector2 velocity = segments[i].currentPosition - segments[i].oldPosition;
                velocity *= currentTimer > timerBeforeDampening && gravityTimer <= 0 ? idleDampeningMultiplier : 1f;

                segments[i].oldPosition = segments[i].currentPosition;
                segments[i].currentPosition += velocity;
                segments[i].currentPosition += gravityVector * Time.fixedDeltaTime;
            }

            segments[0].oldPosition = segments[0].currentPosition;
            segments[0].currentPosition = RotatePointOnFirstSegment();
            segments[0].transform.position = segments[0].currentPosition;

            for (int i = 0; i < constraintSteps; i++)
            {
                ApplyConstraints();
            }
        }

        private void DoSegmentRotation()
        {
            for (int i = 0; i < segments.Count - 1; i++)
            {
                Vector2 direction = segments[i + 1].currentPosition - segments[i].currentPosition;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                DoAmbientRotation(ref angle, i);
                segments[i].rotation = angle;
            }
        }

        private void AttachPosAndRotationToTransform()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                segments[i].transform.position = segments[i].currentPosition;
                segments[i].transform.rotation = Quaternion.AngleAxis(segments[i].rotation, Vector3.forward);
            }
        }

        private void CalculateGravityVector()
        {
            if (overrideGravity) return;
            gravityVector.x = -(playerMovementController.GetXVelocity() + (playerMovementController.IsWallSliding()
                ? playerMovementController.GetXVelocityVector()
                : 1f)) * 0.125f * (-gravityStrength);
            gravityVector.y = gravityStrength + playerMovementController.GetYVelocity() * 0.25f * gravityStrength +
                              Mathf.Abs(playerMovementController.GetXVelocity() * 0.0675f * gravityStrength);
        }

        private void HandleFlip()
        {
            segments[0].transform.localScale = new Vector3(segments[0].transform.localScale.x,
                playerMovementController.IsFacingLeft() ? 1 : -1,
                segments[0].transform.localScale.z);
        }

        private Vector3 RotatePointOnFirstSegment()
        {
            // rotate pivot around transform parent position with transform parent rotation
            Vector3 direction = (Vector3)animationPivot.GetPivot() - transform.parent.position;
            direction = transform.parent.rotation * direction;
            return direction + transform.parent.position;
        }

        private void ApplyConstraints()
        {
            for (int i = 0; i < segments.Count - 1; i++)
            {
                Segment first = segments[i];
                Segment second = segments[i + 1];

                float distance = (first.currentPosition - second.currentPosition).magnitude;
                float error = distance - second.distanceToParent * 1.1f;

                Vector2 directionChange = (first.currentPosition - second.currentPosition).normalized;
                Vector2 changeAmount = directionChange * error;

                if (i != 0)
                {
                    first.currentPosition -= changeAmount * changeMagnitude;
                    segments[i] = first;
                    second.currentPosition += changeAmount * changeMagnitude;
                }
                else
                {
                    second.currentPosition += changeAmount;
                }

                segments[i + 1] = second;
            }
        }

        private void ResetDistances()
        {
            for (int i = 1; i < segments.Count; i++)
            {
                segments[i].SetLocalPositionToStartingLocalCoordinates();
                segments[i].rotation = 0f;
                segments[i].transform.localRotation = Quaternion.identity;
            }
        }

        private void ResetVelocity()
        {
            for (int i = 1; i < segments.Count; i++)
            {
                segments[i].currentPosition = segments[i].transform.position;
                segments[i].oldPosition = segments[i].currentPosition;
            }
        }

        private void SetExitAngle()
        {
            segments[0].rotation = animationPivot.GetExitAngle();
            segments[0].transform.rotation = Quaternion.AngleAxis(segments[0].rotation + (playerMovementController.IsFacingLeft() ? 0 : 180), playerMovementController.IsFacingLeft() ? Vector3.forward : Vector3.back);
            segments[0].transform.position = animationPivot.GetPivot();
        }

        private void DoAmbientRotation(ref float angle, int i)
        {
            angle += Mathf.Sin(i * 2f + Time.unscaledTime * (1 / ambientRotationFrequency)) * ambientRotationMax;
        }
    }
}