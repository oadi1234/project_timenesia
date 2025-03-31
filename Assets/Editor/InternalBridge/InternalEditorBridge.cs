using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Editor.InternalBridge
{
#if UNITY_EDITOR
    internal static class InternalEditorBridge
    {
        public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type) =>
            ScriptAttributeUtility.GetFieldInfoFromProperty(property, out type);
    }
#endif
}