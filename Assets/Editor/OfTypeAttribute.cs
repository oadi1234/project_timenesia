using System;
using UnityEngine;
//Taken from unity forums: https://discussions.unity.com/t/reference-interfaces-in-editor-workaround/896967/3
// thank you CaseyHofland

/// <summary>
/// Specifies types that an Object needs to be of. Can be used to create an Object selector that allows interfaces.
/// </summary>
public class OfTypeAttribute : PropertyAttribute
{
    public Type[] types;

    public OfTypeAttribute(Type type)
    {
        this.types = new Type[] { type };
    }

    public OfTypeAttribute(params Type[] types)
    {
        this.types = types;
    }
}