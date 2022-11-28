using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image fullBead;

    [SerializeField]
    private Image emptyBead;

    [SerializeField]
    private int maxHealth = 0;

    [SerializeField]
    private int currentHealth = 0;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private float scale = 2f;

    [SerializeField]
    private Transform healthBar;

    private int oldMaxHealth;
    private float canvasWidth;
    private float canvasHeight;

    private List<Image> renderedImages;

    private void Awake()
    {
        renderedImages = new List<Image>();
        canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        oldMaxHealth = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetHealth(int health)
    {
        if(health >= maxHealth)
        {
            for (int i = currentHealth; i < maxHealth; i++)
            {
                renderedImages[i].sprite = fullBead.sprite;
            }
            currentHealth = maxHealth;
        }
        else
        {
            for (int i = maxHealth -1; i >= health; i--)
            {
                renderedImages[i].sprite = emptyBead.sprite;
            }
            currentHealth = health;
        }
    }

    public void SetMaxHealth(int health)
    {
        maxHealth = health;

        if(oldMaxHealth < maxHealth)
        {
            for(int i = oldMaxHealth; i<maxHealth; i++)
            {
                GenerateAndAddNewImage(i);
            }
        }
        //this really shouldn't occur naturally too often, or at all. For cheating it might be good to have though.
        else if (oldMaxHealth > maxHealth)
        {
            for (int i = oldMaxHealth-1; i >= maxHealth; i--)
            {
                Destroy(renderedImages[i].gameObject);
                renderedImages.RemoveAt(i);
            }
        }
        oldMaxHealth = maxHealth;
        SetHealth(health);
    }

    private void GenerateAndAddNewImage(int i)
    {
        GameObject imageObject = new GameObject("HealthBead" + i);
        RectTransform trans = imageObject.AddComponent<RectTransform>();
        trans.transform.SetParent(canvas.transform);
        trans.localScale = Vector2.one * scale;
        float positionX = -(canvasWidth / 2) + 40f;
        float positionY = (canvasHeight / 2) - 20f;
        if (i % 2 == 0)
        {
            trans.anchoredPosition = new Vector3(positionX + (i * 20 * scale), positionY, -10);
        }
        else
        {
            trans.anchoredPosition = new Vector3(positionX + (i * 20 * scale), positionY - (10 * scale), -10);
        }
        trans.sizeDelta = new Vector2(42, 42);

        Image image = imageObject.AddComponent<Image>();
        image.sprite = fullBead.sprite;
        imageObject.transform.SetParent(healthBar);
        renderedImages.Add(image);
    }
}
