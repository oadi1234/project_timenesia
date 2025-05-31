using System.Collections;
using System.Collections.Generic;
using _2_Scripts.Global;
using _2_Scripts.UI.Animation;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;

namespace _2_Scripts.UI.Elements.HUD
{
    public class HealthBar : AbstractBar<HealthType, HealthPointStateHandler>
    {
        private int currentShield;
        private int oldCurrentShield;

        public override void Initialize()
        {
            MaxValue = GameDataManager.Instance.currentGameData.MaxHealth;
            PointType = new List<HealthType>();
            RenderedPoints = new List<GameObject>();
            StateHandlers = new List<HealthPointStateHandler>();
            OffsetCalculation = CalculateOffset;
            oldCurrentShield = 0;
            currentShield = 0;
        }

        public override void SetCurrent(int health)
        {
            if (health >= MaxValue)
            {
                for (int i = currentValue; i < MaxValue; i++)
                {
                    SetType(HealthType.Health, i);
                }

                currentValue = MaxValue;
            }
            else if (health > currentValue)
            {
                for (int i = currentValue; i < health; i++)
                {
                    SetType(HealthType.Health, i);
                }

                currentValue = health;
            }
            else
            {
                for (int i = MaxValue - 1; i >= health; i--)
                {
                    SetType(HealthType.Empty, i);
                }

                currentValue = health;
            }
        }

        public void SetCurrentShield(int amount)
        {
            oldCurrentShield = currentShield;
            currentShield = amount;
            if (currentShield < oldCurrentShield) //shield was expended
            {
                for (int i = currentShield; i < oldCurrentShield; i++)
                {
                    //offset by max value, shield points should always appear after normal HP
                    Destroy(RenderedPoints[i + MaxValue]); 
                    StateHandlers.RemoveAt(i + MaxValue);
                    RenderedPoints.RemoveAt(i + MaxValue);
                    PointType.RemoveAt(i + MaxValue);
                }
            }
            else if (currentShield > oldCurrentShield) //shield was generated
            {
                for (int i = oldCurrentShield; i < currentShield; i++)
                {
                    GenerateNewPoint(i+MaxValue);
                    SetType(HealthType.Shield, i+MaxValue);
                }
            }
        }

        public IEnumerator FillSequentially(float delay)
        {
            for (int i = 0; i < MaxValue; i++)
            {
                yield return new WaitForSeconds(delay);
                SetType(HealthType.Health, i);
            }
        }

        public override void SetMax(int newMaxHealth)
        {
            base.SetMax(newMaxHealth);
            SetCurrent(newMaxHealth);
        }

        private Vector3 CalculateOffset(int i)
        {
            //if (i % 2 != 0)
            //{
            //    return new Vector3(positionX + (i * 20 * scale), positionY, -10);
            //}
            //else
            //{
            //    return new Vector3(positionX + (i * 20 * scale), positionY - (10 * scale), -10);
            //}
            return new Vector3(positionX + i * 20 * scale, positionY, -10);
        }
    }
}