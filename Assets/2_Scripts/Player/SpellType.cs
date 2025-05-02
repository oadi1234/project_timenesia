namespace _2_Scripts.Player
{
    public enum SpellType
    {
        None = 0,
        Bolt = 1, //standard ranged attack
        Aoe = 2, //area of effect
        Heavy = 3, //spell with a charge and impact. Might work differently depending on weapon?
        Buff = 4, //temporary buff.
        Sustained = 5, //sustained spell requiring holding the key, i.e. a continuous beam. Limits movement.
        Meleespell = 6 //short range spells that are not bolts and usually use some touching animation.
    }
}