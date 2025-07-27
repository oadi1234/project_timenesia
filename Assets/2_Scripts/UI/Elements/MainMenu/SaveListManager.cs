using System.Collections.Generic;
using _2_Scripts.Global.SaveSystem;
using _2_Scripts.Global.SaveSystem.SaveDataSchemas;
using UnityEngine;

namespace _2_Scripts.UI.Elements.MainMenu
{
    public class SaveListManager : MonoBehaviour
    {
        public List<string> saveDirectoryNameList = new List<string>();

        public GameObject newGameListElement;
        public GameObject loadGameListElement;
        public RectTransform ScrollList;
        public MainMenuManager mainMenuManager;

        private Dictionary<string, GameObject> games = new Dictionary<string, GameObject>();

        public void GeneratePrefabList()
        {
            var lastDirectoryName = "";
            int i = 0;
            foreach (string directoryName in saveDirectoryNameList) 
            {
                GameObject loadButton = Instantiate(loadGameListElement);
                PreviewDataSchema schema = SaveManager.Instance.LoadData<PreviewDataSchema>($"{directoryName}_{SaveManager.SavePreviewSuffix}", directoryName);
                var load = loadButton.GetComponent<LoadButton>();
                load.directoryName = directoryName;
                load.savePreview = schema;
                load.Initialize();
                load.mainMenuManager = mainMenuManager;
                loadButton.GetComponent<IndividualFader>().sequenceIndex = ++i;
                loadButton.name = $"{directoryName}";
                loadButton.transform.SetParent(ScrollList.transform);
                loadButton.transform.localScale = Vector2.one;
                games.Add(directoryName, loadButton);
                lastDirectoryName = directoryName;
            }
            GameObject newGame = Instantiate(newGameListElement);
            var newGameDirectoryName = GetNewGameDirectoryName(lastDirectoryName);
            newGame.GetComponent<StartButton>().directoryName = newGameDirectoryName; 
            newGame.transform.SetParent(ScrollList.transform);
            newGame.transform.localScale = Vector2.one;
            newGame.GetComponent<IndividualFader>().sequenceIndex = ++i;
            newGame.name = $"{newGameDirectoryName}";
            games.Add(newGameDirectoryName, newGame);
        }

        public void RemoveElement(string directoryName)
        {
            bool adjust = false;
            foreach (var game in games)
            {
                if (game.Key == directoryName)
                {
                    adjust = true;
                }
                if (adjust)
                {
                    game.Value.GetComponent<IndividualFader>().sequenceIndex--;
                }
            }
            Destroy(games[directoryName]);
            games.Remove(directoryName);
        }

        private string GetNewGameDirectoryName(string directoryName)
        {
            if (directoryName=="")
            {
                return "save_0";
            }

            for (int i = 0; ; i++)
            {
                if (!games.ContainsKey($"save_{i}"))
                    return $"save_{i}"; //TODO this is actually really bad, as it might set new game to be above an older one. Needs adjustment.
            }
        }
    }
}
