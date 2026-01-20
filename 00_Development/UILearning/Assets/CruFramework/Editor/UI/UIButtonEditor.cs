using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Editor
{

    [CustomEditor(typeof(UIButton))]
    public class UIButtonEditor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            UIButton button = (UIButton)target;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            button.LongTapTriggerMode = (UIButton.LongTapTriggerType)EditorGUILayout.EnumPopup(nameof(button.LongTapTriggerMode), button.LongTapTriggerMode);
            
            if(button.LongTapTriggerMode != UIButton.LongTapTriggerType.None)
            {
                button.LongTapTriggerTime = EditorGUILayout.FloatField(nameof(button.LongTapTriggerTime), button.LongTapTriggerTime);
                
                if(button.LongTapTriggerMode == UIButton.LongTapTriggerType.Repeat)
                {
                    button.LongTapTriggerRepeatInterval = EditorGUILayout.FloatField(nameof(button.LongTapTriggerRepeatInterval), button.LongTapTriggerRepeatInterval);
                }
            }
            
            button.ClickTriggerInterval = EditorGUILayout.FloatField(nameof(button.ClickTriggerInterval), button.ClickTriggerInterval);
            button.IsTriggerOnce = EditorGUILayout.Toggle(nameof(button.IsTriggerOnce), button.IsTriggerOnce);
            
            EditorGUILayout.Space();
            
            SerializedProperty clickEx = serializedObject.FindProperty("onClickEx");
            EditorGUILayout.PropertyField(clickEx);
            
            SerializedProperty longTapEx = serializedObject.FindProperty("onLongTapEx");
            EditorGUILayout.PropertyField(longTapEx);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("==== Unity Button ====");
            base.OnInspectorGUI();
            
            SerializedProperty longTap = serializedObject.FindProperty("onLongTap");
            EditorGUILayout.PropertyField(longTap);
            
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
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