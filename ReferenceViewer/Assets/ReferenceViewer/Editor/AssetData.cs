using System.Collections.Generic;

namespace ReferenceViewer
{
    public class AssetData
    {
        public string path;
        public string guid;
        public List<string> reference = new List<string>();
    }
}