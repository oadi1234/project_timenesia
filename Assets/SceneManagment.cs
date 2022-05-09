using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{
    private bool loaded = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!loaded)
        {
            loaded = true;
            SceneManager.LoadScene("Area1", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
        //if (loaded)
        //{
        //    SceneManager.SetActiveScene(SceneManager.GetSceneByName("Area1"));
        //}
        Debug.Log("SCENE LOADED: " + loaded);
    }




    bool transitionIsDone;  // set elsewhere when the camera finishes moving
    string nextSceneName;
    AsyncOperation async;

    void loadNextScene()
    {
        nextSceneName = "Area1";
        async = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        async.allowSceneActivation = false;
    }

    //void Update()
    //{
    //    if (transitionIsDone)
    //    {
    //        Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
    //        if (nextScene.IsValid())
    //        {
    //            Scene activeScene = SceneManager.GetActiveScene();
    //            SceneManager.SetActiveScene(nextScene);
    //            async.allowSceneActivation = true;
    //            SceneManager.UnloadScene(activeScene.buildIndex);
    //            nextSceneName = null;
    //        }
    //    }
    //}
}
