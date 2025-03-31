using System.Collections;
using System.Collections.Generic;
using _2_Scripts.Global;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.UI.Elements.HUD
{
    public class EffortBar : MonoBehaviour, IPlayerBar
    {
        [SerializeField]
        private GameObject manaPoint;

        [SerializeField]
        private GameObject backgroundPoint;

        [SerializeField]
        private int currentEffort =0;

        [SerializeField]
        private float scale = 1f;

        [SerializeField]
        private RectTransform manaBar;

        private List<EffortType> effortType;
        private int oldSpellCapacity;
        private int oldMaxEffort;
        private float positionX = 0f;
        private float positionY = 0f;
        private int maxEffort;
        private int spellCapacity;
        private List<GameObject> renderedMana;
        private List<Animator> animators;
        private List<GameObject> renderedBackground;

        public void Initialize()
        {
            maxEffort = GameDataManager.Instance.currentGameData.MaxEffort;
            spellCapacity = GameDataManager.Instance.currentGameData.SpellCapacity;
            renderedMana = new List<GameObject>();
            renderedBackground = new List<GameObject>();
            effortType = new List<EffortType>();
            currentEffort = 0;
            oldSpellCapacity = 0;
            oldMaxEffort = 0;
            animators = new List<Animator>();
        }

        public void SetSpellCapacity(int capacity)
        {
            if (spellCapacity > maxEffort)
                spellCapacity = maxEffort;

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

        public void SetMax(int effort)
        {
            maxEffort = effort;

            if(maxEffort > oldMaxEffort)
            {
                for (int i = oldMaxEffort; i < maxEffort; i++)
                {
                    GenerateNewPoint(i);
                }
            }
            else if (oldMaxEffort > maxEffort)
            {
                for (int i = oldMaxEffort - 1; i >= maxEffort; i--)
                {
                    Destroy(renderedMana[i]);
                    animators.RemoveAt(i);
                    renderedMana.RemoveAt(i);
                    effortType.RemoveAt(i);
                    if(i+1<spellCapacity)
                    {
                        //lets just keep it here so I don't forget and hope it just doesn't happen.
                    }
                }
            }

            oldMaxEffort = maxEffort;
        }

        public void SetCurrent(int mana)
        {
            if (mana >= maxEffort)
            {
                for(int i = currentEffort; i < maxEffort; i++)
                {
                    SetEffortAnimation(EffortType.Raw, i);
                }
                currentEffort = maxEffort;
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
                for (int i = maxEffort - 1; i >= mana; i--)
                {
                    SetEffortAnimation(EffortType.Empty, i);
                }
                currentEffort = mana;
            }
        }

        public void SetEffortAnimation(EffortType element, int index)
        {
            Animator animator = animators[index];
            SetCurrentElementToFalse(animator, index);
            SwitchBooleanBasedOnElement(animator, true, element);

            effortType[index] = element;
        }

        public IEnumerator FillSequentially(float delay)
        {
            for (int i = 0; i < maxEffort; i++)
            {
                yield return new WaitForSeconds(delay);
                SetEffortAnimation(EffortType.Raw, i);
            }
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
            SwitchBooleanBasedOnElement(animator, false, element);
        }

        private void SwitchBooleanBasedOnElement(Animator animator, bool boolean, EffortType element)
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
            GameObject imageObject = Instantiate(backgroundPoint, manaBar, true);
            imageObject.name = "EffortNotch" + i;
            RectTransform trans = imageObject.GetComponent<RectTransform>();
            trans.localScale = Vector2.one * scale;
            trans.anchoredPosition = new Vector3(positionX / 2 - 30 + (i * 26 * scale), positionY, -10);
            trans.sizeDelta = new Vector2(28, 28);
            renderedMana.Add(imageObject);
        }

        private void GenerateNewPoint(int i)
        {
            effortType.Add(EffortType.Empty);
            GameObject imageObject = Instantiate(manaPoint, manaBar, true);
            imageObject.name = "EffortPoint" + i;
            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
            animators.Add(imageObject.GetComponent<Animator>());
            rectTransform.localScale = Vector2.one * scale;
            rectTransform.anchoredPosition = new Vector3(positionX/2 - 30 + (i * 26 * scale), positionY, -10);

            rectTransform.sizeDelta = new Vector2(28, 28);
        
            renderedMana.Add(imageObject);
        }

        public EffortType GetEffortElementAt(int index)
        {
            return effortType[index];
        }

    }
}
