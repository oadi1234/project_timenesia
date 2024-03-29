using System;

namespace Spells
{
    public class BaseSpell
    {
        public EffortType[] EffortElements;
        public int CostMana => 0;//EffortElements.Length;
        public string Name { get; }
        public int AnimationTimeMilisec { get; }
        public int CastingTimeMilisec { get; }
    
        public int? Duration { get; }
        public string Description { get; }

        public delegate void CastDelegate();

        public CastDelegate CastHandler;

        public BaseSpell(string name, int cost, int animationTimeMilisec, int castingTimeMilisec, string description, CastDelegate castmethod)
        {
            CastHandler = castmethod;
        }

            public override string ToString()
        {
            return Name + ": " + Description;
        }

        public int GetFullTime()
        {
            return AnimationTimeMilisec + CastingTimeMilisec;
        }
    }
}
