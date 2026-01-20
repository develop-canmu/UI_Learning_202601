
using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.API;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;


namespace Pjfb.Editor
{
    public class DebugToolApi : DebugToolMenuBase
    {
        
        public enum ViewMode
        {
            Json, Class
        }
        
        /// <summary>
        /// Unity仕様上０から連番のIndexにする
        /// </summary>
        private enum ViewTabType
        {
            Log, Response
        }
        
        private DebugToolAPIHandler apiHandler = new DebugToolAPIHandler();
        
        private DebugToolApiConnector apiConnctor = new DebugToolApiConnector();
        
        // スクロール
        private Vector2 scrollValue = Vector2.zero;
        // 最後に表示した個数
        private int latestApiCount = -1;
        // 検索文字列
        private string searchText = string.Empty;
        
        // ビュータイプ
        private ViewTabType viewTabType = ViewTabType.Log;
        // ビュー名
        private string[] viewNames = null;
        
        // スクロール
        private Vector2 responseScrollValue = Vector2.zero;

        public override void OnInitialize()
        {
            viewNames = System.Enum.GetNames(typeof(ViewTabType));
            apiConnctor.SetEditor(Editor);
        }

        public override string GetName()
        {
            return "API";
        }

        public override void OnUpdate()
        {
            APIManager.Instance.SetHandler(apiHandler);
            APIManager.Instance.SetConnecter(apiConnctor);
            // Viewの更新
            if(apiHandler.ResultList.Count != latestApiCount)
            {
                latestApiCount = apiHandler.ResultList.Count;
                Editor.Repaint();
            }
        }
        
        
        private void OnLogGUI()
        {
            // ヘッダー
            EditorGUILayout.BeginHorizontal();
            
            // 検索
            Rect iconRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(20.0f));
            GUI.DrawTexture(iconRect, Editor.SearchIcon);
            // 入力フィールド
            searchText = EditorGUILayout.TextArea(searchText);
            
            EditorGUILayout.EndHorizontal();
            
            // 機能ボタン
            EditorGUILayout.BeginHorizontal();
            // クリア
            if(GUILayout.Button("Clear", GUILayout.Width(60.0f)))
            {
                apiHandler.Clear();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // スクロール
            scrollValue = EditorGUILayout.BeginScrollView(scrollValue);
            
            // APIのレスポンスを表示
            foreach(DebugToolAPIHandler.ApiResultData api in apiHandler.ResultList)
            {
                EditorGUILayout.BeginVertical("TextArea");
                EditorGUILayout.BeginHorizontal();
                
                // API名
                bool foldout = EditorGUILayout.Foldout(SaveData.IsOpenApi(api.Name), api.Name);
                SaveData.SetOpenApi(api.Name, foldout);
                
                // クリップボードにコピー
                if(GUILayout.Button("Copy", GUILayout.Width(40.0f)))
                {
                    GUIUtility.systemCopyBuffer = api.Json;
                }
                
                // レスポンスを保存
                if(GUILayout.Button("Save", GUILayout.Width(40.0f)))
                {
                    SaveData.ResponseList.Add( new DebugToolResponseSaveData(api.Name, api.Json, api.ResponseType) );
                    Editor.Save();
                }
                
                EditorGUILayout.EndHorizontal();

                if(foldout)
                {
                    EditorGUI.indentLevel++;
                    // Post
                    bool foldoutPost = EditorGUILayout.Foldout(SaveData.IsOpenPost(api.Name), "Post");
                    SaveData.SetOpenPost(api.Name, foldoutPost);
                    if(foldoutPost)
                    {
                        EditorGUILayout.TextArea(api.Post);
                    }
                    // Jsonを表示
                    bool foldoutResponse = EditorGUILayout.Foldout(SaveData.IsOpenResponse(api.Name), "Response");
                    SaveData.SetOpenResponse(api.Name, foldoutResponse);
                    // 中身の表示
                    if(foldoutResponse)
                    {
                        switch(SaveData.ApiViewMode)
                        {
                            case ViewMode.Json:
                            {
                                EditorGUILayout.TextArea(api.Json);
                                break;
                            }
                            case ViewMode.Class:
                            {
                                EditorGUI.indentLevel++;
                                api.ClassView.SetJson(api.ResponseType, api.Json);
                                api.ClassView.DrawGUI(false, false);
                                EditorGUI.indentLevel--;
                                break;
                            }
                        }
                    }
                    
                    EditorGUI.indentLevel--;
                    // スペース
                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void OnResponseGUI()
        {
            // スクロール
            responseScrollValue = EditorGUILayout.BeginScrollView(responseScrollValue);
            
            // APIのレスポンスを表示
            for(int i=0;i<SaveData.ResponseList.Count;i++)
            {
                DebugToolResponseSaveData data = SaveData.ResponseList[i];
                EditorGUILayout.BeginVertical("TextArea");
                EditorGUILayout.BeginHorizontal();
                
                // 有効か
                data.Options = (DebugToolResponseSaveData.OptionType)EditorGUILayout.EnumFlagsField(data.Options, GUILayout.Width(20.0f));
                
                // API名
                bool foldout = EditorGUILayout.Foldout(SaveData.IsOpenApi(data.Name), data.Name);
                SaveData.SetOpenApi(data.Name, foldout);
                
                // レスポンスを保存
                if(GUILayout.Button("Del", GUILayout.Width(40.0f)))
                {
                    SaveData.ResponseList.Remove(data);
                    i--;
                    Editor.Save();
                }
                
                EditorGUILayout.EndHorizontal();
                
                if(foldout)
                {
                    switch(SaveData.ApiViewMode)
                    {
                        case ViewMode.Json:
                        {
                            // Jsonを表示
                            data.Json = EditorGUILayout.TextArea(data.Json);
                            // スペース
                            EditorGUILayout.Space();
                            
                            break;
                        }
                        
                        case ViewMode.Class:
                        {
                            if(data.ResponseType == null)break;
                            EditorGUI.indentLevel++;
                            data.ClassView.SetJson(data.ResponseType, data.Json);
                            data.Json = data.ClassView.DrawGUI(data.CanEditCode, false);
                            EditorGUI.indentLevel--;
                            break;
                        }
                    }

                }
                
                EditorGUILayout.EndVertical();
            }
            
            
            
            EditorGUILayout.EndScrollView();
        }

        public override void OnGUI()
        {
            
            // メニュー
            viewTabType = (ViewTabType)GUILayout.Toolbar((int)viewTabType, viewNames);
            // 表示モード
            SaveData.ApiViewMode = (ViewMode)EditorGUILayout.EnumPopup(SaveData.ApiViewMode, GUILayout.Width(60.0f));
            // タイプ別で表示
            switch(viewTabType)
            {
                case ViewTabType.Log:
                {
                    OnLogGUI();
                    break;
                }
                case ViewTabType.Response:
                {
                    OnResponseGUI();
                    break;
                }
            }
        }
    }
}