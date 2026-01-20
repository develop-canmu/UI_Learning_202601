using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework
{
    [FrameworkDocument("Tools", nameof(ResourcePathManager), "パスの管理用クラス。CruFramework/ResourcePathで定義して使用する")]
    public static class ResourcePathManager
    {
        /// <summary>デフォルトで読み込む設定ファイル</summary>
        public static readonly string DefaultResourcePathAsset = "ResourcePath";

        public static ResourcePathAsset LoadDefaultResourcePathAsset()
        {
            return Resources.Load<ResourcePathAsset>(DefaultResourcePathAsset);
        }
        
        static ResourcePathManager()
        {
            // デフォルトの設定を読み込む
            ResourcePathAsset resourcePath = LoadDefaultResourcePathAsset();
            if(resourcePath != null)
            {
                LoadResourcePathAsset( resourcePath );
            }
        }
        
        
        /// <summary>管理しているパス</summary>
        private static Dictionary<string, string> paths = new Dictionary<string, string>();
        
        /// <summary>パス</summary>
        [FrameworkDocument("パスを取得する")]
        public static string GetPath(string key)
        {
            if(paths.TryGetValue(key, out string path))
            {
                return path;
            }
            return string.Empty;
        }
        
        /// <summary>パス</summary>
        public static string GetPath(string key, string id)
        {
            if(paths.TryGetValue(key, out string path))
            {
                return path.Replace("{id}", id);
            }
            return string.Empty;
        }
        
        /// <summary>パス</summary>
        public static string GetPath(string key, params string[] strings)
        {
            if(paths.TryGetValue(key, out string path))
            {
                return string.Format(path, strings);
            }
            return string.Empty;
        }
        
        /// <summary>パス</summary>
        public static string GetPath(string key, int id)
        {
            return GetPath(key, id.ToString());
        }
        
        public static string GetPath(string key, long id)
        {
            return GetPath(key, id.ToString());
        }
        
        /// <summary>パス</summary>
        public static string GetPathFormat(string key, params object[] values)
        {
            if(paths.TryGetValue(key, out string path))
            {
                return string.Format(path, values);
            }
            return string.Empty;
        }

        /// <summary>設定ファイルを読み込む</summary>
        public static void LoadResourcePathAsset(ResourcePathAsset resourcePath)
        {
            Dictionary<string, string> paths = resourcePath.GetValues();
            foreach(KeyValuePair<string, string> path in paths)
            {
                AddPath(path.Key, path.Value);
            }
        }
        
        /// <summary>パスの追加</summary>
        public static void AddPath(string key, string path)
        {
            paths.Add(key, path);
        }
    }
}