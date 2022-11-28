using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneLoader
{
	public static void LoadSceneAndActivate(string sceneName, bool additive = false)
	{
		if (sceneName == null)
		{
			sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		}
		UnityEngine.SceneManagement.SceneManager.LoadScene(
			sceneName, additive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : 0);

		ActivateScene(sceneName);
	}

	public static void LoadScene(string sceneName, bool additive = false)
	{
		if (sceneName == null)
		{
			sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		}
		UnityEngine.SceneManagement.SceneManager.LoadScene(
			sceneName, additive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : 0);		
	}

	public static void ActivateScene(string sceneName)
    {
		//CallAfterDelay.Create(0, () => {
		//	UnityEngine.SceneManagement.SceneManager.SetActiveScene(
		//		UnityEngine.SceneManagement.SceneManager.GetSceneByName(s));
		//});
	}

	public static void UnloadScene(string s)
	{
		UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(s);
	}
}