using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Fuga))]
public class FugaDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.width = position.height = 64;
        property.FindPropertyRelative("texture").objectReferenceValue = EditorGUI.ObjectField(position, property.FindPropertyRelative("texture").objectReferenceValue, typeof(Texture2D), false);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 64;
    }
}
