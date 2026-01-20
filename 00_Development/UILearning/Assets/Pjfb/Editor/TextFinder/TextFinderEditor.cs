using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using CruFramework.UI;
using UnityEditor;

namespace Pjfb
{
    public class TextFinderEditor : EditorWindow
    {
        [MenuItem("Pjfb/TextFinder")]
        public static void Open()
        {
            GetWindow<TextFinderEditor>();
        }
        
        private class ResultTextData
        {
            private string objectName = string.Empty;
            /// <summary>オブジェクト名</summary>
            public string ObjectName{get{return objectName;}}
            
            private string key = string.Empty;
            /// <summary>キー名</summary>
            public string Key{get{return key;}}
            
            private string text = string.Empty;
            /// <summary>テキスト</summary>
            public string Text{get{return text;}}
            
            public ResultTextData(string objectName, string key, string text)
            {
                this.objectName = objectName;
                this.key = key;
                this.text = text;
            }
        }
        
        private class ResultPrefabData
        {
            private string path = string.Empty;
            /// <summary>パス</summary>
            public string Path{get{return path;}}
            
            private List<ResultTextData> textDatas = new List<ResultTextData>();
            /// <summary>テキスト</summary>
            public List<ResultTextData> TextDatas{get{return textDatas;}}
            
            public ResultPrefabData(string path, List<ResultTextData> textDatas)
            {
                this.path = path;
                this.textDatas = textDatas;
            }
        }
        
        // 対象のディレクトリ
        private UnityEngine.Object directoryObject = null;

        // 結果
        private List<ResultPrefabData> resultList = new List<ResultPrefabData>();
        
        // スクロール
        private Vector2 scrollValue = Vector2.zero;

        private void OnGUI()
        {
            directoryObject = EditorGUILayout.ObjectField("Directory", directoryObject, typeof(UnityEngine.Object), true);
            
            // 検索実行
            if(GUILayout.Button("Find") && directoryObject != null)
            {
                // パス取得
                string assetPath = AssetDatabase.GetAssetPath(directoryObject);
                // ディレクトリかチェック
                if(Directory.Exists(assetPath) == false)
                {
                    Debug.LogError("ディレクトリを指定してください");
                }
                else
                {
                    GUI.FocusControl(string.Empty);

                    // 初期化
                    resultList.Clear();
                    // 指定フォルダのプレハブを取得
                    string[] prefabGuids = AssetDatabase.FindAssets("t:prefab", new string[]{assetPath});

                    // 各プレハブをチェック
                    foreach(string prefabGuid in prefabGuids)
                    {
                        // パス取得
                        string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
                        // GameObject読み込み
                        GameObject prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                        // Textコンポーネントを取得
                        TMPro.TextMeshProUGUI[] textComponents = prefabObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
                        // テキストがない場合は処理しない
                        if(textComponents.Length <= 0)continue;
                        
                        // 結果
                        List<ResultTextData> textDatas = new List<ResultTextData>();
                        // 各コンポーネントをチェック
                        foreach(TMPro.TextMeshProUGUI textComponent in textComponents)
                        {
                            // キー設定
                            StringValueSetter valueSetter = textComponent.GetComponent<StringValueSetter>();
                            string key = valueSetter != null ? valueSetter.StringId : "None";
                            // リストに追加
                            textDatas.Add( new ResultTextData(textComponent.name, key, textComponent.text) );
                        }
                     
                        // 結果
                        resultList.Add(new ResultPrefabData(prefabPath, textDatas));
                    }
                }
            }
            
            if(resultList.Count > 0)
            {            
                // クリップボードにコピー
                if(GUILayout.Button("クリップボードにコピー", GUILayout.Width(200)))
                {
                    StringBuilder clipBoard = new StringBuilder();
                    foreach(ResultPrefabData result in resultList)
                    {
                        foreach(ResultTextData textData in result.TextDatas)
                        {
                            // 空文字は無視
                            if(string.IsNullOrEmpty(textData.Text))continue;
                            // 開業を変換して結果に入れる
                            clipBoard.AppendLine(textData.Text.Replace("\n", "<br>"));
                        }
                    }
                    // クリップボードへコピー
                    GUIUtility.systemCopyBuffer = clipBoard.ToString();
                }
                
                // 結果を表示
                scrollValue = EditorGUILayout.BeginScrollView(scrollValue);
                
                foreach(ResultPrefabData result in resultList)
                {                
                    EditorGUILayout.LabelField(result.Path);
                    foreach(ResultTextData textData in result.TextDatas)
                    {
                        EditorGUILayout.BeginVertical("TextArea");
                        EditorGUILayout.LabelField($"{textData.ObjectName} Key = {textData.Key}");
                        EditorGUILayout.LabelField($"{textData.Text}");
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.EndScrollView();
            }
        }
    }
}