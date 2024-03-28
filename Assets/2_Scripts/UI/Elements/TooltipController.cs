using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
