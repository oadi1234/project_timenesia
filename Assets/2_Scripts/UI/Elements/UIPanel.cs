using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//TODO this is absolute mess. Might need a refactor.
namespace _2_Scripts.UI.Elements
{
    public class UIPanel : MonoBehaviour
    {
        public List<(Button button, FadeController fadeController)> ButtonFadeControllerTuples;
        protected FadeController FadeController;
        public bool isActiveAtStart = false;
        public bool controlButtonsFade = true;
        private bool isActive;
        public int buttonIndexOnOpen = 0;

        private void Start()
        {
            LoadButtonList();
            FadeController = GetComponent<FadeController>();
        }
        
        protected virtual void Awake()
        {
            LoadButtonList();
            FadeController = GetComponent<FadeController>();
            SetGameObjectActive(isActiveAtStart);
            if(isActiveAtStart && buttonIndexOnOpen >= 0 && buttonIndexOnOpen < ButtonFadeControllerTuples.Count) 
            {
                ButtonFadeControllerTuples[buttonIndexOnOpen].button.Select();
            }
            isActive = isActiveAtStart;
        }

        public void LoadButtonList()
        {
            // TODO this is relatively buggy. I thought it only affects direct children but it apparently goes all the way down the hierarchy.
            List<Button> butts = new List<Button>(GetComponentsInChildren<Button>());
            ButtonFadeControllerTuples = new List<(Button, FadeController)>();
            foreach (Button b in butts)
            {
                var fadeController = b.GetComponent<FadeController>();
                if (fadeController)
                    ButtonFadeControllerTuples.Add((b, fadeController));
            }
        }

        public void RemoveNullButtons()
        {
            ButtonFadeControllerTuples.RemoveAll(button => !button.button);
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
            SetGameObjectActive(true);
            RemoveNullButtons();
            SetButtonsInteractable(true);
            FadeController.StopAllCoroutines();
            StopAllCoroutines();
            StartCoroutine(Activate());
        }

        public void Close()
        {
            RemoveNullButtons();
            SetButtonsInteractable(false);
            FadeController.StopAllCoroutines();
            StopAllCoroutines();
            StartCoroutine(Deactivate());
        }

        public IEnumerator WaitForButtonFadeOutAndClose()
        {
            SetButtonsInteractable(false);
            FadeController.StopAllCoroutines();
            StopAllCoroutines();
            yield return StartCoroutine(WaitForButtonFadeOutAndDeactivate());
        }

        private IEnumerator Activate()
        {
            if (!controlButtonsFade)
            {
                yield return StartCoroutine(FadeController.DoFadeIn());
            }
            else
            {
                StartCoroutine(FadeController.DoFadeIn());
                yield return StartCoroutine(FadeInButtons());
            }
            if (buttonIndexOnOpen >= 0 && ButtonFadeControllerTuples.Count > buttonIndexOnOpen+1)
            {
                ButtonFadeControllerTuples[buttonIndexOnOpen].button.Select();
            }
        }

        private IEnumerator Deactivate()
        {
            if(ButtonFadeControllerTuples.Count == 0 || !controlButtonsFade)
            {
                yield return StartCoroutine(FadeController.DoFadeOut());
            }
            else
            {
                StartCoroutine(FadeController.DoFadeOut());
                yield return FadeOutButtons();
            }
            SetGameObjectActive(false);
        }

        private IEnumerator WaitForButtonFadeOutAndDeactivate()
        {
            StartCoroutine(FadeOutButtons());
            yield return new WaitForSeconds(0.2f);

            yield return StartCoroutine(FadeController.DoFadeOut());
            SetGameObjectActive(false);
        }

        private IEnumerator FadeInButtons()
        {
            for (int i = 0; i < ButtonFadeControllerTuples.Count; i++)
            {
                ButtonFadeControllerTuples[i].button.interactable = true;
                StartCoroutine(EnableButton(ButtonFadeControllerTuples[i]));
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator FadeOutButtons()
        {
            for (int i = ButtonFadeControllerTuples.Count -1; i >= 0; i--)
            {
                ButtonFadeControllerTuples[i].button.interactable = false;

                yield return DisableButton(ButtonFadeControllerTuples[i]);
            }
        }

        private IEnumerator EnableButton((Button button, FadeController fadeContr) buttonFadeContrTuple)
        {
            SetButtonActive(buttonFadeContrTuple.button, true);

            yield return buttonFadeContrTuple.fadeContr.DoFadeIn();
        }

        private IEnumerator DisableButton((Button button, FadeController fadeContr) buttonFadeContrTuple)
        {

            yield return buttonFadeContrTuple.fadeContr.DoFadeOut();

            SetButtonActive(buttonFadeContrTuple.button, false);
        }

        private void SetButtonsInteractable(bool value)
        {
            foreach ((Button button, FadeController) buttonFadeContrTuple in ButtonFadeControllerTuples)
            {
                buttonFadeContrTuple.button.interactable = value;
            }
        }

        protected virtual void SetGameObjectActive(bool value)
        {
            gameObject.SetActive(value);
        }

        protected virtual void SetButtonActive(Button button, bool value)
        {
            button.gameObject.SetActive(value);
        }
    }
}
