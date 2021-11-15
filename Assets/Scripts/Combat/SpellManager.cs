using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * How it works:
 * Basically player has mana. One point of mana is recharged every second (or half a second?)
 * spell of 1st tier (so a single element) uses 1 mana
 * spell of 2nd tier uses 2
 * etc.
 * 
 * Spells have aforementioned elements. Elements are something like [fire], [water]
 * etc.
 * You can combine them into a set of elements (so the order does not matter).
 * A single element can be used multiple times, [fire, fire] is a perfectly valid spell.
 * 
 * You also have spell notches - it's the maximum amount of spell elements
 * you can put into a single set. so with capacity of 1, only mana at a time can be used
 * with capacity of 3, 3 mana at a time (so 3 elements) can be used
 * 
 * A full set of spell elements form a spell. Need to have scriptable object to store the 
 * spell formulas.
 * 
 * Player can start with 5 mana and 2 notches maybe.
 * 
 * 
 * Elements idea:
 * [force] - default or early on. Used to cast shields, shields give mana on discharge.
 * [fire] - default. Can set some thing aflame maybe? Can be used for self damage. 
 * [water] - default. Can douse flames, or combined with force shield from them.
 * [air] - unlockable. Spells above 1 level might be cast mid-air.
 * [lightning] - unlockable. Possibly an alternative to fire in terms of damage dealing
 * [corrupt] - corrupt power like in axiom verge maybe. Twist the whole spell.
 * [holy] - unobtainable by player, maybe for second character. Actually heals.
 * [???/time] - default as ??? but does nothing. Later on actually might be used for story stuff.
 * [earth] - no clue. possibly npc only. Alternative shielding that does not grant mana maybe.
 * 
 * Advanced elements - could be a direct upgrade for some elements. ie:
 * [sun/light/blaze] - upgrade to the fire element
 * [ice] - upgrade to the water element
 * [void] - upgrade to the corrupt element
 * etc. It might be stuff from temporary passives (like charms in hollow knight).
 * Advanced elements would probably be just upgrades to spell effectiveness.
 */
public class SpellManager : MonoBehaviour
{
    [SerializeField]
    private int spellCapacity = 2; //2 - default


}
