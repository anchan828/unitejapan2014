using UnityEditor;
using UnityEngine;
using System.Collections;
[CustomPropertyDrawer(typeof(Hoge))]
public class HogeDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.y += 2;

        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty nameProperty = property.FindPropertyRelative("name");
        SerializedProperty fugaProperty = property.FindPropertyRelative("age");

        position.height = 17;
        position.width *= 0.5f;
        EditorGUIUtility.LookLikeControls(50);
        EditorGUI.PropertyField(position, nameProperty);

        position.x += position.width;


        EditorGUI.PropertyField(position, fugaProperty);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 21;
    }
}
