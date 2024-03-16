using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum AbilityName
{
    DOUBLE_JUMP,
    DASH,
    LONG_DASH,
    SPATIAL_DASH,
    TIME_GATE, //save anywhere
    WALL_JUMP,
    SWIM_UNDERWATER,
    SLOWMOTION_FOCUS, //debug only? Might not be fun mid actual game.
    MIDAIR_FOCUS // might be deleted later or turned baseline ability.
}
