using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework
{
    [CreateAssetMenu()]
    public class ResourcePathAsset : ScriptableObject
    {
        [System.Serializable]
        public class PathData
        {
            [SerializeField]
            private string key = string.Empty;
            /// <summary>Key</summary>
            public string Key{get{return key;}set{key = value;}}
            
            [SerializeField]
            private string path = string.Empty;
            /// <summary>Path</summary>
            public string Path{get{return path;}set{path = value;}}
        }
        
        [SerializeField]
        private List<PathData> pathDatas = new List<PathData>();
        
        /// <summary>パスデータを取得</summary>
        public List<PathData> GetPathDatas()
        {
            return pathDatas;
        }
        
        /// <summary>パスをKeyValueで取得</summary>
        public Dictionary<string, string> GetValues()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            for(int i=0;i<pathDatas.Count;i++) 
            {
                if( string.IsNullOrEmpty(pathDatas[i].Key) )continue;
                if(result.ContainsKey(pathDatas[i].Key))continue;
                result.Add(pathDatas[i].Key, pathDatas[i].Path);
            }
            return result;
            
        }
    }
}