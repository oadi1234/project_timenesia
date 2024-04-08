using _2___Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadButton : MonoBehaviour
{
    public PreviewStatsDataSchema savePreview { get; set; }
    public string directoryName { get; set; }

    private HealthBar healthBar;
    private EffortBar effortBar;
    private TextMeshProUGUI text;

    public static event Action<string> LoadAction;
    public static event Action<string> DeleteAction;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        effortBar = GetComponentInChildren<EffortBar>();
        healthBar.Initialize();
        effortBar.Initialize();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialize()
    {
        healthBar.SetMaxHealth(savePreview.MaxHealth);
        effortBar.SetMaxEffort(savePreview.MaxEffort);
        effortBar.SetSpellCapacity(savePreview.SpellCapacity);
        // TODO set button image.
        text.text = directoryName; // TODO in the future set the text to something better, like the scene name. For now its convenient for debugging.
    }

    private void OnEnable()
    {
        StartCoroutine(healthBar.FillSequentially(0.3f));
        StartCoroutine(effortBar.FillSequentially(0.3f));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        healthBar.SetCurrentHealth(0);
        effortBar.SetCurrentEffort(0);
    }

    public void Click()
    {
        if (LoadAction != null)
        {
            LoadAction(directoryName);
        }
    }

    public void Delete()
    {
        //SaveManager.Instance.DeleteSave(directoryName);
        if (DeleteAction != null)
            DeleteAction(directoryName);
    }
}
