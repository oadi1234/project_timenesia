using _2_Scripts.Player;
using _2_Scripts.Player.model;

namespace _2_Scripts.Spells
{
    public class BaseSpell
    {
        // public EffortType[] EffortCombination; //id is stored in the Spellbook
        public string Name { get; }
        // public int CostMana { get; private set; } //it's always EffortCombination.Length;
        // public int AnimationTimeMilisec { get; } //TODO delete. Animation events handle this.
        // public int CastingTimeMilisec { get; }
        public SpellType SpellType;
    
        public float Duration { get; } //for buffs and sustained spells
        public string Description { get; }

        public delegate void CastDelegate();

        public CastDelegate CastHandler;

        public BaseSpell(string name, string description, CastDelegate castMethod, SpellType spellType, float duration = 0f)
        {
            CastHandler = castMethod;
            Name = name;
            Description = description;
            // EffortCombination = effortCombination;
            // CostMana = cost;
            Duration = duration;
            SpellType = spellType;
        }

        public override string ToString()
        {
            return Name + ": " + Description;
        }
    }
}
