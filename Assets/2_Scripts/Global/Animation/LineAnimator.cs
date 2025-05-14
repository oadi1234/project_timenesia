using System;
using UnityEngine;

namespace _2_Scripts.Global.Animation
{
    //animating lines through flipbook shader node makes them appear in waves instead of them being synchronised
    // this class is basically a workaround for that.
    public class LineAnimator : MonoBehaviour
    {
        [SerializeField] private Texture2D[] textures;
        [SerializeField] private int animationSpeedFPS = 24;
        [SerializeField] private bool stopAtLastFrame = false;

        private float animationTimer = 0f;
        private Material material;
        private static readonly int LineTextureId = Shader.PropertyToID("_beam_texture");

        private void Awake()
        {
            var lr = GetComponent<LineRenderer>();
            material = new Material(lr.material); //work around directly modifying material
            lr.material = material;
        }

        private void FixedUpdate()
        {
            animationTimer+=Time.fixedDeltaTime*animationSpeedFPS;
            material.SetTexture(LineTextureId, textures[GetFrame()]);
        }

        private int GetFrame()
        {
            if (stopAtLastFrame)
            {
                return Math.Min(textures.Length - 1, (int)animationTimer);
            }

            return (int)animationTimer % textures.Length;
        }
    }
}
