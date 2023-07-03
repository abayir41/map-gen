using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MapGen.Utilities
{
    

    public class OnChangedCallAttribute : PropertyAttribute
    {
        public string methodName;
        public OnChangedCallAttribute(string methodNameNoArguments)
        {
            methodName = methodNameNoArguments;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(OnChangedCallAttribute))]
    public class OnChangedCallAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (!EditorGUI.EndChangeCheck()) return;
            
            var at = attribute as OnChangedCallAttribute;
            var method = property.serializedObject.targetObject.GetType().GetMethods().First(m => at != null && m.Name == at.methodName);

            if (!method.GetParameters().Any())// Only instantiate methods with 0 parameters
                method.Invoke(property.serializedObject.targetObject, null);
        }
    }

#endif
}