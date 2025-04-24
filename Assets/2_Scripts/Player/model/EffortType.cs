namespace _2_Scripts.Player.model
{
    public enum EffortType
    {
        Empty = 0, //used for unset cast combinations
        Aether = 1,
        Entropy = 2,
        Mind = 3,
        Rune = 4,
        Kinesis = 5,
        Raw = 6,
        NoInput = 100, //used only for setting effort on spellcasting, nothing was set
        EndOfInput = 101 //used only for setting effort on spellcasting, end of input
    }
}
