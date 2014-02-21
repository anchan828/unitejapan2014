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
		public class Preference
		{
				static readonly string key = "Overwriter";
				static bool isEnabled = false;
				static Extension ex;
				static Overwriter overwriter = new Overwriter ();

				[PreferenceItem("Overwriter")]
				static void PreferenceOnGUI ()
				{

						if (!isEnabled) {
								isEnabled = true;
								ex = new Exception ();
								RefreshExtensions ();
						}

						EditorGUI.BeginChangeCheck ();
						var all = EditorGUILayout.ToggleLeft ("すべての拡張子", overwriter.all);
						if (EditorGUI.EndChangeCheck ()) {
								overwriter.all = all;
						}
						EditorGUI.BeginDisabledGroup (all);

						EditorGUILayout.BeginHorizontal (GUILayout.Width (Screen.width / 2));
						ex.extension = EditorGUILayout.TextField (ex.extension);
						if (GUILayout.Button ("+") && !string.IsNullOrEmpty (ex.extension)) {
								overwriter.extensions.Insert (0, ex);
								ex = new Exception ();
								GUI.FocusControl ("");
								UpdateExtensions ();
						}
						EditorGUILayout.EndHorizontal ();
						

						for (int i = 0; i < overwriter.extensions.Count; i++) {
								EditorGUILayout.BeginHorizontal (GUILayout.Width (Screen.width / 2));

								var extension = overwriter.extensions [i];
								
								DrawExtension (extension);
								
								if (GUILayout.Button ("-")) {
										overwriter.extensions.Remove (overwriter.extensions [i]);
										UpdateExtensions ();
										break;
								}
								EditorGUILayout.EndHorizontal ();
						}
						EditorGUI.EndDisabledGroup ();
				}

				static void UpdateExtensions ()
				{
						var bytes = ObjectToByteArray (overwriter);
						EditorUserSettings.SetConfigValue (key, Convert.ToBase64String (bytes));
						RefreshExtensions ();
				}

				static void RefreshExtensions ()
				{
						overwriter = GetOverwriter (EditorUserSettings.GetConfigValue (key));
				}

				static void DrawExtension (Extension extension)
				{
						extension.enabled = EditorGUILayout.ToggleLeft (extension.extension, extension.enabled);
						EditorGUI.BeginDisabledGroup (!extension.enabled);
						EditorGUI.EndDisabledGroup ();
				}

				public static Overwriter GetOverwriter ()
				{
						return GetOverwriter (EditorUserSettings.GetConfigValue (key));
				}

				static Overwriter GetOverwriter (string value)
				{
						if (string.IsNullOrEmpty (value)) {
								return new Overwriter ();
						}
					
						byte[] bytes = Convert.FromBase64String (value);

						var overwriter = ByteArrayToObject<Overwriter> (bytes);
						return overwriter != null ? overwriter : new Overwriter ();
				}

				static byte[] ObjectToByteArray (object obj)
				{
						if (obj == null)
								return null;
						BinaryFormatter bf = new BinaryFormatter ();
						MemoryStream ms = new MemoryStream ();
						bf.Serialize (ms, obj);
						return ms.ToArray ();
				}
		
				static T ByteArrayToObject<T> (byte[] arrBytes)
				{
						MemoryStream memStream = new MemoryStream ();
						BinaryFormatter binForm = new BinaryFormatter ();
						memStream.Write (arrBytes, 0, arrBytes.Length);
						memStream.Seek (0, SeekOrigin.Begin);
						T obj = (T)binForm.Deserialize (memStream);
						
						return obj;
				}
		}

		[Serializable]
		public class Overwriter
		{
				public bool all;
				public List<Extension> extensions = new List<Exception> ();
		}

		[Serializable]
		public class Extension
		{
				public string extension;
				public bool enabled = true;
		}

		public class AssetPostprocessor : UnityEditor.AssetPostprocessor
		{

				static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
				{
						var overwriter = Preference.GetOverwriter ();
						foreach (var assetPath in importedAssets) {
								var extension = Path.GetExtension (assetPath);
								if (overwriter.all || overwriter.extensions.Where (ex => ex.enabled).Select (ex => ex.extension).Contains (extension)) {
										var pattern = "\\s[\\d]+\\.(.*)$";
										if (Regex.IsMatch (assetPath, pattern)) {
												var _assetPath = Regex.Replace (assetPath, pattern, ".$1");
												File.Copy (assetPath, _assetPath, true);
												AssetDatabase.DeleteAsset (assetPath);
												AssetDatabase.ImportAsset (_assetPath, ImportAssetOptions.ForceUpdate);
										}
								}
						}
				}
		}

}