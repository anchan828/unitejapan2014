using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace MultiSpriteEditor
{
    public class MultiSpriteEditorWindow : ScriptableWizard
    {

		[SerializeField] Vector2 pixelSize = new Vector2(1, 1);
		[SerializeField] Vector2 offset = Vector2.zero;
		[SerializeField] Vector2 padding = Vector2.zero;
		[SerializeField] SpritePivot pivot = SpritePivot.Center;

		[PreviewTexture, SerializeField] Texture2D[] preview = new Texture2D[0];


		private Texture2D[] m_selectedTextures{
			get{
				return Selection.objects.Where(t => t.GetType() == typeof(Texture2D)).Cast<Texture2D>().ToArray();
			}
		}

        [MenuItem("Window/MultiSpriteEditor")]
        public static void Open()
        {
            DisplayWizard<MultiSpriteEditorWindow>("MultiSpriteEditor", "Slice");
        }

		void OnEnable ()
		{
			preview = m_selectedTextures;
		}

		void OnSelectionChange ()
		{
			preview = m_selectedTextures;
			isValid = preview.Length != 0;
			Repaint();
		}

        void OnWizardCreate()
        {
            DoSlicing();
        }

		public void DoSlicing()
        {
			var _textures = m_selectedTextures;
			for (var i = 0; i < _textures.Length; i++)
            {
				var tex = _textures[i];
                var path = AssetDatabase.GetAssetPath(tex);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
				var isReadable = importer.isReadable;
                importer.isReadable = true;
                EditorUtility.SetDirty(importer);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                var rects = GenerateGridSpriteRectangles(tex);
				EditorUtility.DisplayProgressBar(tex.name, string.Format("{0}/{1}", i, _textures.Length),
				                                 (float)i / (float)_textures.Length);

				importer.isReadable = isReadable;

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