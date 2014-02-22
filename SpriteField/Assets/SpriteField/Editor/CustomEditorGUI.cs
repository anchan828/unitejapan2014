using System.Linq;
using UnityEditor;
using UnityEngine;

public class CustomEditorGUI
{
    static GUIStyle spriteStyle = new GUIStyle(EditorStyles.label);
    public static Sprite SpriteField(Rect rect, Sprite sprite)
    {
        var id = GUIUtility.GetControlID(FocusType.Native, rect);

        if (Event.current.type == EventType.Repaint)
        {
            EditorStyles.objectFieldThumb.Draw(rect, GUIContent.none, id, DragAndDrop.activeControlID == id);

            if (sprite)
            {
                var spriteTexture = AssetPreview.GetAssetPreview(sprite);

                if (spriteTexture)
                {
                    spriteStyle.normal.background = spriteTexture;
                    spriteStyle.Draw(rect, false, false, false, false);
                }
                else
                {
                    Resources.FindObjectsOfTypeAll<EditorWindow>().ToList().ForEach(w => w.Repaint());
                }
            }
        }

        var buttonRect = new Rect(rect);
        buttonRect.x += buttonRect.width * 0.5f;
        buttonRect.width *= 0.5f;
        buttonRect.y += rect.height - 16;
        buttonRect.height = 16;

        if (Event.current.commandName == "ObjectSelectorUpdated")
        {
            sprite = EditorGUIUtility.GetObjectPickerObject() as Sprite;
            HandleUtility.Repaint();
        }
        if (rect.Contains(Event.current.mousePosition))
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (DragAndDrop.objectReferences.Length == 1)
                        DragAndDrop.AcceptDrag();
                    DragAndDrop.activeControlID = id;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    break;
                case EventType.DragExited:
                    if (DragAndDrop.objectReferences.Length == 1)
                    {
                        var reference = DragAndDrop.objectReferences[0] as Sprite;
                        if (reference != null)
                        {
                            sprite = reference;
                            HandleUtility.Repaint();
                        }
                    }
                    break;
            }
        }
        if (GUI.Button(buttonRect, "select", EditorStyles.objectFieldThumb.name + "Overlay2"))
        {
            EditorGUIUtility.ShowObjectPicker<Sprite>(sprite, false, "",
                GUIUtility.GetControlID(FocusType.Passive));
            GUIUtility.ExitGUI();
        }
        return sprite;
    }
}

public class CustomEditorGUILayout
{
    public static Sprite SpriteField(Sprite sprite, params GUILayoutOption[] options)
    {
        EditorGUILayout.LabelField("", "", options);
        var rect = GUILayoutUtility.GetLastRect();
        return CustomEditorGUI.SpriteField(rect, sprite);
    }
}