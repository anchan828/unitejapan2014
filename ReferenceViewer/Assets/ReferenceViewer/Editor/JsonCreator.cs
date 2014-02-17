using System;
using System.Linq;
using UnityEditor;
using System.IO;
using System.Text;

namespace ReferenceViewer
{
    public class JsonCreator
    {
        public static void Build(Action callback = null)
        {
            if (!EditorApplication.SaveCurrentSceneIfUserWantsTo()) return;

            var currentScene = EditorApplication.currentScene;
            var data = new Data();

            GenerateAssetData.Build(AssetDatabase.GetAllAssetPaths(), assetData =>
            {
                data.assetData.AddRange(assetData);
                EditorUtility.UnloadUnusedAssets();
                EditorApplication.OpenScene(currentScene);
                Export(data);
                if (callback != null)
                    callback();
            });
        }

        private static void Export(Data data)
        {
            data.assetData = data.assetData.OrderBy(d => Path.GetExtension(d.path)).ToList();
            const string directory = "build/ReferenceViewer";

            Directory.CreateDirectory(directory);

            var sb = new StringBuilder();

            var writer = new LitJson.JsonWriter(sb);

            for (int i = 0; i < data.assetData.Count; i++)
            {
                var assetData = data.assetData[i];
                if (assetData.sceneData.Count != 0)
                    assetData.sceneData =
                        assetData.sceneData.Distinct(new CompareSelector<SceneData, string>(s => s.name + s.guid)).ToList();
            }
            sb = new StringBuilder();
            writer = new LitJson.JsonWriter(sb) { PrettyPrint = true };
            LitJson.JsonMapper.ToJson(data, writer);

            File.WriteAllText(directory + "/data.json", sb.ToString());
        }
    }
}