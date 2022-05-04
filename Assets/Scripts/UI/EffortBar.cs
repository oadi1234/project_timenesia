using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffortBar : MonoBehaviour
{
    [SerializeField]
    private GameObject manaPoint;

    [SerializeField]
    private GameObject backgroundPoint;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private int maxMana = 4; //default 4

    [SerializeField]
    private int currentMana =0;

    [SerializeField]
    private int spellCapacity = 0; //default 2

    [SerializeField]
    private float scale = 1f;

    [SerializeField]
    private RectTransform manaBar;


    private float exhaustionTime; //some spells incur it, reducing mana regen
    private List<EffortElement> currentSpell;
    private int currentSpellCost;
    private int oldSpellCapacity;
    private int oldMaxMana;
    private List<GameObject> renderedMana;
    private List<GameObject> renderedBackground;
    private float canvasWidth;
    private float canvasHeight;

    public void Initialize()
    {
        renderedMana = new List<GameObject>();
        renderedBackground = new List<GameObject>();
        currentSpell = new List<EffortElement>();
        canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        currentMana = 0;
        oldSpellCapacity = 0;
        oldMaxMana = 0;
    }

    public void SetSpellCapacity(int capacity)
    {
        if (spellCapacity > maxMana)
            spellCapacity = maxMana;

        else if (capacity <= 0)
            spellCapacity = 1;

        else
            this.spellCapacity = capacity;

        if(spellCapacity > oldSpellCapacity)
        {
            for(int i = oldSpellCapacity; i< spellCapacity; i++)
            {
                GenerateNewBackgroundPoint(i);
            }
        }
        else if (oldSpellCapacity > spellCapacity)
        {
            for (int i = oldSpellCapacity - 1; i >=spellCapacity; i--)
            {
                Destroy(renderedBackground[i]);
                renderedBackground.RemoveAt(i);
            }
        }
    }

    public void SetMaxMana(int mana, bool shouldRefill = true)
    {
        maxMana = mana;

        if(maxMana > oldMaxMana)
        {
            for (int i = oldMaxMana; i < maxMana; i++)
            {
                GenerateNewPoint(i);
            }
        }
        else if (oldMaxMana > maxMana)
        {
            for (int i = oldMaxMana - 1; i >= maxMana; i--)
            {
                Destroy(renderedMana[i]);
                renderedMana.RemoveAt(i);
                currentSpell.RemoveAt(i);
                if(i+1<spellCapacity)
                {
                    //lets just keep it here so I don't forget and hope it just doesn't happen.
                }
            }
        }

        oldMaxMana = maxMana;
        if(shouldRefill)
            SetCurrentMana(mana);
    }

    public void SetCurrentMana(int mana)
    {
        if (mana >= maxMana)
        {
            for(int i = currentMana; i < maxMana; i++)
            {
                SetElement(EffortElement.Raw, i);
            }
            currentMana = maxMana;
        }
        else if (mana > currentMana)
        {
            for(int i = currentMana; i < mana; i++)
            {
                SetElement(EffortElement.Raw, i);
            }
            currentMana = mana;
        }
        else
        {
            for (int i = maxMana - 1; i >= mana; i--)
            {
                SetElement(EffortElement.Empty, i);
            }
            currentMana = mana;
        }
    }

    public void SetElement(EffortElement element, int index)
    {
        Animator animator = renderedMana[index].GetComponent<Animator>();
        SetCurrentElementToFalse(animator, index);
        SwitchToBooleanBasedOnElement(animator, true, element);

        currentSpell[index] = element;

    }

    internal void CleanManaSources()
    {
        for (int i = 0; i < currentMana; i++)
        {
            if(currentSpell[i] != EffortElement.Raw)
                SetElement(EffortElement.Raw, i);
        }
    }

    private void SetCurrentElementToFalse(Animator animator, int index)
    {
        EffortElement element = currentSpell[index];
        SwitchToBooleanBasedOnElement(animator, false, element);
    }

    private void SwitchToBooleanBasedOnElement(Animator animator, bool boolean, EffortElement element)
    {
        switch (element)
        {
            case EffortElement.Empty:
                animator.SetBool("empty", boolean);
                break;
            case EffortElement.Raw:
                animator.SetBool("full", boolean);
                break;
            case EffortElement.Fire:
                animator.SetBool("fire", boolean);
                break;
            case EffortElement.Water:
                animator.SetBool("water", boolean);
                break;
            case EffortElement.Force:
                animator.SetBool("force", boolean);
                break;
            case EffortElement.Air:
                animator.SetBool("air", boolean);
                break;
        }
    }

    private void GenerateNewBackgroundPoint(int i) //used only for notches really.
    {
        GameObject imageObject = Instantiate(backgroundPoint);
        imageObject.name = "EffortNotch" + i;
        RectTransform trans = imageObject.GetComponent<RectTransform>();
        imageObject.transform.SetParent(manaBar);
        trans.localScale = Vector2.one * scale;
        float positionX = -(canvasWidth / 2) + 40f;
        float positionY = (canvasHeight / 2) - 80f;
        trans.anchoredPosition = new Vector3(positionX / 2 - 30 + (i * 26 * scale), positionY, -10);
        trans.sizeDelta = new Vector2(28, 28);
        renderedMana.Add(imageObject);
    }

    private void GenerateNewPoint(int i)
    {
        currentSpell.Add(EffortElement.Empty);
        GameObject imageObject = Instantiate(manaPoint);
        imageObject.name = "EffortPoint" + i;
        RectTransform trans = imageObject.GetComponent<RectTransform>();
        imageObject.transform.SetParent(manaBar);
        trans.localScale = Vector2.one * scale;
        float positionX = -(canvasWidth / 2) + 40f;
        float positionY = (canvasHeight / 2) - 80f;
        trans.anchoredPosition = new Vector3(positionX/2 - 30 + (i * 26 * scale), positionY, -10);
        trans.sizeDelta = new Vector2(28, 28);
        renderedMana.Add(imageObject);
    }

    public EffortElement GetEffortElementAt(int index)
    {
        return currentSpell[index];
    }

}
