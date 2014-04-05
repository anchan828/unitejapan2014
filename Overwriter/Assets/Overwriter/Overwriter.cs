using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Exception = Overwriter.Extension;
using System.Text.RegularExpressions;
using System.Linq;

namespace Overwriter
{
    #region Preference
    public class Preference
    {
        static readonly string key = "Overwriter";
        static bool isEnabled;
        static Extension ex;
        static Overwriter overwriter = new Overwriter();

        static GUILayoutOption width
        {
            get
            {
                return GUILayout.Width(Screen.width * 0.25f);
            }

        }

        [PreferenceItem("Overwriter")]
        static void PreferenceOnGUI()
        {

            if (!isEnabled)
            {
                isEnabled = true;
                ex = new Exception();
                RefreshExtensions();
            }

            EditorGUI.BeginChangeCheck();
			overwriter.all = EditorGUILayout.ToggleLeft("すべての拡張子", overwriter.all);

            EditorGUI.BeginDisabledGroup(overwriter.all);

            EditorGUILayout.BeginHorizontal(width);
            ex.extension = EditorGUILayout.TextField(ex.extension);
            if (GUILayout.Button("+") && !string.IsNullOrEmpty(ex.extension))
            {
                AddExtension();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 46;
            for (int i = 0; i < overwriter.extensions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(width, GUILayout.ExpandWidth(false));

                var extension = overwriter.extensions[i];

                DrawExtension(extension);

                if (GUILayout.Button("-"))
                {
                    overwriter.extensions.Remove(overwriter.extensions[i]);
                    UpdateExtensions();
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();

			if (EditorGUI.EndChangeCheck())
			{
				UpdateExtensions();
			}
        }
        
        private static void AddExtension()
        {
            if (!ex.extension.StartsWith("."))
            {
                ex.extension = "." + ex.extension;
            }
            overwriter.extensions.Insert(0, ex);
            ex = new Exception();
            GUI.FocusControl("");
            UpdateExtensions();
        }

        static void UpdateExtensions()
        {
            var bytes = ObjectToByteArray(overwriter);
            EditorUserSettings.SetConfigValue(key, Convert.ToBase64String(bytes));
            RefreshExtensions();
        }

        static void RefreshExtensions()
        {
            overwriter = GetOverwriter(EditorUserSettings.GetConfigValue(key));
        }

        static void DrawExtension(Extension extension)
        {
            extension.enabled = EditorGUILayout.ToggleLeft(extension.extension, extension.enabled);
        }

        public static Overwriter GetOverwriter()
        {
            return GetOverwriter(EditorUserSettings.GetConfigValue(key));
        }

        static Overwriter GetOverwriter(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new Overwriter();
            }

            byte[] bytes = Convert.FromBase64String(value);

            return ByteArrayToObject<Overwriter>(bytes) ?? new Overwriter();
        }

        static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            T obj;
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                obj = (T)binForm.Deserialize(memStream);
            }

            return obj;
        }
    }
    #endregion
    #region Overwriter
    [Serializable]
    public class Overwriter
    {
        public bool all;
        public List<Extension> extensions = new List<Exception>();
    }

    [Serializable]
    public class Extension
    {
        public string extension;
        public bool enabled = true;

		public override string ToString ()
		{
			return string.Format ("[Extension] {0} , {1}",extension,enabled);
		}
    }
    #endregion
    #region Importer
    public class AssetPostprocessor : UnityEditor.AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
        {
            var overwriter = Preference.GetOverwriter();
          
            foreach (var assetPath in importedAssets)
            {
                var extension = Path.GetExtension(assetPath);
                
                if (!overwriter.all && !overwriter.extensions
                    .Where(ex => ex.enabled)
                    .Select(ex => ex.extension)
                    .Contains(extension)) continue;

                const string pattern = "\\s[\\d]+\\.(.*)$";

                if (!Regex.IsMatch(assetPath, pattern)) continue;

                var _assetPath = Regex.Replace(assetPath, pattern, ".$1");
                File.Copy(assetPath, _assetPath, true);

				AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.ImportAsset(_assetPath, ImportAssetOptions.ForceUpdate);
            }
        }
    }
    #endregion
}