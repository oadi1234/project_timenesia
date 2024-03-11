using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerAbilityManager", order = 1)]
public class PlayerAbilityManager : ScriptableObject
{
    #region FLAGS
    public bool hasDoubleJump;
    public bool hasDash;
    public bool hasLongDash;
    public bool hasSpatialDash; //might need to save Dash and Spatial Dash as an enum, not bool, for value compression sake
    public bool timeGate; //save anywhere
    public bool hasWallJump;
    public bool underwaterSwim;
    public bool focusSlowmotion;
    public bool midairFocus; // might be deleted later and turned baseline ability.
    //public bool teleportCircle; // move to other teleportation circle, might be a spell?
    #endregion

    //double jump

    //wall jump

    //dash

    // long dash - increase length and of dash by every effort point spent. It increments in its usage, granting the same distance per point but simply going faster to avoid time wasting.

    //assigned spellcasting

    //free spellcasting

    //swim underwater

    //save anywhere

    //gate(similar to dream gate from hollow knight?)

    //environmental protection

    //[T]short teleport?

    //[T] some crushing (walls)

    //[T] digging? (diggy diggy hole)
}
