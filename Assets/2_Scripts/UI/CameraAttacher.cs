using UnityEngine;

namespace _2_Scripts.UI
{
    public class CameraAttacher : MonoBehaviour
    {
        private Canvas canvas;
        private void Start()
        {
            canvas = GetComponent<Canvas>();
            canvas.worldCamera = CameraScript.Instance.GetComponent<UnityEngine.Camera>();
        }
    }
}
