using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace MultiSpriteEditor
{
    public class MultiSpriteEditorWindow : ScriptableWizard
    {
        public Vector2 pixelSize = new Vector2(1, 1);
        public Vector2 offset = Vector2.zero;
        public Vector2 padding = Vector2.zero;
        public SpritePivot pivot = SpritePivot.Center;

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
                var rects = GenerateGridSpriteRectangles(tex);
                EditorUtility.DisplayProgressBar(tex.name, string.Format("{0}/{1}", i, textures.Length),
                    (float)i / (float)textures.Length);

                importer.isReadable = cache;

                importer.spritesheet = rects.Select((t, j) => new SpriteMetaData
                {
                    alignment = (int)pivot,
                    pivot = Vector2.one * 0.5f,
                    rect = t,
                    name = string.Format("{0}_{1}", tex.name, j)
                }).ToArray();

                EditorUtility.SetDirty(importer);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            }
            EditorUtility.DisplayProgressBar("", "Done.", 1);
            EditorUtility.ClearProgressBar();
        }


        public Rect[] GenerateGridSpriteRectangles(Texture2D texture)
        {
            var rects = new List<Rect>();
            for (var y = (int)offset.y; y + (int)pixelSize.y <= texture.height; y += (int)pixelSize.y)
            {
                for (var x = (int)offset.x; x + (int)pixelSize.x <= texture.width; x += (int)pixelSize.x)
                {

                    var rect = new Rect(x, texture.height - y - pixelSize.y, pixelSize.x, pixelSize.y);

                    if (IsAlphaTexture(texture, rect) == false)
                    {
                        rects.Add(rect);
                    }

                    x += (int)padding.x;
                }

                y += (int)padding.y;
            }

            return rects.ToArray();
        }

        private bool IsAlphaTexture(Texture2D texture, Rect rect)
        {
            var pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            var alphaPixels = pixels.Where(p => p.a == 0).ToArray();
            return pixels.Length == alphaPixels.Length;
        }
    }

    public enum SpritePivot
    {
        Center = 0,
        TopLeft,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }
}