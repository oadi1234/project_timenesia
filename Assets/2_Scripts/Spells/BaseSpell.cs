using _2_Scripts.Player;
using _2_Scripts.Player.model;

namespace _2_Scripts.Spells
{
    public class BaseSpell
    {
        public string Name { get; }
        public readonly SpellType SpellType;
    
        public float Duration { get; }
        public static string descriptionSuffix = ".description";

        public delegate void CastDelegate();

        public CastDelegate CastHandler;

        public BaseSpell(string name, CastDelegate castMethod, SpellType spellType, float duration = 0f)
        {
            CastHandler = castMethod;
            Name = name;
            Duration = duration;
            SpellType = spellType;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
