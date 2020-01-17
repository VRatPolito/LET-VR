using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace PrattiToolkit
{
    

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");

        EditorGUI.BeginProperty(position, label, sceneAssetProp);
        EditorGUI.PropertyField(position, sceneAssetProp, label);
        EditorGUI.EndProperty();
    }
}

}