using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerPosition : MonoBehaviour
    {
        private static Transform _transform;
        public static Transform GetPlayerTransform()
        {
            return _transform;
        }

        public static Vector3 GetPlayerPosition()
        {
            return _transform.position;
        }
        
        private void Awake()
        {
            _transform = gameObject.transform;
        }
    }
}
