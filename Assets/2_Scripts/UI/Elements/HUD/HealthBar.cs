using System.Collections;
using System.Collections.Generic;
using _2_Scripts.Model;
using _2_Scripts.Player.model;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.UI.Elements.HUD
{
    public class HealthBar : MonoBehaviour, IPlayerBar
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
        private readonly float positionX = 0f;
        private readonly float positionY = 0f;

        private List<HealthType> healthType;
        private List<Animator> animators;
        private List<GameObject> renderedHealth;

        public void Initialize()
        {
            healthType = new List<HealthType>();
            renderedHealth = new List<GameObject>();
            animators = new List<Animator>();
        }

        public void SetCurrent(int health)
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
            Animator animator = animators[index];
            SetCurrentHealthToFalse(animator, index);
            SwitchToBooleanBasedOnType(animator, true, type);

            healthType[index] = type;
        }

        public IEnumerator FillSequentially(float delay)
        {
            for (int i = 0; i < maxHealth; i++)
            {
                yield return new WaitForSeconds(delay);
                SetHealthAnimation(HealthType.Health, i);
            }
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

        public void SetMax(int newMaxHealth)
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
                    animators.RemoveAt(i);
                    renderedHealth.RemoveAt(i);
                }
            }
            oldMaxHealth = maxHealth;
            SetCurrent(newMaxHealth);
        }

        private void GenerateNewPoint(int i)
        {
            healthType.Add(HealthType.Empty);
            GameObject imageObject = Instantiate(healthPoint, healthBar, true);
            imageObject.name = "HealthBead" + i;
            RectTransform trans = imageObject.GetComponent<RectTransform>();
            animators.Add(imageObject.GetComponent<Animator>());
            trans.localScale = Vector2.one * scale;
            trans.anchoredPosition = new Vector3(positionX + (i * 20 * scale), positionY, -10);
            trans.sizeDelta = new Vector2(42, 42);
            imageObject.transform.SetParent(healthBar);

            renderedHealth.Add(imageObject);
        }
    }
}
