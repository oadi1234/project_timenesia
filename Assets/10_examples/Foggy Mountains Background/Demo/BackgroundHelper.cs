using UnityEngine;
using UnityEngine.UI;

namespace _10_examples.Foggy_Mountains_Background.Demo
{
    public class BackgroundHelper : MonoBehaviour
    {
        public float speed = 0;
        float pos = 0;
        private RawImage image;
        // Start is called before the first frame update
        void Start()
        {
            image = GetComponent<RawImage>();
        }

        // Update is called once per frame
        void Update()
        {
            pos += speed;

            if (pos > 1.0F)

                pos -= 1.0F;

            image.uvRect = new Rect(pos, 0, 1, 1);
        }
    }
}
