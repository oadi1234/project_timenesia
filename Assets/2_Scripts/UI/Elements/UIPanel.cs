using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Elements
{
    public class UIPanel : MonoBehaviour
    {
        public List<Button> buttons;
        private FadeController fadeController;
        public bool isActiveAtStart = false;
        public bool controlButtonsFade = true;
        private bool isActive;
        public int buttonIndexOnOpen = 0;

        void Start()
        {
            buttons = new List<Button>(GetComponentsInChildren<Button>());
            gameObject.SetActive(isActiveAtStart);
            if(isActiveAtStart && buttonIndexOnOpen >= 0 && buttonIndexOnOpen < buttons.Count) 
            {
                buttons[buttonIndexOnOpen].Select();
            }
            isActive = isActiveAtStart;
            fadeController = GetComponent<FadeController>();
        }

        public void LoadButtonList()
        {
            buttons = new List<Button>(GetComponentsInChildren<Button>());
        }

        public void RemoveNullButtons()
        {
            buttons.RemoveAll(button => button == null);
        }

        public void SetButtonToBeSelectedOnActive(int index)
        {
            buttonIndexOnOpen = index;
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
            gameObject.SetActive(true);
            RemoveNullButtons();
            SetButtonsInteractable(true);
            fadeController.StopAllCoroutines();
            StopAllCoroutines();
            StartCoroutine(Activate());
        }

        public void Close()
        {
            RemoveNullButtons();
            SetButtonsInteractable(false);
            fadeController.StopAllCoroutines();
            StopAllCoroutines();
            StartCoroutine(Deactivate());
        }

        public IEnumerator WaitForButtonFadeOutAndClose()
        {
            SetButtonsInteractable(false);
            fadeController.StopAllCoroutines();
            StopAllCoroutines();
            yield return StartCoroutine(WaitForButtonFadeOutAndDeactivate());
        }

        private IEnumerator Activate()
        {
            if (!controlButtonsFade)
            {
                yield return StartCoroutine(fadeController.DoFadeIn());
            }
            else
            {
                StartCoroutine(fadeController.DoFadeIn());
                yield return StartCoroutine(FadeInButtons());
            }
            if (buttonIndexOnOpen >= 0 && buttons.Count > buttonIndexOnOpen)
            {
                buttons[buttonIndexOnOpen].Select();
            }
        }

        private IEnumerator Deactivate()
        {
            if(buttons.Count == 0 || !controlButtonsFade)
            {
                yield return StartCoroutine(fadeController.DoFadeOut());
            }
            else
            {
                StartCoroutine(fadeController.DoFadeOut());
                yield return FadeOutButtons();
            }
            gameObject.SetActive(false);
        }

        private IEnumerator WaitForButtonFadeOutAndDeactivate()
        {
            StartCoroutine(FadeOutButtons());
            yield return new WaitForSeconds(0.2f);

            yield return StartCoroutine(fadeController.DoFadeOut());
            gameObject.SetActive(false);
        }

        private IEnumerator FadeInButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].interactable = true;
                StartCoroutine(EnableButton(buttons[i]));
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator FadeOutButtons()
        {
            for (int i = buttons.Count -1; i >= 0; i--)
            {
                buttons[i].interactable = false;

                yield return DisableButton(buttons[i]);
            }
        }

        private IEnumerator EnableButton(Button button)
        {
            button.gameObject.SetActive(true);

            yield return button.GetComponent<FadeController>().DoFadeIn();
        }

        private IEnumerator DisableButton(Button button)
        {

            yield return button.GetComponent<FadeController>().DoFadeOut();

            button.gameObject.SetActive(false);
        }

        private void SetButtonsInteractable(bool value)
        {
            foreach (Button button in buttons)
            {
                button.interactable = value;
            }
        }
    }
}
