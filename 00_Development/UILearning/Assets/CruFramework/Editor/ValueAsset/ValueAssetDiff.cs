using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using UnityEditor;

namespace Pjfb
{
    public class ValueAssetDiff : EditorWindow
    {
        [MenuItem("CruFramework/Tools/ValueAssetDiff")]
        public static void Open()
        {
            GetWindow<ValueAssetDiff>();
        }
        
        private enum DiffType
        {
            Delete, Add, Change
        }
        
        private class DiffData
        {
            public DiffType diffType = DiffType.Delete;
            public string key = string.Empty;
            public object value1 = null;
            public object value2 = null;
            
            public DiffData(string key, DiffType diffType, object value1, object value2)
            {
                this.diffType = diffType;
                this.key = key;
                this.value1 = value1;
                this.value2 = value2;
            }
        }
        
        // 比較するアセット
        private ValueAsset valueAsset1 = null;
        private ValueAsset valueAsset2 = null;
        
        // 差分データ
        private List<DiffData> diffDataList = new List<DiffData>();
        
        // スクロール
        private Vector2 scrollValue = Vector2.zero;

        private Dictionary<string, object> GetValues(ValueAsset valueAsset)
        {
            string[] keys = valueAsset.GetKeys();
            Dictionary<string, object> value = new Dictionary<string, object>();
            foreach(string key in keys)
            {
                value.Add(key, valueAsset.GetValue(key));
            }
            
            return value;
        }

        private void UpdateDiff()
        {
            // 差分データ削除
            diffDataList.Clear();
            
            // アセット指定がない
            if(valueAsset1 == null || valueAsset2 == null)
            {
                return;
            }
            
            // 違うアセット
            if(valueAsset1.GetType() != valueAsset2.GetType())
            {
                return;
            }
            
            Dictionary<string, object> values1 = GetValues(valueAsset1);
            Dictionary<string, object> values2 = GetValues(valueAsset2);
            
            // 差分をチェック
            foreach(KeyValuePair<string, object> value in values1)
            {
                // 相手側にもキーが有る場合は変更
                if(values2.TryGetValue(value.Key, out object v))
                {
                    if(object.Equals(value.Value, v) == false)
                    {
                        diffDataList.Add( new DiffData(value.Key, DiffType.Change, value.Value, v) );
                    }
                }
                // 相手にキーがないの削除
                else
                {
                    diffDataList.Add( new DiffData(value.Key, DiffType.Delete, value.Value, null) );
                }
                
                values2.Remove(value.Key);
            }
            
            // 残ったキーを追加
            foreach(KeyValuePair<string, object> value in values2)
            {
                diffDataList.Add( new DiffData(value.Key, DiffType.Add, null, value.Value) );
            }
        }
        
        private void OnEnable()
        {
            UpdateDiff();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();
            
            if(GUILayout.Button("Reload", GUILayout.Width(200)))
            {
                UpdateDiff();
            }
            
            valueAsset1 = (ValueAsset)EditorGUILayout.ObjectField("SrcAsset", valueAsset1, typeof(ValueAsset), true);
            valueAsset2 = (ValueAsset)EditorGUILayout.ObjectField("DstAsset", valueAsset2, typeof(ValueAsset), true);
            
            if(EditorGUI.EndChangeCheck())
            {
                UpdateDiff();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // アセット指定がない
            if(valueAsset1 == null || valueAsset2 == null)return;

            scrollValue = EditorGUILayout.BeginScrollView(scrollValue);
            
            foreach(DiffData diff in diffDataList)
            {
                EditorGUILayout.BeginHorizontal();
                
                
                switch(diff.diffType)
                {
                    case DiffType.Add:
                        GUI.color = Color.green;
                        break;
                    case DiffType.Change:
                        GUI.color = Color.yellow;
                        break;
                    case DiffType.Delete:
                        GUI.color = Color.red;
                        break;
                }
                
                if(GUILayout.Button(diff.key, GUILayout.Width(200)))
                {
                    if(diff.diffType == DiffType.Change || diff.diffType == DiffType.Delete)
                    {
                        GenericMenu menu = new GenericMenu();
                        
                        menu.AddDisabledItem( new GUIContent(diff.key) );
                        
                        menu.AddItem( new GUIContent("左を適用"), false, ()=>
                        {
                            valueAsset2.SetValue(diff.key, diff.value1);
                            UpdateDiff();
                        });
                        
                        menu.ShowAsContext();
                    }
                }
                
                GUI.color = Color.white;
                
                EditorGUILayout.TextField(diff.value1 != null ? diff.value1.ToString() : string.Empty);
                EditorGUILayout.TextField(diff.value2 != null ? diff.value2.ToString() : string.Empty);
                
                EditorGUILayout.EndHorizontal();
                
            }        
            
            EditorGUILayout.EndScrollView();
        }
    }
}