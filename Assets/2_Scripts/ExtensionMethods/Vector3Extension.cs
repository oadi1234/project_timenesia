using UnityEngine;

namespace _2_Scripts.ExtensionMethods
{
    public static class Vector3Extension
    {
        private static readonly Vector3 _mirrorHorizontal = new (-1, 1, 1);
        private static readonly Vector3 _mirrorVertical = new (1, -1, 1);
        public static Vector3 MirrorHorizontally(this Vector3 vector3)
        {
            return Vector3.Scale(vector3, _mirrorHorizontal);
        }
        public static Vector3 MirrorVertically(this Vector3 vector3)
        {
            return Vector3.Scale(vector3, _mirrorVertical);
        }

    }
}