using System.Collections;
using System.IO;
using UnityEngine;

namespace _2_Scripts.UI.Elements.MainMenu
{
    public class SaveListLoader : MonoBehaviour
    {
        public SaveListManager SaveListManager;
        // Start is called before the first frame update
        void Awake()
        {
            StartCoroutine(LoadListThenDeleteScript());
        }

        private IEnumerator LoadListThenDeleteScript()
        {
            var directoryInfo = new DirectoryInfo($"{Application.persistentDataPath}/saves/").GetDirectories();
            foreach ( var directory in directoryInfo)
            {
                if (directory.Name.StartsWith("save_"))
                {
                    //var fileInfoDirectory = directory.GetFiles();
                    //foreach ( var file in fileInfoDirectory)
                    //{
                    //TODO: some sort of validation that all the correct files are inside the game save might be needed.
                    //}
                    SaveListManager.saveDirectoryNameList.Add(directory.Name);
                }
                yield return null;
            }
            SaveListManager.GeneratePrefabList();
            // TODO In case we want to make the game load everything on start, we should send an event here that it is done and we can close the "loading intermission" screen.
            Destroy(this);
        }
    }
}
