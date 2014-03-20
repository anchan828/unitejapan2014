using GlobalGameManager;
using UnityEditor;
using UnityEngine;
public class GlobalGameManagerInspector : Editor
{

    public static void Show<T>() where T : GlobalGameManagerObject
    {
        var path = string.Format("GlobalGameManager/{0}", typeof(T).Name);
        var manager = Resources.Load<T>(path);

        if (manager == null)
        {
            manager = Creator.GlobalGameManager<T>();
        }

        Selection.activeObject = manager;
    }
}
