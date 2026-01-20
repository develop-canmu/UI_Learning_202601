using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ThirdParty.AppGuard.Editor
{
    public class ObjectLoader<T> where T : ScriptableObject
    {
        private KeyValuePair<string, T> LoadedObject { get; set; }

        public T GetObject(string objectName)
        {
            // Check LoadedObject
            if (LoadedObject.Key == objectName && LoadedObject.Value != null)
            {
                return LoadedObject.Value;
            }

            // Load Object
            Debug.Log($"Find Object: [{typeof(T).Name}] {objectName}");
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name} {objectName}");
            Debug.Log("Find Length: " + guids.Length);
            if (guids[0].Length == 0)
            {
                Debug.LogError(LogFormat($"Not Found ScriptableObject >>> {objectName} [{typeof(T).Name}]"));
                return null;
            }
            var settingFilePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var setting = AssetDatabase.LoadAssetAtPath<T>(settingFilePath);
            LoadedObject = new KeyValuePair<string, T>(objectName, setting);

            Debug.Log($"Load Object Path: {settingFilePath}");

            return LoadedObject.Value;
        }

        private string LogFormat(string msg) => $"[ObjectLoader] {msg}";
    }
}