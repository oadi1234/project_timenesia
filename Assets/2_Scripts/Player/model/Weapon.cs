namespace _2_Scripts.Player.model
{
    // actual weapons
    //TODO because weapon animations will need to be handled in their own special way, a sprite swapper for weapon
    // sprite will need to be created through code. As such, there should probably be no animator controller there
    // in the final version.
    // This class will probably help with that, but for now I will leave it as is.
    public enum Weapon
    {
        None = 0,
        StartingStaff = 1,
        Fists = 2
    }
}