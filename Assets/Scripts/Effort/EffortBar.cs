using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffortBar : MonoBehaviour
{
    [SerializeField]
    private GameObject manaPoint;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    [Range(0, 10)]
    private int maxMana = 4; //default 4

    [SerializeField]
    [Range(0, 10)]
    private int currentMana = 0;

    [SerializeField]
    [Range(1, 5)]
    private int spellCapacity = 2; //default 2

    [SerializeField]
    private float scale = 1f;

    [SerializeField]
    private RectTransform manaBar;


    private float exhaustionTime; //some spells incur it, reducing mana regen
    private List<EffortElement> currentSpell;
    private int currentSpellCost;
    private List<GameObject> renderedMana;
    private List<GameObject> renderedBackground;
    private float canvasWidth;
    private float canvasHeight;

    private void Start()
    {
        renderedMana = new List<GameObject>();
        renderedBackground = new List<GameObject>();
        canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        for(int i =0; i<maxMana; i++)
        {
            GenerateNewPoint(i);
        }
    }

    public void SetSpellCapacity(int capacity)
    {
        this.spellCapacity = capacity;

        
    }

    public void SetMaxMana(int mana)
    {
        this.maxMana = mana;
        this.currentMana = mana;
    }

    public void SetCurrentMana(int mana)
    {
        if (maxMana < mana)
        {
            currentMana = maxMana;
        }
        else
            currentMana = mana;
    }

    public void SetElement(EffortElement element, int index)
    {
        if(index + 1 <= spellCapacity)
        {

        }
    }

    private void GenerateNewBackgroundPoint(int i) //used only for notches really.
    {

    }

    private void GenerateNewPoint(int i)
    {
        GameObject imageObject = manaPoint;
        imageObject.name = "EffortPoint" + i;
        RectTransform trans = imageObject.GetComponent<RectTransform>();
        Animator animator = imageObject.GetComponent<Animator>();
        trans.localScale = Vector2.one * scale;
        float positionX = -(canvasWidth / 2) + 40f;
        float positionY = (canvasHeight / 2) - 40f;
        trans.anchoredPosition = new Vector3(positionX + (i * 20 * scale), positionY, -10);
        trans.sizeDelta = new Vector2(28, 28);
        renderedMana.Add(imageObject);
    }

}
