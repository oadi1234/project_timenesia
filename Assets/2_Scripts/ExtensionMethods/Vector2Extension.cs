using UnityEngine;

namespace _2_Scripts.ExtensionMethods
{
    public static class Vector2Extension
    {
        public static Vector2 PerpendicularClockwise(this Vector2 vector2)
        {
            return new Vector2(vector2.y, -vector2.x);
        }

        public static Vector2 PerpendicularCounterClockwise(this Vector2 vector2)
        {
            return new Vector2(-vector2.y, vector2.x);
        }
    }
}