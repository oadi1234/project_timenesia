using System.Collections.Generic;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.UI.Elements.HUD
{
    // ReSharper disable once InconsistentNaming
    public abstract class AbstractBar<T, U> : MonoBehaviour, IPlayerBar 
        where T : System.Enum
        where U : IGUIPointStateHandler<T>
    {
        [SerializeField] protected GameObject point;
        [SerializeField] protected int currentValue = 0;
        [SerializeField] protected float scale;
        [SerializeField] protected RectTransform bar;
        [SerializeField] protected float positionX = 0f;
        [SerializeField] protected float positionY = 0f;
        [SerializeField] protected Vector2 sizeDelta = new(28, 28);

        protected List<T> PointType;
        protected int OldMaxValue;
        protected int MaxValue;
        protected List<GameObject> RenderedPoints;
        protected List<U> StateHandlers;
        
        protected delegate Vector3 PositionCalculationOffset(int i);
        protected PositionCalculationOffset OffsetCalculation = _ => Vector3.zero;
        
        private U currentStateHandler;

        public abstract void Initialize();

        public virtual void SetMax(int value)
        {
            MaxValue = value;

            if (MaxValue > OldMaxValue)
            {
                for (int i = OldMaxValue; i < MaxValue; i++)
                {
                    GenerateNewPoint(i);
                }
            }
            else if (OldMaxValue > MaxValue)
            {
                for (int i = OldMaxValue - 1; i >= MaxValue; i--)
                {
                    Destroy(RenderedPoints[i]);
                    StateHandlers.RemoveAt(i);
                    RenderedPoints.RemoveAt(i);
                    PointType.RemoveAt(i);
                }
            }

            OldMaxValue = MaxValue;
        }

        public abstract void SetCurrent(int value);

        protected void SetType(T type, int index)
        {
            currentStateHandler = StateHandlers[index];
            currentStateHandler.SetCurrentState(type);
            PointType[index] = type;
        }

        protected void GenerateNewPoint(int i)
        {
            PointType.Add((T)(object)0);
            GameObject imageObject = Instantiate(point, bar, true);
            imageObject.name = "Point_" + i;
            StateHandlers.Insert(i, imageObject.GetComponent<U>());
            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.localScale = Vector2.one * scale;
            rectTransform.anchoredPosition = OffsetCalculation(i);
            rectTransform.sizeDelta = sizeDelta;

            RenderedPoints.Insert(i, imageObject);
        }
    }
}