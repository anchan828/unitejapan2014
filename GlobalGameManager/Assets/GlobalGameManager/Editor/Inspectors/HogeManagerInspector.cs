using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(HogeManager))]
public class HogeManagerInspector : GlobalGameManagerInspector
{
    [MenuItem("Edit/Project Settings/HogeManager")]
    private static void ShowManagerSettings()
    {
        Show<HogeManager>();
    }
}