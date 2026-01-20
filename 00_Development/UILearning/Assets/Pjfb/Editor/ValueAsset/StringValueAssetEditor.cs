using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEditor;

namespace Pjfb
{
    [CustomEditor(typeof(StringValueAsset))]
    public class StringValueAssetEditor : UnityEditor.Editor
    {
        // スクロールビューの高さ定数（key-valueリストの表示領域）
        private const float SCROLL_VIEW_HEIGHT = 400f;
        
        // リスト表示
        private bool isDrawList = false;
        // 検索文字列
        private string searchText = string.Empty;
        
        // 検索アイコン
        private Texture searchIcon = null;
        
        // リスト表示用のスクロール位置
        private Vector2 scrollPosition = Vector2.zero;
        
        private void OnEnable()
        {
            searchIcon = EditorGUIUtility.IconContent("Search Icon").image;
        }
        
        /// <summary>アセットの保存</summary>
        private void SaveAsset()
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
        
        public override void OnInspectorGUI()
        {
            StringValueAsset asset = (StringValueAsset)target;
            
            // 追加
            if(GUILayout.Button("Add"))
            {
                // 編集ウィンドウを開く
                StringValueAssetAddWindow editWindow = ScriptableObject.CreateInstance<StringValueAssetAddWindow>();
                // 編集するデータを登録
                editWindow.SetEditData(asset, SaveAsset);
                // モーダルとして開く
                editWindow.ShowModal();
            }
            
            EditorGUILayout.Space();
            
            // リスト表示
            if(GUILayout.Button("List"))
            {
                isDrawList = !isDrawList;
            }
            
            if(isDrawList)
            {
                
                StringValueAsset.ValueData editTarget = null;
                // 検索
                Rect rect = EditorGUILayout.GetControlRect();
                EditorGUI.DrawTextureAlpha( new Rect(rect.x, rect.y, rect.height, rect.height), searchIcon);
                searchText = EditorGUI.TextField(new Rect(rect.x + rect.height, rect.y, rect.width - rect.height, rect.height) , searchText);
                
                EditorGUILayout.Space();
                
                // ヘッダー行
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Action", GUILayout.Width(40));
                EditorGUILayout.LabelField("Key", GUILayout.Width(200));
                EditorGUILayout.LabelField(":", GUILayout.Width(10));
                EditorGUILayout.LabelField("Value", GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                
                // 区切り線
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                
                // スクロールビュー開始
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(SCROLL_VIEW_HEIGHT));
                
                foreach(StringValueAsset.ValueData value in asset.Values)
                {
                    // 検索
                    if(searchText.Length > 0)
                    {
                        if(value.Key.Contains(searchText) == false && value.Value.Contains(searchText) == false)continue;
                    }
                    
                    EditorGUILayout.BeginHorizontal();
                    
                    // 編集ボタン
                    if(GUILayout.Button("Edit", GUILayout.Width(40)))
                    {
                        editTarget = value;
                    }
                    
                    // キーの表示
                    EditorGUILayout.SelectableLabel(value.Key, GUILayout.Width(200), GUILayout.Height(20.0f));
                    
                    // 区切り線
                    EditorGUILayout.LabelField(":", GUILayout.Width(10));
                    
                    // 値の表示（長い場合は切り詰める）
                    string displayValue = value.Value;
                    if (displayValue.Length > 100)
                    {
                        displayValue = displayValue.Substring(0, 100) + "...";
                    }
                    // 改行を除去して表示
                    displayValue = displayValue.Replace("\n", " ").Replace("\r", " ");
                    EditorGUILayout.SelectableLabel(displayValue, GUILayout.Height(20.0f));
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                // スクロールビュー終了
                EditorGUILayout.EndScrollView();
                
                if(editTarget != null)
                {
                    // 編集ウィンドウを開く
                    StringValueAssetEditWindow editWindow = ScriptableObject.CreateInstance<StringValueAssetEditWindow>();
                    // 編集するデータを登録
                    editWindow.SetEditData(asset, editTarget, SaveAsset);
                    // モーダルとして開く
                    editWindow.ShowModal();
                }
            }
        }
    }
    
    /// <summary>
    /// データ追加ようウィンドウ
    /// </summary>
    public class StringValueAssetAddWindow : EditorWindow
    {
        private StringValueAsset editAsset = null;
        
        // 追加するキー
        private string key = string.Empty;
        // 追加する直
        private string value = string.Empty;
        
        private Action onSave = null;
        
        // メッセージ
        private string messageText = string.Empty;
        
        /// <summary>編集するデータ</summary>
        public void SetEditData(StringValueAsset target, Action onSave)
        {
            editAsset = target;
            this.onSave = onSave;
        }
        
        private void OnGUI()
        {
            key = EditorGUILayout.TextField("Key", key);
            value = EditorGUILayout.TextArea(value, GUILayout.Height(60));
            // 追加処理
            if(GUILayout.Button("Add"))
            {
                
                messageText = string.Empty;
                
                // 重複チェック
                foreach(StringValueAsset.ValueData value in editAsset.Values)
                {
                    if(value.Key == key)
                    {
                        messageText = "Keyが重複しています";
                    }
                }
                
                // キーがない
                if(string.IsNullOrEmpty(key))
                {
                    messageText = "Keyが空です";
                }
                
                
                if(string.IsNullOrEmpty(messageText))
                {
                    editAsset.AddValueData(key, value);
                    
                    onSave();
                    // 追加後に閉じる
                    Close();
                }
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(messageText);
        }
    }
    
    /// <summary>
    /// 編集用ウィンドウ
    /// </summary>
    public class StringValueAssetEditWindow : EditorWindow
    {
        private StringValueAsset editAsset = null;
        // 編集するデータ
        private StringValueAsset.ValueData editValue = null;
        
        private Action onSave = null;
        
        private string key = string.Empty;
        private string value = String.Empty;
        
        /// <summary>編集するデータの登録</summary>
        public void SetEditData(StringValueAsset asset, StringValueAsset.ValueData target, Action onSave)
        {
            editAsset = asset;
            editValue = target;
            
            this.onSave = onSave;
            
            key = target.Key;
            value = target.Value;
        }

        private void OnGUI()
        {
            key = EditorGUILayout.TextField("Key", key);
            value = EditorGUILayout.TextArea(value, GUILayout.Height(60));
            
            if(GUILayout.Button("Save"))
            {
                editValue.Key = key;
                editValue.Value = value;
                
                onSave();
                Close();
            }
        }
    }
}