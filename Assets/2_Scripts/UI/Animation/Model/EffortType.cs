namespace _2_Scripts.UI.Animation.Model
{
    public enum EffortType
    {
        Empty = 0,
        Aether = 1,     // a
        Entropy = 2,    // e
        Mind = 3,       // m
        Rune = 4,       // r
        Kinesis = 5,    // k
        Raw = 6,
        NoInput = 100, //used only for setting effort on spellcasting, nothing was set
        EndOfInput = 101 //used only for setting effort on spellcasting, end of input
    }
}
