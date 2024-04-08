using _2_Scripts.Global.SaveSystem;
using UnityEngine;

namespace _2_Scripts.UI.Elements.MainMenu
{
    public class DeleteSavePanelScript : MonoBehaviour
    {
        public SaveListManager saveListManager;
        public MainMenuManager mainMenuManager;
        public UIPanel saveListPanel;
        public string directoryName { get; set; }

        public void Confirm()
        {
            SaveManager.Instance.DeleteSave(directoryName);
            saveListManager.RemoveElement(directoryName); //button gets destroyed here
            //saveListPanel.RemoveNullsAndDestroyedFromList();
            //saveListPanel.ReloadButtons();
            mainMenuManager.CloseDeleteSaveConfirmationPanel();
        }
    }
}
