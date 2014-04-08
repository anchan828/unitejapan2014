using UnityEditor;
using UnityEngine;

public class ScrollView : EditorWindow
{
	[MenuItem("Window/ScrollView")]
	private static void Open ()
	{
		GetWindow<ScrollView> ();
	}

	private Vector2 pos = Vector2.zero;
	private bool on = false;

	void OnGUI ()
	{
		on = EditorGUILayout.ToggleLeft ("見えてないところを消す", on);


		EditorGUILayout.BeginHorizontal (EditorStyles.toolbar);

		EditorGUILayout.LabelField ("Texture", "Size");

		EditorGUILayout.EndHorizontal ();
		pos = EditorGUILayout.BeginScrollView (pos);
		var num = 3000;

		if (on) {
			var lineHight = 16 + 2;

			var visibleElement = new
            {
                min = Mathf.FloorToInt ((pos.y / lineHight)),
                max = Mathf.FloorToInt ((pos.y + position.height) / lineHight) -1
            };

			if (visibleElement.min != 0 && Event.current.type != EventType.Repaint) {
				GUILayout.Space (lineHight * visibleElement.min);
			}

			for (var i = visibleElement.min; i < visibleElement.max; i++) {
				if (num != i)
					EditorGUILayout.LabelField (new GUIContent ("texture " + i, EditorGUIUtility.whiteTexture), new GUIContent(EditorUtility.FormatBytes (i * 1024)));
			}

			if (num != visibleElement.max && Event.current.type != EventType.Repaint) {
				GUILayout.Space (lineHight * (num - Mathf.Clamp (visibleElement.max, 0, num)));
			}
		} else {
			for (int i = 0; i < num; i++) {
				EditorGUILayout.LabelField (new GUIContent ("texture " + i, EditorGUIUtility.whiteTexture), new GUIContent(EditorUtility.FormatBytes (i * 1024)));
			}
		}
		EditorGUILayout.EndScrollView ();


	}
}