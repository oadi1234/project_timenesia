using System.Collections.Generic;
using UnityEngine;

namespace _2___Scripts.UI
{
    public class EffortBar : MonoBehaviour
    {
        [SerializeField]
        private GameObject manaPoint;

        [SerializeField]
        private GameObject backgroundPoint;

        [SerializeField]
        private int maxMana = 4; //default 4

        [SerializeField]
        private int currentEffort =0;

        [SerializeField]
        private int spellCapacity = 0; //default 2

        [SerializeField]
        private float scale = 1f;

        [SerializeField]
        private RectTransform manaBar;

        private List<EffortType> effortType;
        private int oldSpellCapacity;
        private int oldMaxEffort;
        private float positionX = 0f;
        private float positionY = 0f;   
        private List<GameObject> renderedMana;
        private List<GameObject> renderedBackground;

        public void Initialize()
        {
            renderedMana = new List<GameObject>();
            renderedBackground = new List<GameObject>();
            effortType = new List<EffortType>();
            currentEffort = 0;
            oldSpellCapacity = 0;
            oldMaxEffort = 0;
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

        public void SetMaxEffort(int effort, bool shouldRefill = true)
        {
            maxMana = effort;

            if(maxMana > oldMaxEffort)
            {
                for (int i = oldMaxEffort; i < maxMana; i++)
                {
                    GenerateNewPoint(i);
                }
            }
            else if (oldMaxEffort > maxMana)
            {
                for (int i = oldMaxEffort - 1; i >= maxMana; i--)
                {
                    Destroy(renderedMana[i]);
                    renderedMana.RemoveAt(i);
                    effortType.RemoveAt(i);
                    if(i+1<spellCapacity)
                    {
                        //lets just keep it here so I don't forget and hope it just doesn't happen.
                    }
                }
            }

            oldMaxEffort = maxMana;
            if(shouldRefill)
                SetCurrentMana(effort);
        }

        public void SetCurrentMana(int mana)
        {
            if (mana >= maxMana)
            {
                for(int i = currentEffort; i < maxMana; i++)
                {
                    SetEffortAnimation(EffortType.Raw, i);
                }
                currentEffort = maxMana;
            }
            else if (mana > currentEffort)
            {
                for(int i = currentEffort; i < mana; i++)
                {
                    SetEffortAnimation(EffortType.Raw, i);
                }
                currentEffort = mana;
            }
            else
            {
                for (int i = maxMana - 1; i >= mana; i--)
                {
                    SetEffortAnimation(EffortType.Empty, i);
                }
                currentEffort = mana;
            }
        }

        public void SetEffortAnimation(EffortType element, int index)
        {
            Animator animator = renderedMana[index].GetComponent<Animator>();
            SetCurrentElementToFalse(animator, index);
            SwitchToBooleanBasedOnElement(animator, true, element);

            effortType[index] = element;
        }

        internal void CleanManaSources()
        {
            for (int i = 0; i < currentEffort; i++)
            {
                if(effortType[i] != EffortType.Raw)
                    SetEffortAnimation(EffortType.Raw, i);
            }
        }

        private void SetCurrentElementToFalse(Animator animator, int index)
        {
            EffortType element = effortType[index];
            SwitchToBooleanBasedOnElement(animator, false, element);
        }

        private void SwitchToBooleanBasedOnElement(Animator animator, bool boolean, EffortType element)
        {
            switch (element)
            {
                case EffortType.Empty:
                    animator.SetBool("empty", boolean);
                    break;
                case EffortType.Raw:
                    animator.SetBool("full", boolean);
                    break;
                case EffortType.Fire:
                    animator.SetBool("fire", boolean);
                    break;
                case EffortType.Cold:
                    animator.SetBool("cold", boolean);
                    break;
                case EffortType.Force:
                    animator.SetBool("force", boolean);
                    break;
                case EffortType.Life:
                    animator.SetBool("life", boolean);
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
            trans.anchoredPosition = new Vector3(positionX / 2 - 30 + (i * 26 * scale), positionY, -10);
            trans.sizeDelta = new Vector2(28, 28);
            renderedMana.Add(imageObject);
        }

        private void GenerateNewPoint(int i)
        {
            effortType.Add(EffortType.Empty);
            GameObject imageObject = Instantiate(manaPoint);
            imageObject.name = "EffortPoint" + i;
            RectTransform trans = imageObject.GetComponent<RectTransform>();
            imageObject.transform.SetParent(manaBar);
            trans.localScale = Vector2.one * scale;
            trans.anchoredPosition = new Vector3(positionX/2 - 30 + (i * 26 * scale), positionY, -10);

            trans.sizeDelta = new Vector2(28, 28);
        
            renderedMana.Add(imageObject);
        }

        public EffortType GetEffortElementAt(int index)
        {
            return effortType[index];
        }

    }
}
