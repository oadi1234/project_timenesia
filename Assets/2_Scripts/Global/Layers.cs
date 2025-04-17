namespace _2_Scripts.Global
{
    public enum Layers
    {
        //TODO this needs cleaning up. I have already deleted the following layers:
        // Spell (6) - playerAttack is more universal, Collision(13), Ceiling(14), OneWayGround(15).
        // I need to know whether stuff like SafeGroundCheckpoint, Interactables and EnemyNavigation need to be a layer.
        // Imo a single non-player-collision layer with proper logic in script should do the trick,
        //  as these layers need to do logic anyway. "Trigger" would be a good name for these.
        
        // Please comment when and why those layers are necessary.
        
        Default = 0,
        TransparentFX = 1,
        Ignore_Raycast = 2,
        Water = 4,
        UI = 5,
        PlayerIFrame = 7, //Player with no collision with most stuff.
        Player = 8, //player layer, baseline for most logic.
        Enemy = 10, //enemy layer, usually hurts player.
        Wall = 12, //basic collision layer.
        Trigger = 13, // TODO placeholder for now, essentially catch for all "on player enter do something"
        EnemyNavigation = 16, //used for enemy navigation, seems like it could be a "Trigger" layer.
        Interactables = 17, // seems like it could be a "Trigger" layer.
        Hazard = 18, // something like static enemy. Main difference is not being pass through on IFrame compared to Enemy
        SafeGroundCheckpoint = 19, //no clue
        PlayerAttack = 20 //used for handling player attacks. All of them, spells or weapon swings alike.
    }
}