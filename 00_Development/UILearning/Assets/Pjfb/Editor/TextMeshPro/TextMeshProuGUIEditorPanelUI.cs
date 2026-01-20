using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;

namespace Pjfb.Editor
{
    [CustomEditor(typeof(TextMeshProUGUI), true), CanEditMultipleObjects]
    public class TextMeshProUGUIEditorPanelUI : TMP_EditorPanelUI
    {
        private TextMeshProUGUI tmp = null;
        // マテリアルプロパティセッターがついているか
        private bool hasMaterialPropertySetter = false;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            tmp = (TextMeshProUGUI)target;
            hasMaterialPropertySetter = tmp.gameObject.GetComponent<TextMeshFontMaterialPropertySetter>() != null;
        }

        protected override void DrawExtraSettings()
        {
            GUILayout.Label(new GUIContent("<b>Pjfb Settings</b>"), TMP_UIStyleManager.sectionHeader);
            DrawPjfbSettings();
            
            base.DrawExtraSettings();
        }

        // Face部分の設定
        private void DrawPjfbSettings()
        {
            // すでにプロパティセッターがついているならボタンを押せなくする
            EditorGUI.BeginDisabledGroup(hasMaterialPropertySetter);
            if (GUILayout.Button("Add FontMaterialPropertySetter", TMP_UIStyleManager.alignmentButtonMid))
            {
                tmp.gameObject.AddComponent<TextMeshFontMaterialPropertySetter>();
                hasMaterialPropertySetter = true;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}