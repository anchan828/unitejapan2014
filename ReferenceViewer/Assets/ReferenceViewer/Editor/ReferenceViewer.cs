using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace ReferenceViewer
{
    public class ReferenceViewer : EditorWindow
    {

        private List<Item> hitItems = new List<Item>();
        private Vector2 pos = Vector2.zero;

        [MenuItem("Window/ReferenceViewer")]
        private static void Open()
        {
            GetWindow<ReferenceViewer>();
        }

        [MenuItem("Assets/Find References In Project", true)]
        private static bool FindValidate()
        {
            return Selection.objects.Length != 0;
        }

        [MenuItem("Assets/Find References In Project")]
        private static void Find()
        {
            var text = File.ReadAllText("build/ReferenceViewer/data.json");
            var data = LitJson.JsonMapper.ToObject<Data>(text);
            var items = (Selection.objects.Select(obj => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)))
                .Select(guid => new
                {
                    searched = GetGUIContent(guid),
                    referenced =
                        data.assetData.Where(assetData => assetData.reference.Contains(guid))
                            .Select(assetData => GetGUIContent(assetData.guid))
                            .Where(c => c.image)
                            .OrderBy(c => c.image.name)
                            .ToList(),
                    reference =
                        data.assetData.Find(item => item.guid == guid)
                                .reference.Select(g => GetGUIContent(g))
                                .Where(c => c.image)
                                .OrderBy(c => c.image.name)
                                .ToList()
                })
                .Where(item => (item.referenced.Count != 0 || item.reference.Count != 0) && item.searched.image)
                .OrderBy(item => item.searched.image.name)
                .Select(item => new Item
                {
                    searchedGUIContent = item.searched,
                    referencedGUIContents = item.referenced,
                    referenceGUIContents = item.reference
                })).ToList();

            GetWindow<ReferenceViewer>().Results(items);
        }

        private void Results(List<Item> items)
        {
            hitItems = items;
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Create Json", EditorStyles.toolbarButton))
            {
                JsonCreator.Build();
            }


            if (hitItems.Count == 0)
            {
                EditorGUILayout.LabelField("Not Found");
            }
            else
            {
                pos = EditorGUILayout.BeginScrollView(pos);

                foreach (var hitItem in hitItems)
                {

                    EditorGUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width * 0.98f));
                    DrawGUIContents(hitItem.referenceGUIContents);
                    var iconSize = EditorGUIUtility.GetIconSize();
                    EditorGUIUtility.SetIconSize(Vector2.one * 32);
                    GUILayout.Label(hitItem.searchedGUIContent, GUILayout.Width(Screen.width * 0.3f), GUILayout.ExpandWidth(false));
                    EditorGUIUtility.SetIconSize(iconSize);
                    PingObjectIfOnMouseDown(hitItem.searchedGUIContent.tooltip);

                    DrawGUIContents(hitItem.referencedGUIContents);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndScrollView();
            }


        }

        private static void DrawGUIContents(List<GUIContent> contents)
        {
            if (contents.Count != 0)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.3f));

                foreach (var content in contents)
                {
                    EditorGUILayout.LabelField(content, GUILayout.Width(Screen.width * 0.3f), GUILayout.ExpandWidth(true));

                    PingObjectIfOnMouseDown(content.tooltip);
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                GUILayout.Space(Screen.width * 0.3f + 16);
                //                GUILayout.Box("", GUILayout.Width(Screen.width * 0.3f), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }



        }
        private static void PingObjectIfOnMouseDown(string path)
        {
            if (Event.current.type != EventType.MouseDown) return;
            if (!GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) return;

            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

            EditorGUIUtility.PingObject(obj);
        }

        private static GUIContent GetGUIContent(string guidOrAssetPath)
        {
            var assetPath = File.Exists(guidOrAssetPath) ? guidOrAssetPath : AssetDatabase.GUIDToAssetPath(guidOrAssetPath);

            var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

            return GetGUIContent(asset);
        }

        private static GUIContent GetGUIContent(Object obj)
        {
            var content = new GUIContent(EditorGUIUtility.ObjectContent(obj, obj.GetType()));

            var type = PrefabUtility.GetPrefabType(obj);

            if (type == PrefabType.Prefab)
            {
                var icon = EditorGUIUtility.Load("Icons/Generated/PrefabNormal Icon.asset") as Texture2D;
                content.image = icon;
            }

            content.tooltip = AssetDatabase.GetAssetPath(obj);

            return content;
        }

        private class Item
        {
            public GUIContent searchedGUIContent;
            public List<GUIContent> referencedGUIContents = new List<GUIContent>();
            public List<GUIContent> referenceGUIContents = new List<GUIContent>();
        }
    }
}