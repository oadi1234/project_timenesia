using System.Collections;
using UnityEngine;

namespace _2_Scripts.UI.Elements.MainMenu
{
    public class FadeInScript : MonoBehaviour
    {
        public FadeController BlackPanel;

        // Update is called once per frame
        void Awake()
        {
            StartCoroutine(WaitSingleFrameThenFadeOut());
        }

        private IEnumerator WaitSingleFrameThenFadeOut()
        {
            yield return new WaitForEndOfFrame();
            yield return BlackPanel.DoFadeOut();
            Destroy(this);
        }
    }
}
