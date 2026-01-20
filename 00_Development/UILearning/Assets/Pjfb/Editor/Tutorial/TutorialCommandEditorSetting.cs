using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using CruFramework.UI;
using Unity.VisualScripting;
using UnityEditor;

namespace Pjfb
{
    [CustomPropertyDrawer(typeof(TutorialCommandSetting.TutorialCommandData))]
    public class TutorialCommandEditorSetting : PropertyDrawer
    {

        private TutorialCommandSetting.TutorialCommandData tutorialCommand;        
        private Type[] commandType;
        private string[] commandTypeNames;
        private int typeIndices;
        
        // 初期化
        public TutorialCommandEditorSetting()
        {
            commandType = Assembly.GetAssembly(typeof(TutorialCommandSetting.TutorialCommandData)).GetTypes().Where(t => t.IsSubclassOf(typeof(TutorialCommandSetting.TutorialCommandData)) && !t.IsAbstract).ToArray();
            commandTypeNames = commandType.Select(t => t.Name).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            tutorialCommand = (TutorialCommandSetting.TutorialCommandData)property.managedReferenceValue;
            
            // ポップアップ箇所の初期化
            
            // すでにコマンドが仕込まれている場合それを初期値に
            if(tutorialCommand != null)
            {
                Type type = tutorialCommand.GetType();
                typeIndices = Array.IndexOf(commandType, type);
            }
            else
            {
                typeIndices = 0;
                tutorialCommand = (TutorialCommandSetting.TutorialCommandData)Activator.CreateInstance(commandType[typeIndices]);
            }
            // ポップアップ位置の設定
            Rect popupPosition = EditorGUI.PrefixLabel(position, label);
            // ポップアップの判定位置調整
            popupPosition.height =  EditorGUIUtility.singleLineHeight;
            typeIndices = EditorGUI.Popup(popupPosition,typeIndices, commandTypeNames);
            if(tutorialCommand.GetType() != commandType[typeIndices])
            {
                property.managedReferenceValue = (TutorialCommandSetting.TutorialCommandData)Activator.CreateInstance(commandType[typeIndices]);
            }
            
            if(GUI.changed)
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            
            // クラスごとのフィールドの配置
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);
            contentPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(contentPosition, property, label, true);
        }
        
        // 必要な高さを算出
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                return EditorGUIUtility.singleLineHeight;
            }
            
            return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight;
        }
        
    }
}