using System;
using _2_Scripts.Global.SaveSystem.SaveDataSchemas;
using _2_Scripts.UI.Elements.HUD;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI.Elements.MainMenu
{
    public class LoadButton : MonoBehaviour
    {
        public PreviewDataSchema savePreview { get; set; }
        public MainMenuManager mainMenuManager { get; set; }
        public string directoryName { get; set; }

        [SerializeField] private HealthBar healthBar;
        [SerializeField] private EffortBar effortBar;
        [SerializeField] private TextMeshProUGUI text;

        public static event Action<string, PreviewDataSchema> LoadAction;
        public static event Action<string> DeleteAction;

        private void Awake()
        {
            healthBar.Initialize();
            effortBar.Initialize();
        }

        public void Initialize()
        {
            healthBar.SetMax(savePreview.MaxHealth);
            effortBar.SetMax(savePreview.MaxEffort);
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
            healthBar.SetCurrent(0);
            effortBar.SetCurrent(0);
        }

        public void Click()
        {
            LoadAction?.Invoke(directoryName, savePreview);
        }

        public void Delete()
        {
            //SaveManager.Instance.DeleteSave(directoryName);
            mainMenuManager.OpenDeleteSaveConfirmationPanel(directoryName);
        }
    }
}
