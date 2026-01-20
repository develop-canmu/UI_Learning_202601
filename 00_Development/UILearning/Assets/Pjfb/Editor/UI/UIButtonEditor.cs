using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{

    [CustomEditor(typeof(UIButton))]
    public class UIButtonEditor : CruFramework.Editor.UIButtonEditor
    {
        
        [MenuItem("GameObject/UI/Pjfb/UIButton")]
        private static void CreateMenu()
        {
            GameObject parent = Selection.gameObjects.Length > 0 ? Selection.gameObjects[0] : null;
            
            // Button
            GameObject buttonObject = new GameObject();
            // レイヤー
            buttonObject.layer = parent.layer;
            // 名前
            buttonObject.name = nameof(UIButton);
            // RectTransform
            RectTransform rt = buttonObject.AddComponent<RectTransform>();
            // 親の設定
            buttonObject.transform.SetParent(parent.transform);
            // 一応初期化
            rt.localPosition = Vector3.zero;;
            rt.localScale = Vector3.one;;
            // UIButton
            UIButton button = buttonObject.AddComponent<UIButton>();
            // Image
            button.targetGraphic = buttonObject.AddComponent<Image>();
            
            rt.sizeDelta = new Vector2(200.0f, 50.0f);
            
            Selection.activeGameObject = buttonObject;
        }
        
 

        public override void OnInspectorGUI()
        {
            
            UIButton button = (UIButton)target;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            

            SerializedProperty soundType = serializedObject.FindProperty("soundType");
            EditorGUILayout.PropertyField(soundType);
            
            if(EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            base.OnInspectorGUI();
        }
        
        private void DrawGridInfoGUI(ScrollGrid.GridInfo grid)
        {
            grid.SpacingType = (ScrollGrid.SpacingType)EditorGUILayout.EnumPopup(nameof(grid.SpacingType), grid.SpacingType);
            if(grid.SpacingType == ScrollGrid.SpacingType.FixedSpace)
            {
                grid.Spacing = EditorGUILayout.FloatField(nameof(grid.Spacing), grid.Spacing);
            }
        }
    }
    
}