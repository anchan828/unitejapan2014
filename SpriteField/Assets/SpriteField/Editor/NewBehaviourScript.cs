using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : EditorWindow
{
    [MenuItem("Window/SpriteEditor")]
    static void Open()
    {
        GetWindow<NewBehaviourScript>();
    }

    private Sprite sprite1,sprite2;
    void OnGUI()
    {
        sprite1 = CustomEditorGUI.SpriteField(new Rect(134, 1, 128, 128), sprite1);
        sprite2 = CustomEditorGUILayout.SpriteField(sprite2, GUILayout.Width(128), GUILayout.Height(128));
    }
}
