using UnityEditor;
using UnityEngine;

public class ScrollView : EditorWindow
{
    [MenuItem("Window/ScrollView")]
    private static void Open()
    {
        GetWindow<ScrollView>();
    }

    private Vector2 pos = Vector2.zero;
    private bool on = false;
    void OnGUI()
    {
        on = EditorGUILayout.ToggleLeft("見えてないところを消す", on);

        pos = GUILayout.BeginScrollView(pos);
        var num = 3000;

        if (on)
        {
            var lineHight = 16 + 2;

            var visibleElement = new
            {
                min = Mathf.FloorToInt((pos.y / lineHight)),
                max = Mathf.FloorToInt((pos.y + position.height) / lineHight)
            };

            if (visibleElement.min != 0 && Event.current.type != EventType.Repaint)
            {
                GUILayout.Space(lineHight*visibleElement.min);
            }

            for (var i = visibleElement.min; i < visibleElement.max; i++)
            {
                if (num != i)
                    GUILayout.Label("GUIElement " + i);
            }

            if (num != visibleElement.max && Event.current.type != EventType.Repaint)
            {
                GUILayout.Space(lineHight*(num - Mathf.Clamp(visibleElement.max, 0, num)));
            }
        }
        else
        {
            for (int i = 0; i < num; i++)
            {
                GUILayout.Label("GUIElement " + i);
            }
        }
        GUILayout.EndScrollView();
    }
}