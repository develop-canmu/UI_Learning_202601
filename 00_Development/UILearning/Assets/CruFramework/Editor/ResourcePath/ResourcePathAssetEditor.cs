using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace CruFramework
{
        [CustomEditor(typeof(ResourcePathAsset))]
    public class ResourcePathAssetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // ResourcePath専用のElementを作成（Inspector用）
            return new ResourcePathElement(
                target as ResourcePathAsset,
                showHeader: true,        // Inspectorにはヘッダー表示
                showUtilityButtons: true); // Inspectorにはユーティリティボタン表示
        }
        
        public override void OnInspectorGUI()
        {
            // UI Toolkitを使用するため、IMGUIは無効化
        }
    }
} 