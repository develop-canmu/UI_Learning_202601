using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System;

namespace CruFramework
{
    public class ResourcePathEditor : EditorWindow
    {
        [MenuItem("CruFramework/ResourcePath")]
        public static void Open()
        {
            GetWindow<ResourcePathEditor>(nameof(ResourcePathEditor));
        }
        
        private ResourcePathAsset resourcePathAsset = null;
        private ResourcePathElement resourcePathElement;
        
        private void OnEnable()
        {
            resourcePathAsset = ResourcePathManager.LoadDefaultResourcePathAsset();
            if(resourcePathAsset == null)
            {
                Label label = new Label();
                label.text = $"{ResourcePathManager.DefaultResourcePathAsset}が見つかりません。\\nResources直下に{ResourcePathManager.DefaultResourcePathAsset}を配置して、ツールウィンドウを再起動してください。";
                rootVisualElement.Add(label);
                return;
            }
            
            CreateUI();
        }
        
        private void CreateUI()
        {
            // ResourcePath専用のElementを作成
            resourcePathElement = new ResourcePathElement(
                resourcePathAsset,
                showHeader: false,        // EditorWindowにはヘッダー不要
                showUtilityButtons: false); // EditorWindowにはユーティリティボタン不要
            
            rootVisualElement.Add(resourcePathElement);
            
            // 検索フィールドにフォーカスを設定
            rootVisualElement.schedule.Execute(() => {
                resourcePathElement?.FocusSearchField();
            }).ExecuteLater(100);
        }
    }
}