using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UIElementPlacing : MonoBehaviour
    {
        [SerializeField]
        private GameObject spriteObject;
        [SerializeField]
        private GameObject canvas;
        [SerializeField]
        private Transform bar;
        [SerializeField]
        private string objectName;
        [SerializeField]
        private float scale;
        [SerializeField]
        private float xOffset;
        [SerializeField]
        private float yOffset;

        private float canvasWidth;
        private float canvasHeight;

        private List<Image> renderedImages;

        private void Awake()
        {
            renderedImages = new List<Image>();
            canvasWidth = canvas.GetComponent<CanvasScaler>().referenceResolution.x;
            canvasHeight = canvas.GetComponent<CanvasScaler>().referenceResolution.y;
            GenerateAndAddNewImage();
        }

        // Start is called before the first frame update
        void Start()
        {
        }
        
        
        private void GenerateAndAddNewImage(int i = 1)
        {
            var imageObject = Instantiate(spriteObject, bar, false);
            imageObject.name = objectName + i;
            
            RectTransform trans = imageObject.GetComponent<RectTransform>();

            trans.localScale = Vector2.one * scale;
            float positionX = -(canvasWidth / 2) + xOffset;
            float positionY = (canvasHeight / 2) + yOffset;
            
            trans.anchoredPosition = new Vector3(positionX / 2 - 30 + (i * 26 * scale), positionY, -10);
            trans.sizeDelta = new Vector2(28, 28);
        }
    }
}