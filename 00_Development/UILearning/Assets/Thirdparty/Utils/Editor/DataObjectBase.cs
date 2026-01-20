using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ThirdParty.Utils.Editor
{
    [Serializable]
    public abstract class DataSettingObject<T> where T : ScriptableObject
    {
        public virtual string SettingFileName { get; } = typeof(T).ToString();

        private string SaveDirectory { get; set; }

        public string SavePath
        {
            get
            {
                if (string.IsNullOrEmpty(SaveDirectory))
                {
                    var temp = ScriptableObject.CreateInstance<T>();
                    var mono = MonoScript.FromScriptableObject(temp);
                    var scriptPath = AssetDatabase.GetAssetPath(mono);
                    SaveDirectory = Path.GetDirectoryName(scriptPath);
                }

                return Path.Combine(SaveDirectory, SettingFileName);
            }
        }


        public T LoadSetting()
        {
            var data = EditorUtil.ReadAsset<T>(SavePath);
            if (data == null)
            {
                return ScriptableObject.CreateInstance<T>();
            }

            return data;
        }

        public void SaveSetting(T data)
        {
            if (data == null)
            {
                Debug.LogWarning("Fail : Not Found Setting Data Object");
                return;
            }

            EditorUtil.WriteAsset(data, SavePath);
        }
    }
}