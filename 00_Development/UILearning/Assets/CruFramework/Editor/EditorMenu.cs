using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CruFramework.UI;

namespace CruFramework.Editor
{
    internal static class EditorMenu
    {
        public const string AddressablesMenuPath = "CruFramework/Addressables";
        public const string BuildMenuPath = "CruFramework/Build";
        public const string UIMenuPath = "CruFramework/UI";
        public const string ToolMenuPath = "CruFramework/Tools";

        /// <summary>UI解像度の設定ファイルを生成する</summary>
        [MenuItem("Assets/Create/" + UIMenuPath + "/Create " + nameof(UIResolutionSettings))]
        private static void CreateUIResolutionSettings()
        {
            ScriptableObject asset = ScriptableObject.CreateInstance<UIResolutionSettings>();
            AssetDatabase.CreateAsset(asset, "Assets/UIResolutionSettings.asset");
            AssetDatabase.Refresh();
        }
    }
}
