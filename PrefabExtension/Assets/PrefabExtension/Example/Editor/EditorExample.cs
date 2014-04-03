using UnityEditor;
using System.Linq;

public class EditorExample
{
	[MenuItem("PrefabExtension/Apply Prefab State")]
	static void ApplyPrefabState ()
	{
		Selection.gameObjects.ApplyPrefabState (false);
	}

	[MenuItem("PrefabExtension/Apply Prefab State", true)]
	static bool ValidateApplyPrefabState ()
	{
		return Selection.gameObjects.All (prefabInstance => prefabInstance.IsPrefabInstance ());
	}
}