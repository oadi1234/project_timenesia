using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    private Button[] buttons;
    private FadeController fadeController;
    public bool isActiveAtStart = false;
    private bool isActive;
    public int buttonIndexOnOpen = 0;

    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        gameObject.SetActive(isActiveAtStart);
        isActive = isActiveAtStart;
        fadeController = GetComponent<FadeController>();
    }

    public void SelectButton(int index)
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
        SetButtonsInteractable(true);
        fadeController.StopAllCoroutines();
        StopAllCoroutines();
        StartCoroutine(Activate());
    }

    public void Close()
    {
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
        StartCoroutine(fadeController.DoFadeIn());
        yield return StartCoroutine(FadeInButtons());
        if (buttonIndexOnOpen >= 0 && buttons.Length > buttonIndexOnOpen)
        {
            buttons[buttonIndexOnOpen].Select();
        }
    }

    private IEnumerator Deactivate()
    {
        StartCoroutine(fadeController.DoFadeOut());
        yield return StartCoroutine(FadeOutButtons());
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
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
            StartCoroutine(EnableButton(buttons[i]));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator FadeOutButtons()
    {
        for (int i = buttons.Length-1; i >= 0; i--)
        {
            buttons[i].interactable = false;
            
            yield return StartCoroutine(DisableButton(buttons[i]));
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
