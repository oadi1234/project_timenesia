using _2___Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadButton : MonoBehaviour
{
    private PreviewStatsDataSchema savePreview;
    private HealthBar healthBar;
    private EffortBar effortBar;
    private TextMeshProUGUI text;
    private string saveName { get; set; }

    public static event Action<string> Load;

    public LoadButton(PreviewStatsDataSchema savePreview, string saveName)
    {
        this.savePreview = savePreview;
        this.saveName = saveName;
    }
    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        effortBar = GetComponentInChildren<EffortBar>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialize()
    {
        healthBar.SetMaxHealth(savePreview.MaxHealth);
        effortBar.SetMaxEffort(savePreview.MaxEffort);
        // TODO set button image.
        text.text = saveName; // TODO set to something, name of the scene etc.
    }

    public void Click()
    {
        if (Load != null)
        {
            Load(saveName);
        }
    }
}
