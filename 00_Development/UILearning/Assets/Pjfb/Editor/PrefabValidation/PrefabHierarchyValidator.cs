using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Pjfb.Editor
{
    public class PrefabHierarchyValidator : EditorWindow
    {
        private const string PrefabQuery = "t:prefab";
        private const string SearchFolder = "Assets/";
        private const string DefaultFileName = "PrefabHierarchyData";
        private const string Extension = "json";

        private List<AssetReference> assetReferenceList = new List<AssetReference>();
        private Vector2 scrollPosition = Vector2.zero;
        // 差分のチェックフラグ
        private bool isChecked = false;
        
                
        [MenuItem("Pjfb/PrefabHierarchy Validation")]
        public static void Open()
        {
            GetWindow<PrefabHierarchyValidator>();
        }
        
        private struct AssetReference
        {
            public GameObject Object;
            public string[] Names;
        }
        
        [System.Serializable]
        private struct PrefabGameObjectData
        {
            // 親のId
            public string ParentGlobalId => parentGlobalId;
            [SerializeField]
            private string parentGlobalId;
            // GameObjectのId
            public string GlobalId => globalId;
            [SerializeField]
            private string globalId;
            // GameObjectの名前
            public string Name => name;
            [SerializeField]
            private string name;
            // 並び順
            public int SiblingIndex => siblingIndex;
            [SerializeField]
            private int siblingIndex;

            public PrefabGameObjectData(string parentGlobalId, string globalId, string name, int siblingIndex)
            {
                this.parentGlobalId = parentGlobalId;
                this.globalId = globalId;
                this.name = name;
                this.siblingIndex = siblingIndex;
            }
        }
        
        [System.Serializable]
        private struct PrefabData
        {
            // プレハブのGuid
            public string Guid => guid;
            [SerializeField]
            private string guid;
            // プレハブのGameObject
            public List<PrefabGameObjectData> GameObjectDataList => gameObjectDataList;
            [SerializeField]
            private List<PrefabGameObjectData> gameObjectDataList;

            public PrefabData(string guid, List<PrefabGameObjectData> list)
            {
                this.guid = guid;
                gameObjectDataList = list;
            }
        }

        [System.Serializable]
        private struct ExportData
        {
            [System.Serializable]
            private struct PrefabDataWrapper
            {
                public List<PrefabData> Data => data;
                [SerializeField]
                private List<PrefabData> data;
                public PrefabDataWrapper(List<PrefabData> data)
                {
                    this.data = data;
                }
            }
            
            public List<PrefabData> PrefabDataList => wrapper.Data;
            
            // List保存のためのラッパー
            [SerializeField]
            private PrefabDataWrapper wrapper;

            public ExportData(List<PrefabData> prefabDataList)
            {
                wrapper = new PrefabDataWrapper(prefabDataList);
            }

            public void AddList(PrefabData item)
            {
                wrapper.Data.Add(item);
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Export All"))
            {
                // フォルダピッカーでパスを指定
                var filePath = EditorUtility.SaveFilePanel($"Save {Extension}", Application.dataPath, DefaultFileName, Extension);

                if (string.IsNullOrEmpty(filePath)) return;
                
                // プロジェクト内の全てのPrefabデータを読み込む
                var data = LoadAll();
                // ファイルに書き出す
                ExportPrefabData(filePath, data);
            }
            
            // スペース
            GUILayout.Space(10);
            
            // 読み込みボタン
            if (GUILayout.Button("Check All"))
            {
                // 差分データを初期化
                assetReferenceList.Clear();
                
                // ファイルピッカーでファイルを指定
                var filePath = EditorUtility.OpenFilePanel($"Open {Extension}", Application.dataPath, Extension);
                
                if (string.IsNullOrEmpty(filePath)) return;
                
                // ファイルから旧データを読み込む
                var reference = ImportPrefabData(filePath);
                
                // 差分をチェック
                CheckAll(reference, assetReferenceList);
                
                // チェック済みフラグを立てる
                isChecked = true;
            }
            
            // スペース
            GUILayout.Space(10);

            if (assetReferenceList?.Count > 0)
            {
                // スクロールビュー
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                // assetReferenceListをアセット選択可能にして表示する
                foreach (var assetReference in assetReferenceList)
                {
                    EditorGUILayout.ObjectField(assetReference.Object, typeof(GameObject), false);

                    // 階層をスクロールビューで表示
                    foreach (var name in assetReference.Names)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField(name);
                        EditorGUI.indentLevel--;
                        
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                if (isChecked)
                {
                    EditorGUILayout.LabelField("該当アセットなし");
                }
            }
        }

        private void OnValidate()
        {
            isChecked = false;
        }

        /// <summary>比較を行う</summary>
        private void CheckAll(ExportData reference, List<AssetReference> diff)
        {
            var all = LoadAll();
            
            var currentMap = all.PrefabDataList.ToDictionary(x => x.Guid, x => x);
            var referenceMap = reference.PrefabDataList.ToDictionary(x => x.Guid, x => x);

            int index = 0;
            foreach (var (key, currentData) in currentMap)
            {
                index++;
                EditorUtility.DisplayProgressBar("CheckAll", key, (float)index / (float)currentMap.Count);
                
                // 参照元に存在しないデータはスキップ
                if (referenceMap.TryGetValue(key, out var referenceData) == false)
                {
                    Debug.LogError($"Not Found {AssetDatabase.GUIDToAssetPath(key).Split("/").Last()}");
                    continue;
                }

                // 階層が一致していない場合差分リストに追加
                var currentDict = currentData.GameObjectDataList.ToDictionary(x => x.GlobalId, x => x);
                var referenceDict = referenceData.GameObjectDataList.ToDictionary(x => x.GlobalId, x => x);

                List<string> nameList = new List<string>();
                foreach (var (currentKey, currentObject) in currentDict)
                {
                    if (referenceDict.TryGetValue(currentKey, out var referenceObject) == false)
                    {
                        Debug.LogError($"Not Found {currentKey}");
                        continue;
                    }
                    
                    var matchParentId = currentObject.ParentGlobalId == referenceObject.ParentGlobalId;
                    var matchSiblingIndex = currentObject.SiblingIndex == referenceObject.SiblingIndex;

                    if (matchParentId == false || matchSiblingIndex == false)
                    {
                        nameList.Add(currentObject.Name);
                    }
                }
                
                if (nameList.Count > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(currentData.Guid);
                    var assetRef = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    diff.Add(new AssetReference
                    {
                        Object = assetRef,
                        Names = nameList.ToArray()
                    });
                }
            }
            
            // プログレスバーの非表示
            EditorUtility.ClearProgressBar();
        }
        
        /// <summary> Assets下のPrefabを全て取得する </summary>
        private ExportData LoadAll()
        {
            // 結果出力用
            ExportData result = new ExportData(new List<PrefabData>());
            // Assets下のプレハブを取得
            string[] prefabGuids = AssetDatabase.FindAssets(PrefabQuery, new[] { SearchFolder });
            // 各プレハブをチェック
            for(int i=0;i<prefabGuids.Length;i++)
            {
                string guid = prefabGuids[i];
                
                // プログレスバーの表示
                EditorUtility.DisplayProgressBar("CheckLocalPrefabs", guid, (float)(i + 1) / (float)prefabGuids.Length);
                
                // データ格納用
                PrefabData prefabData = new PrefabData(guid, new List<PrefabGameObjectData>());
                // 結果に追加
                result.AddList(prefabData);
                
                // GameObject読み込み
                ExportPrefabData(AssetDatabase.LoadAssetAtPath<Transform>( AssetDatabase.GUIDToAssetPath(guid) ), string.Empty, prefabData);
            }
            
            // プログレスバーの非表示
            EditorUtility.ClearProgressBar();

            return result;
        }
        
        /// <summary>プレハブのデータを再帰的に取得する</summary>
        private void ExportPrefabData(Transform transform, string parent, PrefabData prefabData)
        {
            for(int i=0;i<transform.childCount;i++)
            {
                Transform child = transform.GetChild(i);
                
                // Id
                var globalId = GlobalObjectId.GetGlobalObjectIdSlow(child).ToString();
                // 親のId
                var parentGlobalId = parent;
                // 名前を追加
                var name = child.name;
                // 並び
                var siblingIndex = i;
                
                PrefabGameObjectData gameObjectData = new PrefabGameObjectData(parentGlobalId, globalId, name, siblingIndex);
                // 結果に追加
                prefabData.GameObjectDataList.Add(gameObjectData);
                
                // 子供を出力
                ExportPrefabData(child, gameObjectData.GlobalId, prefabData);
            }
        }
        
        /// <summary>読み込む関数</summary>
        private ExportData ImportPrefabData(string path)
        {
            var file = File.ReadAllText(path);
            return JsonUtility.FromJson<ExportData>(file);
        }
        
        /// <summary>書き出す関数</summary>
        private void ExportPrefabData(string path, ExportData data)
        {
            var str = JsonUtility.ToJson(data);
            File.WriteAllText(path, str);
        }
    }
}