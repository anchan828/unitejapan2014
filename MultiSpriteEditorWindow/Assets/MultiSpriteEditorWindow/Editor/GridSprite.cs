using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace MultiSpriteEditor
{
    [System.Serializable]
    public class GridSprite
    {
        [SerializeField]
        private Vector2 pixelSize = new Vector2(1, 1);
        [SerializeField]
        private Vector2 offset = Vector2.zero;
        [SerializeField]
        private Vector2 padding = Vector2.zero;
        [SerializeField]
        private SpritePivot pivot = SpritePivot.Center;

        public int GetPivot()
        {
            return (int)pivot;
        }

        public void OnGUI()
        {
            pixelSize = EditorGUILayout.Vector2Field("Pixel Size", pixelSize);
            offset = EditorGUILayout.Vector2Field("Offset", offset);
            padding = EditorGUILayout.Vector2Field("Padding", padding);
            pivot = (SpritePivot)EditorGUILayout.EnumPopup("Pivot", pivot);
        }

        public Rect[] GenerateGridSpriteRectangles(Texture2D texture)
        {
            var rects = new List<Rect>();
            for (var y = (int)offset.y; y + (int)pixelSize.y <= texture.height; y += (int)pixelSize.y)
            {
                for (var x = (int)offset.x; x + (int)pixelSize.x <= texture.width; x += (int)pixelSize.x)
                {

                    var rect = new Rect(
                        x,
                        texture.height - y - pixelSize.y,
                        pixelSize.x,
                        pixelSize.y
                        );


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

        private static bool IsAlphaTexture(Texture2D texture, Rect rect)
        {
            var pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            var alphaPixels = pixels.Where(p => p.a == 0).ToArray();
            return pixels.Length == alphaPixels.Length;
        }
    }
}