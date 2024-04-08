using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
