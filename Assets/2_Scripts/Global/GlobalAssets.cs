using UnityEngine;

namespace _2_Scripts.Global
{
    public class GlobalAssets : MonoBehaviour
    {
        [Header("SFX")]
        public AudioClip SaveAudioClip;
    
        [Space(10)]
        [Header("Music")]
        public AudioClip Music_01;
        public AudioClip Music_02;
        public AudioClip Music_03;
    
        public static GlobalAssets Instance = null;
	
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad (gameObject);
        }
    }
}


