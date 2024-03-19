using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2___Scripts.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private GameObject healthPoint;

        [SerializeField]
        private float scale = 1f;

        [SerializeField]
        private RectTransform healthBar;

        private int maxHealth = 0;
        private int currentHealth = 0;
        private int oldMaxHealth = 0;
        private float positionX = 0f;
        private float positionY = 0f;

        private List<HealthType> healthType;
        private List<GameObject> renderedHealth;

        public void Initialize()
        {
            healthType = new();
            renderedHealth = new();
        }

        public void SetCurrentHealth(int health)
        {
            if (health >= maxHealth)
            {
                for (int i = currentHealth; i < maxHealth; i++)
                {
                    SetHealthAnimation(HealthType.Health, i);
                }
                currentHealth = maxHealth;
            }
            else
            {
                for (int i = maxHealth -1; i >= health; i--)
                {
                    SetHealthAnimation(HealthType.Empty, i);
                }
                currentHealth = health;
            }
        }
        public void SetHealthAnimation(HealthType type, int index)
        {
            Animator animator = renderedHealth[index].GetComponent<Animator>();
            SetCurrentHealthToFalse(animator, index);
            SwitchToBooleanBasedOnType(animator, true, type);

            healthType[index] = type;
        }

        private void SwitchToBooleanBasedOnType(Animator animator, bool boolean, HealthType type)
        {
            switch (type)
            {
                case HealthType.Empty:
                    animator.SetBool("empty", boolean);
                    break;
                case HealthType.Health:
                    animator.SetBool("health", boolean);
                    break;
                case HealthType.Shield:
                    animator.SetBool("shield", boolean);
                    break;
            }
        }

        private void SetCurrentHealthToFalse(Animator animator, int index)
        {
            HealthType element = healthType[index];
            SwitchToBooleanBasedOnType(animator, false, element);
        }

        public void SetMaxHealth(int newMaxHealth)
        {
            maxHealth = newMaxHealth;

            if(oldMaxHealth < maxHealth)
            {
                for(int i = oldMaxHealth; i<maxHealth; i++)
                {
                    GenerateNewPoint(i);
                }
            }
            // Debug only.
            else if (oldMaxHealth > maxHealth)
            {
                for (int i = oldMaxHealth-1; i >= maxHealth; i--)
                {
                    Destroy(renderedHealth[i].gameObject);
                    renderedHealth.RemoveAt(i);
                }
            }
            oldMaxHealth = maxHealth;
            SetCurrentHealth(newMaxHealth);
        }

        private void GenerateNewPoint(int i)
        {
            healthType.Add(HealthType.Empty);
            GameObject imageObject = Instantiate(healthPoint);
            imageObject.name = "HealthBead" + i;
            RectTransform trans = imageObject.GetComponent<RectTransform>();
            imageObject.transform.SetParent(healthBar);
            trans.localScale = Vector2.one * scale;
            if (i % 2 != 0)
            {
                trans.anchoredPosition = new Vector3(positionX + (i * 20 * scale), positionY, -10);
            }
            else
            {
                trans.anchoredPosition = new Vector3(positionX + (i * 20 * scale), positionY - (10 * scale), -10);
            }
            trans.sizeDelta = new Vector2(42, 42);
            imageObject.transform.SetParent(healthBar);

            renderedHealth.Add(imageObject);
        }
    }
}
