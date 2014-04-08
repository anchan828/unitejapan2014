using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(Prefab))]
public class PrefabDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		Object obj = null;

		var gameObjectProperty = property.FindPropertyRelative ("_gameObject");

		if (gameObjectProperty != null) {
			obj = gameObjectProperty.objectReferenceValue;
		}

		EditorGUI.BeginChangeCheck ();

		var prefab = EditorGUI.ObjectField (position, label, obj, typeof(GameObject), true) as GameObject;

		if (EditorGUI.EndChangeCheck ()) {

			if (prefab != null) {
				PrefabType prefabType = PrefabUtility.GetPrefabType (prefab);

				if (prefabType == PrefabType.PrefabInstance) {

					prefab = PrefabUtility.GetPrefabParent (prefab) as GameObject;
				
				} else if (prefabType != PrefabType.Prefab) {
				
					Debug.LogError ("This GameObject is not a Prefab.");
					return;
				}
			}

			if (gameObjectProperty != null) {
				gameObjectProperty.objectReferenceValue = prefab;
			}
		}
	}
}