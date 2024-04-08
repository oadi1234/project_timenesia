using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerPosition : MonoBehaviour
    {
        private static Transform _transform;
        public static Transform GetPlayerPosition()
        {
            return _transform;
        }
        
        private void Awake()
        {
            _transform = gameObject.transform;
        }
    }
}
