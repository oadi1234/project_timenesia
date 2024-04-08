using System.Collections;
using UnityEngine;

namespace _2_Scripts.UI.Elements
{
    public class UISelectBox : MonoBehaviour
    {

        private FadeController fadeController;
        private bool isActive = false;

        void Start()
        {
            fadeController = GetComponent<FadeController>();
        }

        public void ToggleActive()
        {
            if (isActive)
            {
                Close();
            }
            else
            {
                Open();
            }
            isActive = !isActive;
        }

        public void Open()
        {
            fadeController.StopAllCoroutines();
            StopAllCoroutines();
            StartCoroutine(fadeController.DoFadeIn());
        }

        public void Close()
        {
            fadeController.StopAllCoroutines();
            StopAllCoroutines();
            StartCoroutine(fadeController.DoFadeOut());
        }

        public IEnumerator WaitForFadeOutAndDeactivate()
        {
            fadeController.StopAllCoroutines();
            StopAllCoroutines();
            yield return StartCoroutine(fadeController.DoFadeOut());
        }
    }
}
