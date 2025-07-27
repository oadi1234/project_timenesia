using _2_Scripts.UI.Elements.Enum;
using _2_Scripts.UI.Elements.Menu;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _2_Scripts.UI.Elements
{
    public class ButtonFrame : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public UISelectBox selectBox;
        public TooltipController tooltip;
        public TooltipType tooltipType;

        public void OnSelect(BaseEventData eventData)
        {
            if (selectBox)
            {
                selectBox.ToggleActive();
            }
            if (tooltip)
            {
                tooltip.SetTooltipText(tooltipType);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (selectBox)
            {
                selectBox.ToggleActive();
            }
            if (tooltip)
            {
                tooltip.SetTooltipText(TooltipType.None);
            }
        }
    }
}
