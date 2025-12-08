using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevUlts.Assistants.DictionarySerialize.Editor
{
    [CustomPropertyDrawer(typeof(Dict<,>))]
    public class DictEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty listProperty = property.FindPropertyRelative("list");
            EditorGUI.PropertyField(position, listProperty, label, true);
            if (listProperty.isExpanded)
            {
                CheckForDuplicateKeys(listProperty, position); // Check for duplicate keys
            }

            DisplayHelpBoxIfNeeded(listProperty, position, property, label); // Display help box if needed

            EditorGUI.EndProperty();
        }

        private void CheckForDuplicateKeys(SerializedProperty listProperty, Rect position)
        {
            var img = (Texture2D)AssetDatabase.LoadMainAssetAtPath(
                "Assets/GameTool/Assistants/Textures/d_iconconflictedoverlay.png");

            List<object> keySet = new List<object>();
            SerializedProperty arraySizeProp = listProperty.FindPropertyRelative("Array.size");

            for (int i = 0; i < arraySizeProp.intValue; i++)
            {
                SerializedProperty elementProp = listProperty.GetArrayElementAtIndex(i);
                SerializedProperty keyProp = elementProp.FindPropertyRelative("key");
                var key = GetPropertyValue(keyProp);

                keySet.Add(key);
            }

            var y = EditorGUIUtility.singleLineHeight + 8;
            for (int i = 0; i < arraySizeProp.intValue; i++)
            {
                SerializedProperty elementProp = listProperty.GetArrayElementAtIndex(i);
                SerializedProperty keyProp = elementProp.FindPropertyRelative("key");
                var key = GetPropertyValue(keyProp);

                if (keySet.IndexOf(key) != keySet.LastIndexOf(key))
                {
                    EditorGUI.DrawPreviewTexture(
                        new Rect(
                            position.position + new Vector2(-16,
                                y),
                            new Vector2(16, 16)), img);
                }

                y += EditorGUI.GetPropertyHeight(elementProp) + 2;
            }
        }

        private void DisplayHelpBoxIfNeeded(SerializedProperty listProperty, Rect position, SerializedProperty property,
            GUIContent label)
        {
            var img = (Texture2D)AssetDatabase.LoadMainAssetAtPath(
                "Assets/GameTool/Assistants/Textures/d_iconconflictedoverlay.png");

            if (HasDuplicateKeys(listProperty))
            {
                EditorGUI.HelpBox(
                    new Rect(
                        position.position + new Vector2(0,
                            GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight * 2f),
                        new Vector2(position.width, EditorGUIUtility.singleLineHeight * 2f)),
                    "Duplicate keys detected!", MessageType.Error);
                EditorGUI.DrawPreviewTexture(
                    new Rect(
                        position.position + new Vector2(position.width - EditorGUIUtility.singleLineHeight * 4,
                            1),
                        new Vector2(16, 16)), img);
            }
        }

        private bool HasDuplicateKeys(SerializedProperty listProperty)
        {
            HashSet<object> keySet = new HashSet<object>();
            SerializedProperty arraySizeProp = listProperty.FindPropertyRelative("Array.size");

            for (int i = 0; i < arraySizeProp.intValue; i++)
            {
                SerializedProperty elementProp = listProperty.GetArrayElementAtIndex(i);
                SerializedProperty keyProp = elementProp.FindPropertyRelative("key");
                var key = GetPropertyValue(keyProp);

                if (keySet.Contains(key))
                {
                    return true;
                }
                else
                {
                    keySet.Add(key);
                }
            }

            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("list"), label, true);

            if (HasDuplicateKeys(property.FindPropertyRelative("list")))
            {
                height += EditorGUIUtility.singleLineHeight * 2.5f;
            }

            return height;
        }

        private object GetPropertyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex;
                case SerializedPropertyType.LayerMask:
                    return property.intValue;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.Rect:
                    return property.rectValue;
                case SerializedPropertyType.Character:
                    return (char)property.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue;
                // case SerializedPropertyType.Gradient: // Unity 2021.1+
                //     return property.gradientValue;
                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return property.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return property.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return property.boundsIntValue;
                case SerializedPropertyType.FixedBufferSize: // Unity 2021.2+
                    return property.fixedBufferSize;
                case SerializedPropertyType.ManagedReference: // Unity 2022.1+
                    return property.objectReferenceValue;
                // Add more cases for other types as needed
                default:
#if UNITY_2022_1_OR_NEWER
                    return property.boxedValue;
#else
                    return null;
#endif
            }
        }
    }
}