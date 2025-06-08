using _2_Scripts.UI.Elements.Enum;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI.Elements.Menu
{
    public class TooltipController : MonoBehaviour
    {
        public TextMeshProUGUI textObject;
        private TooltipStringLoader tooltipStringLoader;
        private void Awake()
        {
            tooltipStringLoader = GetComponent<TooltipStringLoader>();
            textObject.text = tooltipStringLoader.GetTooltip(TooltipType.None);
        }

        public void SetTooltipText(TooltipType tooltip)
        {
            textObject.text = tooltipStringLoader.GetTooltip(tooltip);
        }

    }
}
