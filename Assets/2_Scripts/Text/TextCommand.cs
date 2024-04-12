using System;
using UnityEngine;

[Serializable]
public class TextCommand : MonoBehaviour // TODO in case i left it as monobehaviour - it really does not need to be one, but debugging in inspector was easier with it that way.
    //If it still is mono behaviour then remove, make it so fields have proper accessors and it will work fine.
{
    public TextCommandType type;
    public int indexStart;
    public int indexEnd;
    public float magnitude; //for shake variance, wave length, pulse length or gradient length?
}
