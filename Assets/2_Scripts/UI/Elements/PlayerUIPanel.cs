using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPanel : UIPanel
{
    public UIPanel PlayerMenuTabs;
    public UIPanel Inventory, Spells, Map, Journal;
    public FadeController PlayerMenuFadeController;
    private IEnumerator fadeInCoroutine;
    private IEnumerator fadeOutCoroutine;

    private UIPlayerTabType currentlyOpenTab = UIPlayerTabType.Inventory; //by default inventory is opened first, but the object remembers the last menu browsed for convenience.
    private void Awake()
    {
        fadeOutCoroutine = PlayerMenuFadeController.DoFadeOut();
        fadeInCoroutine = PlayerMenuFadeController.DoFadeIn();
    }
    public void OpenPlayerMenuUI()
    {
        DoBackgroundFadeIn();
        PlayerMenuTabs.Open();
        switch (currentlyOpenTab)
        {
            case UIPlayerTabType.Inventory:
                Inventory.Open();
                break;
            case UIPlayerTabType.Spells:
                Spells.Open(); 
                break;
            case UIPlayerTabType.Map:
                Map.Open(); 
                break;
            case UIPlayerTabType.Journal:
                Journal.Open(); 
                break;
        }
    }

    public void ClosePlayerMenuUI()
    {
        DoBackgroundFadeOut();
        CloseCurrentlyOpenTab();
        StartCoroutine(CloseCoroutine());
    }

    public void OpenInventory()
    {
        if(currentlyOpenTab!=UIPlayerTabType.Inventory)
        {
            CloseCurrentlyOpenTab();
            currentlyOpenTab = UIPlayerTabType.Inventory;
            Inventory.Open();
        }
    }

    public void OpenSpells()
    {
        if (currentlyOpenTab != UIPlayerTabType.Spells)
        {
            CloseCurrentlyOpenTab();
            currentlyOpenTab = UIPlayerTabType.Spells;
            Spells.Open();
        }
    }

    public void OpenMap()
    {
        if (currentlyOpenTab != UIPlayerTabType.Map)
        {
            CloseCurrentlyOpenTab();
            currentlyOpenTab = UIPlayerTabType.Map;
            Map.Open();
        }
    }

    public void OpenJournal()
    {
        if (currentlyOpenTab != UIPlayerTabType.Journal)
        {
            CloseCurrentlyOpenTab();
            currentlyOpenTab = UIPlayerTabType.Journal;
            Journal.Open();
        }
    }

    private void CloseCurrentlyOpenTab()
    {
        switch (currentlyOpenTab)
        {
            case UIPlayerTabType.Inventory:
                Inventory.Close();
                break;
            case UIPlayerTabType.Spells:
                Spells.Close();
                break;
            case UIPlayerTabType.Map:
                Map.Close();
                break;
            case UIPlayerTabType.Journal:
                Journal.Close(); break;
        }
    }

    public IEnumerator CloseCoroutine()
    {
        yield return PlayerMenuTabs.WaitForButtonFadeOutAndClose();
    }

    private void DoBackgroundFadeIn()
    {
        fadeInCoroutine = PlayerMenuFadeController.DoFadeIn();
        StopCoroutine(fadeOutCoroutine);
        StartCoroutine(fadeInCoroutine);
    }

    private void DoBackgroundFadeOut()
    {
        fadeOutCoroutine = PlayerMenuFadeController.DoFadeOut();
        StopCoroutine(fadeInCoroutine);
        StartCoroutine(fadeOutCoroutine);
    }
}
