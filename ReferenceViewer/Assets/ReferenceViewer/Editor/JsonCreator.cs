using System.Linq;
using UnityEditor;
using System.IO;
using System.Text;

namespace ReferenceViewer
{
    public class JsonCreator
    {
        public static void Build()
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
            });
        }

        private static void Export(Data data)
        {
            data.assetData = data.assetData.OrderBy(d => Path.GetExtension(d.path)).ToList();
            const string directory = "build/ReferenceViewer";

            Directory.CreateDirectory(directory);
            var sb = new StringBuilder();
            var writer = new LitJson.JsonWriter(sb) { PrettyPrint = true };
            LitJson.JsonMapper.ToJson(data, writer);

            File.WriteAllText(directory + "/data.json", sb.ToString());
        }
    }
}