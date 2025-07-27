using System.Collections;
using System.Collections.Generic;
using _2_Scripts.Global;
using _2_Scripts.UI.Animation;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;

namespace _2_Scripts.UI.Elements.HUD
{
    public class EffortBar : AbstractBar<EffortType, EffortPointStateHandler>
    {
        [SerializeField]
        private GameObject backgroundPoint;
        
        private int oldSpellCapacity;
        private int spellCapacity;

        public override void Initialize()
        {
            MaxValue = GameDataManager.Instance.currentGameData.MaxEffort;
            spellCapacity = GameDataManager.Instance.currentGameData.SpellCapacity;
            RenderedPoints = new List<GameObject>();
            PointType = new List<EffortType>();
            currentValue = 0;
            oldSpellCapacity = 0;
            OldMaxValue = 0;
            StateHandlers = new List<EffortPointStateHandler>();
            OffsetCalculation = CalculateOffset;
        }

        public void SetSpellCapacity(int capacity)
        {
            if (spellCapacity > MaxValue)
                spellCapacity = MaxValue;

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

        public override void SetCurrent(int mana)
        {
            if (mana >= MaxValue)
            {
                for(int i = currentValue; i < MaxValue; i++)
                {
                    SetType(EffortType.Raw, i);
                }
                currentValue = MaxValue;
            }
            else if (mana > currentValue)
            {
                for(int i = currentValue; i < mana; i++)
                {
                    SetType(EffortType.Raw, i);
                }
                currentValue = mana;
            }
            else
            {
                for (int i = MaxValue - 1; i >= mana; i--)
                {
                    SetType(EffortType.Empty, i);
                }
                currentValue = mana;
            }
        }

        public bool TrySetEffortType(EffortType element, int index)
        {
            if (index + 1 > currentValue) return false;
            SetType(element, index);
            return true;
        }

        public IEnumerator FillSequentially(float delay)
        {
            for (int i = 0; i < MaxValue; i++)
            {
                yield return new WaitForSeconds(delay);
                SetType(EffortType.Raw, i);
            }
        }

        internal void CleanManaSources()
        {
            for (int i = 0; i < currentValue; i++)
            {
                if(PointType[i] != EffortType.Raw)
                    SetType(EffortType.Raw, i);
            }
        }
        
        private Vector3 CalculateOffset(int i)
        {
            return new Vector3(positionX / 2 - 30 + i * 26 * scale, positionY, -10);
        }

        private void GenerateNewBackgroundPoint(int i) //used only for notches really.
        {
            GameObject imageObject = Instantiate(backgroundPoint, bar, true);
            imageObject.name = "Notch_" + i;
            RectTransform trans = imageObject.GetComponent<RectTransform>();
            trans.localScale = Vector2.one * scale;
            trans.anchoredPosition = OffsetCalculation(i);
            trans.sizeDelta = new Vector2(28, 28);
            RenderedPoints.Add(imageObject);
        }
    }
}
