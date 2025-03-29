using System.Linq;
using _2_Scripts.Unity.InternalBridge;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
//Taken from CaseyHofland on unity forums: https://discussions.unity.com/t/reference-interfaces-in-editor-workaround/896967
// Thanks a lot.

namespace _2_Scripts.Unity
{
    [CustomPropertyDrawer(typeof(OfTypeAttribute))]
    public class OfTypeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Check if the property type is of an object reference.
            if (property.propertyType != SerializedPropertyType.ObjectReference
                && property.propertyType != SerializedPropertyType.ExposedReference)
            {
                throw new System.ArgumentException(
                    "This attribute is not supported on properties of this property type.",
                    nameof(property.propertyType));
            }

            // Set up the type variables.
            InternalEditorBridge.GetFieldInfoFromProperty(property, out var type); // INTERNAL UNITY FUNCTION
            var ofType = attribute as OfTypeAttribute;

            // Set up the object field.
            var objectField = new ObjectField(property.displayName);
            objectField.AddToClassList(ObjectField.alignedFieldUssClassName);
            objectField.BindProperty(property);
            objectField.objectType = type;

            // Disable dropping if not assignable from drag and drop.
            objectField.RegisterCallback<DragUpdatedEvent>(dragUpdated =>
            {
                if (!DragAndDrop.objectReferences.Any(obj
                        => obj is GameObject gameObject ? FirstValidOrDefault(gameObject) : IsValid(obj)))
                {
                    dragUpdated.PreventDefault();
                }
            });

            // Assign the appropriate value.
            objectField.RegisterValueChangedCallback(changed =>
            {
                if (IsValid(changed.newValue))
                {
                    return;
                }
                if (changed.newValue is GameObject gameObject
                         || changed.newValue is Component component && (gameObject = component.gameObject))
                {
                    objectField.value = FirstValidOrDefault(gameObject);
                }
                else
                {
                    objectField.value = null;
                }
            });

            return objectField;

            // Helper methods.
            bool IsValid(Object obj) => !obj || type.IsAssignableFrom(obj.GetType()) &&
                ofType.types.All(type => type.IsAssignableFrom(obj.GetType()));

            Component FirstValidOrDefault(GameObject gameObject) =>
                gameObject.GetComponents<Component>().FirstOrDefault(IsValid);
        }
    }
}