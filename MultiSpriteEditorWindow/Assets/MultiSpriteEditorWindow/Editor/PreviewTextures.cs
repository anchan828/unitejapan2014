using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomPropertyDrawer(typeof(PreviewTexture))]
public class PreviewTextureDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		int index;
		if (GetIndex (property, out index)) {

			position.width = position.height = 64;

			position.x += index * 70;

			if (index != 0)
				position.y -= 68;

			EditorGUI.DrawTextureTransparent (position, property.objectReferenceValue as Texture2D);

		}
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		int index;
		if (GetIndex (property, out index)) {
			return index == 0 ? 66 : -2;
		}


		return base.GetPropertyHeight (property, label);
	}

	bool GetIndex (SerializedProperty property, out int index)
	{

		var match = Regex.Match (property.propertyPath, "Array.data\\[(\\d+)\\]");
		index = 0;
		if (match.Success) {
			index = int.Parse (match.Groups [1].Value);
		}

		return match.Success;
	}

}

public class PreviewTexture : PropertyAttribute
{

}