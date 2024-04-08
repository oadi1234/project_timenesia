using UnityEngine;

namespace _2_Scripts.Player.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerTransform", menuName = "ScriptableObjects/PlayerTransform", order = 5)]
    public class PlayerTransformReference : ScriptableObject 
    {
        public Transform value; 
    }
}