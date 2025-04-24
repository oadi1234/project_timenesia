using System.Collections;
using System.Collections.Generic;
using _2_Scripts.Global;
using _2_Scripts.Player.Animation.GUI;
using _2_Scripts.Player.model;
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
        private List<EffortPointStateHandler> effortPointStateHandlers;
        private EffortPointStateHandler currentEffortPointStateHandler;

        public void Initialize()
        {
            maxEffort = GameDataManager.Instance.currentGameData.MaxEffort;
            spellCapacity = GameDataManager.Instance.currentGameData.SpellCapacity;
            renderedMana = new List<GameObject>();
            effortType = new List<EffortType>();
            currentEffort = 0;
            oldSpellCapacity = 0;
            oldMaxEffort = 0;
            effortPointStateHandlers = new List<EffortPointStateHandler>();
        }

        public void SetSpellCapacity(int capacity)
        {
            if (spellCapacity > maxEffort)
                spellCapacity = maxEffort;

            else if (capacity <= 0)
                spellCapacity = 1;

            else
                spellCapacity = capacity;

            if(spellCapacity > oldSpellCapacity)
            {
                for(int i = oldSpellCapacity; i< spellCapacity; i++)
                {
                    GenerateNewBackgroundPoint(i);
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
                    effortPointStateHandlers.RemoveAt(i);
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
                    SetEffortType(EffortType.Raw, i);
                }
                currentEffort = maxEffort;
            }
            else if (mana > currentEffort)
            {
                for(int i = currentEffort; i < mana; i++)
                {
                    SetEffortType(EffortType.Raw, i);
                }
                currentEffort = mana;
            }
            else
            {
                for (int i = maxEffort - 1; i >= mana; i--)
                {
                    SetEffortType(EffortType.Empty, i);
                }
                currentEffort = mana;
            }
        }

        public void SetEffortType(EffortType element, int index)
        {
            currentEffortPointStateHandler = effortPointStateHandlers[index];
            currentEffortPointStateHandler.SetCurrentState(element);

            effortType[index] = element;
        }

        public bool TrySetEffortType(EffortType element, int index)
        {
            if (index + 1 > currentEffort) return false;
            SetEffortType(element, index);
            return true;
        }

        public IEnumerator FillSequentially(float delay)
        {
            for (int i = 0; i < maxEffort; i++)
            {
                yield return new WaitForSeconds(delay);
                SetEffortType(EffortType.Raw, i);
            }
        }

        internal void CleanManaSources()
        {
            for (int i = 0; i < currentEffort; i++)
            {
                if(effortType[i] != EffortType.Raw)
                    SetEffortType(EffortType.Raw, i);
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
            effortPointStateHandlers.Add(imageObject.GetComponent<EffortPointStateHandler>());
            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
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
