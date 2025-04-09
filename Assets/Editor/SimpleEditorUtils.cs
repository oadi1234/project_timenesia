#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Editor
{
#if UNITY_EDITOR
// IN YOUR EDITOR FOLDER, have SimpleEditorUtils.cs.
// paste in this text.
// to play, HIT COMMAND-ZERO rather than command-P
// (the zero key, is near the P key, so it's easy to remember)
// simply insert the actual name of your opening scene
// "__preEverythingScene" on the second last line of code below.

    using UnityEditor;

    [InitializeOnLoad]
    public static class SimpleEditorUtils
    {
        // click command-0 to go to the prelaunch scene and then play

        [MenuItem("Edit/Play-Unplay, But From Prelaunch Scene %0")]
        public static void PlayFromPrelaunchScene()
        {
            if (EditorApplication.isPlaying == true)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/1_Scenes/MainScene.unity");
            EditorApplication.isPlaying = true;
        }
    }
#endif
}