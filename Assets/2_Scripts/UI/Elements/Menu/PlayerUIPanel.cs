using System.Collections;
using _2_Scripts.UI.Elements.Enum;
using UnityEngine;

namespace _2_Scripts.UI.Elements.Menu
{
    public class PlayerUIPanel : PlayerSubmenuUIPanel
    {
        [SerializeField] private UIPanel playerMenuTabs;
        [SerializeField] private UIPanel inventory;
        [SerializeField] private UIPanel spells;
        [SerializeField] private UIPanel map;
        [SerializeField] private UIPanel journal;
        [SerializeField] private UIPanel tooltip;
        [SerializeField] private FadeController playerMenuFadeController;
        private IEnumerator fadeInCoroutine;
        private IEnumerator fadeOutCoroutine;

        [SerializeField] private UIPlayerTabType currentlyOpenTab = UIPlayerTabType.Inventory; //by default inventory is opened first, but the object remembers the last menu browsed for convenience.
        
        protected override void Awake()
        {
            base.Awake();
            fadeOutCoroutine = playerMenuFadeController.DoFadeOut();
            fadeInCoroutine = playerMenuFadeController.DoFadeIn();
        }
        
        public void OpenPlayerMenuUI()
        {
            DoBackgroundFadeIn();
            tooltip.Open();
            playerMenuTabs.Open();
            switch (currentlyOpenTab)
            {
                case UIPlayerTabType.Inventory:
                    inventory.Open();
                    break;
                case UIPlayerTabType.Spells:
                    spells.Open();
                    break;
                case UIPlayerTabType.Map:
                    map.Open();
                    break;
                case UIPlayerTabType.Journal:
                    journal.Open();
                    break;
            }
        }

        public void ClosePlayerMenuUI()
        {
            DoBackgroundFadeOut();
            CloseCurrentlyOpenTab();
            tooltip.Close();
            StartCoroutine(CloseCoroutine());
        }

        public void OpenInventory()
        {
            if(currentlyOpenTab!=UIPlayerTabType.Inventory)
            {
                CloseCurrentlyOpenTab();
                currentlyOpenTab = UIPlayerTabType.Inventory;
                playerMenuTabs.SetButtonToBeSelectedOnActive((int)UIPlayerTabType.Inventory);
                inventory.Open();
            }
        }

        public void OpenSpells()
        {
            if (currentlyOpenTab != UIPlayerTabType.Spells)
            {
                CloseCurrentlyOpenTab();
                currentlyOpenTab = UIPlayerTabType.Spells;
                playerMenuTabs.SetButtonToBeSelectedOnActive((int)UIPlayerTabType.Spells);
                spells.Open();
            }
        }

        public void OpenMap()
        {
            if (currentlyOpenTab != UIPlayerTabType.Map)
            {
                CloseCurrentlyOpenTab();
                currentlyOpenTab = UIPlayerTabType.Map;
                playerMenuTabs.SetButtonToBeSelectedOnActive((int)UIPlayerTabType.Map);
                map.Open();
            }
        }

        public void OpenJournal()
        {
            if (currentlyOpenTab != UIPlayerTabType.Journal)
            {
                CloseCurrentlyOpenTab();
                currentlyOpenTab = UIPlayerTabType.Journal;
                playerMenuTabs.SetButtonToBeSelectedOnActive((int)UIPlayerTabType.Journal);
                journal.Open();
            }
        }

        private void CloseCurrentlyOpenTab()
        {
            switch (currentlyOpenTab)
            {
                case UIPlayerTabType.Inventory:
                    inventory.Close();
                    break;
                case UIPlayerTabType.Spells:
                    spells.Close();
                    break;
                case UIPlayerTabType.Map:
                    map.Close();
                    break;
                case UIPlayerTabType.Journal:
                    journal.Close(); break;
            }
        }

        public IEnumerator CloseCoroutine()
        {
            yield return playerMenuTabs.WaitForButtonFadeOutAndClose();
        }

        private void DoBackgroundFadeIn()
        {
            fadeInCoroutine = playerMenuFadeController.DoFadeIn();
            StopCoroutine(fadeOutCoroutine);
            StartCoroutine(fadeInCoroutine);
        }

        private void DoBackgroundFadeOut()
        {
            fadeOutCoroutine = playerMenuFadeController.DoFadeOut();
            StopCoroutine(fadeInCoroutine);
            StartCoroutine(fadeOutCoroutine);
        }
    }
}
