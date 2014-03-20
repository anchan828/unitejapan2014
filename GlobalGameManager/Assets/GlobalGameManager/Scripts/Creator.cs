using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GlobalGameManager
{
#if UNITY_EDITOR
    using UnityEditor;

    [InitializeOnLoad]
#endif
    public class Creator
    {

        private const string inspectorFolderPath = "Assets/GlobalGameManager/Editor/Inspectors";
        private const string globalGameManagerFolderPath = "Assets/GlobalGameManager/Resources/GlobalGameManager";


        #region constructors

        static Creator()
        {
#if UNITY_EDITOR

            foreach (
                var monoScript in
                    MonoImporter.GetAllRuntimeMonoScripts()
                        .Where(
                            monoScript =>
                                monoScript.GetClass() != null &&
                                monoScript.GetClass().IsSubclassOf(typeof (GlobalGameManagerObject))))
            {
                var type = monoScript.GetClass();

                if (ExistGlobalGameManager(type) == false)
                {
                    GlobalGameManager(type);
                }

                if (ExistGameManagerInspector(type) == false)
                {
                    GlobalGameManagerInspector(type);
                }
            }

#endif
        }

        #endregion

        #region public methods

        public static T GlobalGameManager<T>() where T : GlobalGameManagerObject
        {
            return (T) GlobalGameManager(typeof (T));
        }

        #endregion

        #region private methods

        private static GlobalGameManagerObject GlobalGameManager(Type type)
        {
            var instance = ScriptableObject.CreateInstance(type) as GlobalGameManagerObject;
#if UNITY_EDITOR


            AssetDatabase.CreateAsset(instance, GetGlobalGameManagerPath(type));

            if (ExistGameManagerInspector(type) == false)
            {
                GlobalGameManagerInspector(type);
            }

            instance =
                Resources.Load(string.Format("GlobalGameManager/{0}", type.Name), type) as GlobalGameManagerObject;
#endif
            return instance;
        }

        private static void GlobalGameManagerInspector(Type type)
        {
#if UNITY_EDITOR
            var script = new StringBuilder();
            script.AppendLine("using UnityEditor;");
            script.AppendLine("using UnityEngine;");
            script.AppendLine("using System.Collections;");
            script.AppendLine("");
            script.AppendLine("[CustomEditor(typeof(#NAME#))]");
            script.AppendLine("public class #NAME#Inspector : GlobalGameManagerInspector");
            script.AppendLine("{");
            script.AppendLine("    [MenuItem(\"Edit/Project Settings/#NAME#\")]");
            script.AppendLine("    private static void ShowManagerSettings()");
            script.AppendLine("    {");
            script.AppendLine("        Show<#NAME#>();");
            script.AppendLine("    }");
            script.Append("}");

            var content = Regex.Replace(script.ToString(), "#NAME#", type.Name);
            var path = GetGlobalGameManagerInspectorPath(type);
            File.WriteAllText(path, content);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
#endif
        }

        private static string GetGlobalGameManagerInspectorPath(Type type)
        {
            return string.Format("{0}/{1}Inspector.cs", inspectorFolderPath, type.Name);
        }

        private static string GetGlobalGameManagerPath(Type type)
        {
            return string.Format("{0}/{1}.asset", globalGameManagerFolderPath, type.Name);
        }

        private static bool ExistGameManagerInspector(Type type)
        {
            Directory.CreateDirectory(inspectorFolderPath);
            return File.Exists(GetGlobalGameManagerInspectorPath(type));
        }

        private static bool ExistGlobalGameManager(Type type)
        {
            Directory.CreateDirectory(globalGameManagerFolderPath);
            return File.Exists(GetGlobalGameManagerPath(type));
        }

        #endregion private methods
    }
}