using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Player.Animation
{
    public class RopeEvents : MonoBehaviour
    {
        [SerializeField] private List<PlayerVerletRope> allRopeElements = new();
        [SerializeField] private float timer = 3f/12f;
        [SerializeField] private List<RopeSpriteStateHandler> stateHandlers = new();

        public void Sway(Vector2 direction)
        {
            direction.Normalize();
            foreach (var rope in allRopeElements)
            {
                rope.OverrideGravity(direction, timer);
            }

            foreach (var handler in stateHandlers)
            {
                handler.SetMoveTimer(timer*3);
            }
        }
    }
}
