using UnityEngine;

namespace _2_Scripts.Scenes.Objects
{
    public class OnLoadDeactivate : MonoBehaviour, IOnLoadChecker
    {

        private void Awake()
        {
            OnLoadHandler();
        }

        public void OnLoadHandler()
        {
            if(SceneDataHolder.Instance.TryGetLoadStrategy(gameObject.scene.name, gameObject.name)) {
                gameObject.SetActive(false);
            }
        }
    }
}
