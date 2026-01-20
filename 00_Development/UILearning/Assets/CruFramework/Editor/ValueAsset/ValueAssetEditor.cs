using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UIElements;

namespace CruFramework.Editor
{
    [CustomEditor(typeof(ValueAsset), true)]
    public class ValueAssetEditor : UnityEditor.Editor
    {
        
        private class SerializePropertyCache
        {
            public SerializedProperty target;
            public SerializedProperty label;
            public SerializedProperty value;
        }
        
        private ReorderableList orderList = null;

        private Texture searchIcon = null;
        private string searchText = string.Empty;
        
        private Dictionary<int, SerializePropertyCache> propertyCaches = new Dictionary<int,SerializePropertyCache>();

        private void OnEnable()
        {
            searchIcon = EditorGUIUtility.IconContent("Search Icon").image;
        }

        public override void OnInspectorGUI()
        {
            ValueAsset valueAsset = (ValueAsset)target;
            
            serializedObject.Update();
            
            // Values
            SerializedProperty defaultValue = serializedObject.FindProperty("defaultValue");
            // Values
            SerializedProperty values = serializedObject.FindProperty("values");
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(defaultValue);
            
            // オーダーリスト
            if(orderList == null)
            {
                orderList = new ReorderableList(serializedObject, values);
                
                orderList.draggable = false;
                
                orderList.onAddCallback += (v)=>
                {
                    valueAsset.AddValueData();
                    propertyCaches.Clear();
                };
                
                orderList.onRemoveCallback += (v)=> 
                {
                    foreach (int index in v.selectedIndices)
                    {
                        valueAsset.RemoveAtValueData(index);
                    }
                    propertyCaches.Clear();
                };
                
                orderList.onReorderCallback += (v)=>
                {
                    propertyCaches.Clear();
                };
                
                // リスト表示名
                orderList.drawHeaderCallback = (rect) =>
                {
                    Rect iconRect = new Rect(rect.x, rect.y, rect.height, rect.height);
                    EditorGUI.DrawTextureAlpha(iconRect, searchIcon);
                    
                    Rect textRect = new Rect(rect.x + rect.height, rect.y, rect.width - rect.height, rect.height);
                    searchText = EditorGUI.TextField(textRect, searchText);
                };
                
                orderList.elementHeightCallback = (index)=>
                {
                    // キャッシュから取得
                    if(propertyCaches.TryGetValue(index, out SerializePropertyCache c) == false)
                    {
                        return EditorGUIUtility.singleLineHeight;
                    }
                    
                    if(c.label.stringValue.Contains(searchText) || c.value.stringValue.Contains(searchText))
                    {
                        return EditorGUIUtility.singleLineHeight;
                    }
                    
                    return 0;
                };
                
                orderList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    // キャッシュから取得
                    if(propertyCaches.TryGetValue(index, out SerializePropertyCache c) == false)
                    {
                        c = new SerializePropertyCache();
                        c.target = values.GetArrayElementAtIndex(index);
                        c.label = c.target.FindPropertyRelative("key");
                        c.value = c.target.FindPropertyRelative("value");
                        propertyCaches.Add(index, c);
                    }
                    // 検索
                    if(searchText.Length > 0)
                    {
                        if(c.label.stringValue.Contains(searchText) == false && c.value.stringValue.Contains(searchText) == false)
                        {
                            return;
                        }
                    }

                    Rect labelRect = new Rect(rect.x, rect.y, 150.0f, rect.height);
                    c.label.stringValue = EditorGUI.TextField(labelRect, new GUIContent(string.Empty), c.label.stringValue);
                    
                    Rect valueRect = new Rect(rect.x + 152.0f, rect.y, rect.width - 152, rect.height);
                    EditorGUI.PropertyField(valueRect, c.value, new GUIContent(string.Empty));
                };
  
            }
            // リスト表示
            orderList.DoLayoutList();
            
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(valueAsset);
                serializedObject.ApplyModifiedProperties();
            }
            
        }
    }
}
