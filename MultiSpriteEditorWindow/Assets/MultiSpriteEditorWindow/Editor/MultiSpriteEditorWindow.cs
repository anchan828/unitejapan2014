using UnityEngine;
using UnityEditor;
using System.Linq;

namespace MultiSpriteEditor
{
    public class MultiSpriteEditorWindow : ScriptableWizard
    {
        public GridSprite gridSprite = new GridSprite();

        [MenuItem("Window/MultiSpriteEditor")]
        public static void Open()
        {
            DisplayWizard<MultiSpriteEditorWindow>("MultiSpriteEditor", "Slice");
        }

        private void OnWizardCreate()
        {
            DoSlicing();
        }

        public void DoSlicing()
        {

            var textures = Selection.objects.Where(t => t.GetType() == typeof(Texture2D)).Cast<Texture2D>().ToArray();

            for (var i = 0; i < textures.Length; i++)
            {
                var tex = textures[i];
                var path = AssetDatabase.GetAssetPath(tex);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                var cache = importer.isReadable;
                importer.isReadable = true;
                EditorUtility.SetDirty(importer);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                var rects = gridSprite.GenerateGridSpriteRectangles(tex);
                EditorUtility.DisplayProgressBar(tex.name, string.Format("{0}/{1}", i, textures.Length),
                    (float)i / (float)textures.Length);

                importer.isReadable = cache;

                importer.spritesheet = rects.Select((t, j) => new SpriteMetaData
                {
                    alignment = gridSprite.GetPivot(),
                    pivot = Vector2.one * 0.5f,
                    rect = t,
                    name = tex.name + " " + j
                }).ToArray();

                EditorUtility.SetDirty(importer);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            }
            EditorUtility.DisplayProgressBar("", "Done.", 1);
            EditorUtility.ClearProgressBar();
        }
    }
}