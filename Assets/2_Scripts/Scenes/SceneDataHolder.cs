using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Scenes
{
    [CreateAssetMenu(fileName = "dataHolder", menuName = "ScriptableObjects/SceneDataHolder", order = 4)]
    public class SceneDataHolder : ScriptableObject
    {
        //private Dictionary<string, List<string>> inactiveObjectsPerScene = new Dictionary<string, List<string>>();
        //private Dictionary<string, List<string>> alteredObjectsForSavePerScene = new Dictionary<string, List<string>>();

        private Dictionary<string, bool> objectLoadStrategy = new Dictionary<string, bool>(); //string is sceneName+objectName

        static SceneDataHolder _instance;
        public static SceneDataHolder Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<SceneDataHolder>("dataHolder");
                }
                return _instance;
            }
        }

        public void AddData(string sceneName, string objectName)
        {
            objectLoadStrategy.Add(sceneName + objectName, true);
        }

        public bool TryGetLoadStrategy(string sceneName, string objectName) 
        {
            return objectLoadStrategy.GetValueOrDefault(sceneName + objectName, false);
        }

        public void SetLoadStrategyOnGameLoad(Dictionary<string, bool> loadData)
        {
            objectLoadStrategy = loadData;
        }

        public void ClearLoadStrategy()
        {
            objectLoadStrategy = new Dictionary<string, bool>();
        }

        //switch (behaviour)
        //{
        //    case LoadObjectBehaviour.INACTIVE:
        //        Add(sceneName, objectName, ref inactiveObjectsPerScene);
        //        break;
        //    case LoadObjectBehaviour.SAVE_PERSIST:
        //        Add(sceneName, objectName, ref alteredObjectsForSavePerScene);
        //        break;
        //}

        //private void Add(string sceneName, string objectName, ref Dictionary<string, List<string>> listToAdd) 
        //{
        //    if (inactiveObjectsPerScene.ContainsKey(sceneName))
        //    {
        //        inactiveObjectsPerScene.TryGetValue(sceneName, out List<string> list);
        //        list.Add(objectName);
        //        listToAdd[sceneName] = list;
        //    }
        //    else
        //    {
        //        List<string> list = new() { objectName };
        //        listToAdd.Add(sceneName, list);
        //    }
        //}

        //public List<string> tryGetPerLoadBehaviour(string sceneName, LoadObjectBehaviour behaviour = LoadObjectBehaviour.INACTIVE) 
        //{
        //    switch (behaviour)
        //    {
        //        case LoadObjectBehaviour.INACTIVE:
        //            return inactiveObjectsPerScene.GetValueOrDefault(sceneName);
        //        case LoadObjectBehaviour.SAVE_PERSIST:
        //            return alteredObjectsForSavePerScene.GetValueOrDefault(sceneName);
        //        default:
        //            return new();
        //    }
        //}

        //public void setAlteredObjectsOnLoad(Dictionary<string, List<string>> data)
        //{
        //    alteredObjectsForSavePerScene = data;
        //}
    }
}
