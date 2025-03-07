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
        private float freezeSimulationTimer = 0f;

        void Awake()
        {
            for (int i = 1; i < segments.Count; i++)
            {
                segments[i].distanceToParent = (Vector2.Distance(segments[i].transform.position,
                    segments[i - 1].transform.position));
            }
        }

        void FixedUpdate()
        {
            if (playerMovementController.GetTotalVelocity() < 0.5f)
                currentTimer += Time.fixedDeltaTime;
            else
                currentTimer = 0f;

            SimulateRope();
            DoSegmentRotation();
            AttachPosAndRotationToTransform();
            CalculateGravityVector();
            HandleFlip();
        }

        private void SimulateRope()
        {
            for (int i = 1; i < segments.Count; i++)
            {
                Vector2 velocity = segments[i].currentPosition - segments[i].oldPosition;
                velocity *= currentTimer > timerBeforeDampening ? idleDampeningMultiplier : 1f;

                segments[i].oldPosition = segments[i].currentPosition;
                segments[i].currentPosition += velocity;
                segments[i].currentPosition += gravityVector * Time.fixedDeltaTime;
            }

            segments[0].oldPosition = segments[0].currentPosition;
            segments[0].transform.position = RotatePointOnFirstSegment();
            segments[0].currentPosition = RotatePointOnFirstSegment();

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
            gravityVector.x = -(playerMovementController.GetXVelocity() + (playerMovementController.GetIsWallSliding()
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

        private void DoAmbientRotation(ref float angle, int i)
        {
            angle += Mathf.Sin(i * 2f + Time.unscaledTime * (1 / ambientRotationFrequency)) * ambientRotationMax;
        }
    }
}