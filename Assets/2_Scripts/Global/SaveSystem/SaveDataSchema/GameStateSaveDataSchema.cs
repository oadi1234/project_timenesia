using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameStateSaveDataSchema
{
    public GameStateSaveDataSchema() { }
    public GameStateSaveDataSchema(GameStateSaveDataSchema gameState)
    {
        Coins = gameState.Coins;
        gameVersion = gameState.gameVersion;
        abilities = gameState.abilities;
        alteredObjects = gameState.alteredObjects;
    }

    public int Coins { get; set; }
    public int gameVersion { get; set; }

    #region ABILITIES
    public Dictionary<AbilityName, bool> abilities = new Dictionary<AbilityName, bool>();
    #endregion

    #region INACTIVE_OBJECTS
    public Dictionary<string, bool> alteredObjects = new Dictionary<string, bool>(); //only objects which should be inactivated across save games should land here.
    #endregion

    #region SPELLS
    // TODO fill out when spells are implemented... in some way.
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
    #endregion
}
