using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        tooltip.SetTooltipText(tooltipType);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (selectBox)
        {
            selectBox.ToggleActive();
        }
        tooltip.SetTooltipText(TooltipType.None);
    }
}
