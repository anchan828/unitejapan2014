using UnityEngine;
using UnityEditor;

public static class PrefabExtension
{
	public static void ApplyPrefabState (this GameObject prefabInstance, bool force = true)
	{
		var prefab = PrefabUtility.GetPrefabParent (prefabInstance) as GameObject;

		if (prefab == null) {
			Debug.LogException (new System.NullReferenceException ());
			return;
		}

		bool updatePrefab = true;

		if (force == false) {
			updatePrefab = EditorUtility.DisplayDialog ("Apply", "Do you want to change the Prefab?", "Yes", "No");
		}

		if (updatePrefab) {
			PrefabUtility.ReplacePrefab (prefabInstance, prefab, ReplacePrefabOptions.ConnectToPrefab);
		}
	}

	public static void ApplyPrefabState (this GameObject[] prefabInstances, bool force = true)
	{
		if (force == false) {
			if (EditorUtility.DisplayDialog ("Apply", "Do you want to change the Prefab?", "Yes", "No") == false) {
				return;
			}
		}

		foreach (var prefabInstance in prefabInstances) {
			ApplyPrefabState (prefabInstance, true);
		}
	}

	public static bool IsPrefabInstance (this GameObject prefabInstance)
	{
		return PrefabUtility.GetPrefabType (prefabInstance) == PrefabType.PrefabInstance;
	}
}