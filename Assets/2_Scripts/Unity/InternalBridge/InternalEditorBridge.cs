using System;
using System.Reflection;
using UnityEditor;

namespace _2_Scripts.Unity.InternalBridge
{
    internal static class InternalEditorBridge
    {
        public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type) =>
            ScriptAttributeUtility.GetFieldInfoFromProperty(property, out type);
    }
}