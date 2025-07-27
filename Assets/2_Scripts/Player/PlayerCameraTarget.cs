using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerCameraTarget : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] [Range(0f, 10f)] private float movementMultiplierX = 0.5f;
        [SerializeField] [Range(0f, 10f)] private float movementMultiplierY = 0.2f;
        [SerializeField] [Range(0f, 10f)] private float movementSharpnessX = 3f;
        [SerializeField] [Range(0f, 10f)] private float movementSharpnessY = 3f;
        private Vector3 targetPosition;

        private void FixedUpdate()
        {
            targetPosition.Set(Mathf.Lerp(transform.localPosition.x, playerMovementController.GetXVelocity()*movementMultiplierX, Time.deltaTime*movementSharpnessX), 
                Mathf.Lerp(transform.localPosition.y, playerMovementController.GetYVelocity()*movementMultiplierY, Time.deltaTime*movementSharpnessY), transform.localPosition.z);
            transform.localPosition = targetPosition;
            Debug.DrawLine(transform.position, playerMovementController.transform.position, Color.cyan);
        }
    }
}
