using System;
using System.Collections.Generic;
using _2_Scripts.Model;
using _2_Scripts.Player;
using _2_Scripts.UI.Elements.HUD;

namespace _2_Scripts.Global.SaveSystem.SaveDataSchemas
{
    [Serializable]
    public class GameData
    {
        public int MaxHealth;
        public int MaxEffort;
        public int SpellCapacity;
        public int Coins { get; set; }
        public string gameVersion { get; set; }
        
        public GameData()
        {
        }

        //for json magic
        public GameData(GameData gameState)
        {
            Coins = gameState.Coins;
            gameVersion = gameState.gameVersion;
            // Abilities = gameState.Abilities;
            AlteredObjects = gameState.AlteredObjects;
            MaxHealth = gameState.MaxHealth;
            MaxEffort = gameState.MaxEffort;
            SpellCapacity = gameState.SpellCapacity;
            Coins = gameState.Coins;
        }

        #region ABILITIES

        public Dictionary<AbilityName, bool> Abilities =
            new();
        // { //TODO: why exception? o.O
        //     {AbilityName.DoubleJump, false},
        //     {AbilityName.Dash, false},
        //     {AbilityName.LongDash, false},
        //     {AbilityName.SpatialDash, false},
        //     {AbilityName.TimeGate, false},
        //     {AbilityName.WallJump, false},
        //     {AbilityName.SwimUnderwater, false},
        //     {AbilityName.SlowMotionFocus, false},
        //     {AbilityName.MidairFocus, false},
        //     {AbilityName.ChargedSpell, false},
        //     {AbilityName.DoubleJump, false}
        // };
        #endregion

        #region INACTIVE_OBJECTS
        public Dictionary<string, bool> AlteredObjects = new(); //only objects which should be inactivated across save games should land here.
        #endregion

        #region CONTAINERS
        // TODO fill when containers are implemented
        #endregion

        #region BOSSES
        // TODO fill when bosses are implemented
        #endregion

        #region SHORTCUTS
        // TODO fill when shortcuts are implemented.
        #endregion

        #region QUEST_FLAGS
        // TODO
        #endregion
    }
}
