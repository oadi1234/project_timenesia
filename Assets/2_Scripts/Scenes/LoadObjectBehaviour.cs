using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Class containing data on how should an object be handled when scene is loaded. Paired with scene name in game data manager */
public enum LoadObjectBehaviour
{
    UNCHANGED, //Load normally
    INACTIVE, //set inactive.
    DELETE, //remove after load. either inactive or deleted will be the finally used version of this.
    ALTERED, //set special state, i.e. a background object will be broken after attacking it
    SAVE_PERSIST //special state. Might either be inactive or altered, depending on game logic. Persists across saves, ie. chest will stay opened, or ability will be forever collected.
}
