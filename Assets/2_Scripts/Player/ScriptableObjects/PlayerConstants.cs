using UnityEngine;

//might change it from scriptable object to a simple class later on - it requires no instance then and simply works. For now it allows fiddling with numbers.
namespace _2_Scripts.Player.ScriptableObjects
{
    [CreateAssetMenu(fileName = "constants", menuName = "ScriptableObjects/PlayerConstants", order = 1)]
    public class PlayerConstants : ScriptableObject
    {
        static PlayerConstants _instance;

        public static PlayerConstants Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<PlayerConstants>("constants");
                }

                return _instance;
            }
        }

        #region CONSTANTS

        public float coyoteTime = 0.05f;
        public float knockbackTime = 0.5f;
        public float jumpGroundCheckCooldown = 0.01f;
        public float minJumpTimeBeforeWallSlidingEnabled = 0.15f;
        public float groundedCheckRay = 0.1f;
        public float ceilingCheckRadius = 0.1f;
        public float wallCheckRay = 0.2f;
        public float maxJumpTime = 0.2f;
        public float jumpVerticalSpeed = 19f;
        public float dashSpeed = 25f;
        public float doubleJumpPersistenceModifier = 1f;
        public float postWallJumpSpeedModifier = 1.1f;

        public float
            wallJumpPersistenceModifier =
                1f; //increases time counted after jumping, so holding the key is a bit shorter. To count by how much it decreases do 1/(this variable)

        public float moveSpeed = 7f;
        public float dashCooldown = 0.6f;
        public float maxHorizontalSpeed = 25f;
        public float maxVerticalSpeed = 19f;
        public float initialJumpForce = 10f;
        public float doubleJumpForce = 3f;
        public float wallSlideSpeed = -4.5f;

        public float lungeMultiplier = 15f;
        //private float _glideSpeed = 5f; //Unused for now, marked for deletion
        //private float _crouchSpeed = 0.3f; //unused for now, but might get some use later

        #endregion
    }
}