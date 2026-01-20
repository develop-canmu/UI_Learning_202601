

///////////////////////////////////////////////////////////////////
//
// アプリケーションレイヤーで準備するもの
//・AdvManager を継承したクラス
//・AdvConfig を継承したクラス
//・AdvEditor を継承したクラス
//・アプリ側の仕様に対応したコマンド
//
// Idの定義
// 1.AdvObjectIds<T> を継承したモデルクラスを定義してAdvConfig(アプリ側でオーバーライドしたクラス)に宣言
// 2.intフィールドに属性をつける [AdvObjectId(nameof(AdvConfig.FieldName))]
// 3.ノードエディタ上で文字列で選択できるようになる
// 内部的には数字Idで保持しています
// 参考：AdvCommandMove.cs
// 
///////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CruFramework.Adv;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CruFramework.Editor.Adv
{
    // 保存用
    [System.Serializable]
    public struct AdvEditorSaveData
    {
        [SerializeField]
        public ulong allocateNodeId;
        [SerializeField]
        public string advDataFileGuid;
        [SerializeReference]
        public List<AdvCommandNode> commandNodes;
    }
    
    public abstract class AdvEditor<T> : AdvEditor where T : AdvManager
    {
        public sealed override Type GetManagerType()
        {
            return typeof(T);
        }
    }
    
    [AdvDocument(AdvDocumentEditor.EditorCategory, "Editor", "")]
    public abstract class AdvEditor : EditorWindow
    {
        private static AdvEditor instance = null;
        /// <summary>Instance</summary>
        public static AdvEditor Instance{get{return instance;}}
        
        // 編集中のファイル
        private AdvDataFile editFile = null;

        private AdvGraphView graphView = null;
        // グラフビュー
        public AdvGraphView GraphView{get{return graphView;}}
        
        [SerializeField]
        private string latestEditFile = string.Empty;
        [SerializeField]
        private float scale = 1.0f;
        [SerializeField]
        private Vector3 scroll = Vector3.zero;
        
        [SerializeField]
        private Vector3 playScroll = Vector3.zero;

        private ObjectField editFileField = null;
        
        // Editorのメッセージ表示
        private Label message = null;
        // メッセージの表示時間
        private float messageTimer = 0;

        // テストラン
        private bool isTestRun = false;
        private bool isErrorCheck = false;
        
        private ScrollView errorCheckResultRoot = null;
        /// <summary>エラーチェック結果表示</summary>
        public ScrollView ErrorCheckResultRoot{get{return errorCheckResultRoot;}}
        
        // テストランクラス
        private AdvEditorTestRun testRun = null;

        // 検索文字列
        private string searchText = string.Empty;
        
        /// <summary>エディタ用ファイルの保存先</summary>
        protected virtual string EditorFileDirectory{get{return "Assets";}}
        /// <summary>ファイルの保存先</summary>
        protected virtual string AdvFileDirectory{get{return "Assets";}}
        /// <summary>Configファイルまでのパス</summary>
        public abstract AdvConfig AdvConfigAsset{get;}
        
        /// <summary>UndoRedo</summary>
        public AdvEditorUndoRedo UndoRedo{get{return AdvEditorUndoRedo.instance;}}
        
        /// <summary>マネージャーの種類</summary>
        public abstract Type GetManagerType();
        
        // キー押下時
        private void OnKeyDown(KeyDownEvent e)
        {
            if(e.commandKey || e.ctrlKey)
            {
                if(e.keyCode == KeyCode.S)
                {
                    Save();
                }
            }
        }
        
        /// <summary>
        /// 再生モードが変わったとき
        /// </summary>
        private void OnChangePlayMode(PlayModeStateChange state)
        {
            // 保存
            if(state == PlayModeStateChange.ExitingEditMode)
            {
                playScroll = graphView.viewTransform.position;
                Save();
            }
            
            UndoRedo.SetEditor(this);
            // テストランの場合はファイルを読み込み
            if(state == PlayModeStateChange.EnteredPlayMode && isTestRun)
            {
                testRun = new AdvEditorTestRun(this, isErrorCheck);
                OnStartTestRun(testRun.TestRunManager);
            }
            
            if(state == PlayModeStateChange.ExitingPlayMode && isTestRun)
            {
                testRun.End();
                // フラグをOff
                isTestRun = false;
                isErrorCheck = false;
                
                graphView.viewTransform.position = playScroll;
            }
        }

        private void CreateMenu()
        {
            // メニューバーの生成
            Toolbar toolbar = new Toolbar();
            toolbar.style.alignItems = Align.Center;
            rootVisualElement.Add(toolbar);
            
            // 保存ボタン
            ToolbarButton saveButton = new ToolbarButton();
            saveButton.text = "Save";
            saveButton.clicked += ()=>{Save(true);};
            toolbar.Add(saveButton);
            
            // 保存ボタン
            ToolbarButton saveAsButton = new ToolbarButton();
            saveAsButton.text = "Save As";
            saveAsButton.clicked += ()=>{Save(false);};
            toolbar.Add(saveAsButton);
            
            // 読み込みボタン
            ToolbarButton loadButton = new ToolbarButton();
            loadButton.text = "Load";
            loadButton.clicked += ()=>
            {
                string path = EditorUtility.OpenFilePanel("Select adv file", EditorFileDirectory, "adv");
                if(string.IsNullOrEmpty(path) == false)
                {
                    Load(path);
                }
            };
            toolbar.Add(loadButton);
            
            // 新規作成
            ToolbarButton newButton = new ToolbarButton();
            newButton.text = "New";
            newButton.clicked += ()=>
            {
                CreateNewFile();
            };
            toolbar.Add(newButton);
            
            // 新規作成
            ToolbarButton loadExcelButton = new ToolbarButton();
            loadExcelButton.text = "Import Excel";
            loadExcelButton.clicked += ()=>
            {
                LoadExcel();
            };
            toolbar.Add(loadExcelButton);
            
            VisualElement space = new VisualElement();
            space.style.width = 100.0f;
            toolbar.Add(space);
            
            
            // 読み込みボタン
            ToolbarButton finderButton = new ToolbarButton();
            finderButton.text = "Finder";
            finderButton.clicked += ()=>
            {
                AdvCommandFileFinder.Open(EditorFileDirectory);
            };
            toolbar.Add(finderButton);

            // 読み込みボタン
            ToolbarButton configButton = new ToolbarButton();
            configButton.text = "Config";
            configButton.clicked += ()=>
            {
                OnConfigButton();
            };
            toolbar.Add(configButton);
            
            // 読み込みボタン
            ToolbarButton helpButton = new ToolbarButton();
            helpButton.text = "Help";
            helpButton.clicked += ()=>
            {
                OnHelpButton();
            };
            toolbar.Add(helpButton);
            
            // 右寄せ
            toolbar.Add(new ToolbarSpacer() { flex = true });

            
            // エラーチェック
            ToolbarButton errorCheckResultButton = new ToolbarButton();
            errorCheckResultButton.text = "ErrorCheckResult";
            errorCheckResultButton.clicked += ()=>{OnErrorCheckResultButton();};
            toolbar.Add(errorCheckResultButton);
            
            // エラーチェック
            ToolbarButton errorCheckButton = new ToolbarButton();
            errorCheckButton.text = "ErrorCheck";
            errorCheckButton.clicked += ()=>{ExecuteErrorCheck();};
            toolbar.Add(errorCheckButton);
            
            // テストラン
            ToolbarButton testRunButton = new ToolbarButton();
            testRunButton.text = "Test Run";
            testRunButton.clicked += ()=>{TestRun();};
            toolbar.Add(testRunButton);
        }
        
        private void CreateGraphViewToolMenu()
        {
            VisualElement element = new VisualElement();
            rootVisualElement.Add(element);
            element.style.flexDirection = FlexDirection.Row;
            element.style.height = 20.0f;
            
            
            // 虫眼鏡アイコン
            Image searchIcon = new Image();
            searchIcon.image = EditorGUIUtility.IconContent("Search Icon").image;
            searchIcon.style.width = 20.0f;
            element.Add( searchIcon );
            
            // テキストフィールド
            TextField searchField = new TextField();
            searchField.style.width = 200.0f;
            searchField.RegisterValueChangedCallback((v)=>
            {
                searchText = searchField.value;
                OnChangeSearchText();
            });
            
            element.Add( searchField );
            
            // 編集ファイル
            editFileField = new ObjectField();
            editFileField.objectType = typeof(UnityEngine.Object);
            editFileField.allowSceneObjects = false;
            editFileField.value = editFile;
            
            element.Add(editFileField);
        }
        
        // グラフビューの生成
        private void CreateGraphView()
        {
            if(graphView == null)
            {
                VisualElement parent = new VisualElement();
                parent.style.flexGrow = 1.0f;
                rootVisualElement.Add(parent);
                
                graphView = new AdvGraphView(this);
                graphView.style.flexGrow = 1.0f;
                graphView.style.backgroundColor = Color.black;
                parent.Add( graphView );

                
                // キー入力
                graphView.RegisterCallback<KeyDownEvent>(OnKeyDown);
                
                errorCheckResultRoot = new ScrollView();
                errorCheckResultRoot.style.height = 120.0f;
                errorCheckResultRoot.style.display = DisplayStyle.None;
                errorCheckResultRoot.style.borderBottomWidth = 2.0f;
                errorCheckResultRoot.style.borderBottomColor = Color.gray;
                errorCheckResultRoot.style.borderTopWidth = 2.0f;
                errorCheckResultRoot.style.borderTopColor = Color.gray;
                rootVisualElement.Add(errorCheckResultRoot);
            }
        }
        
        // フッターの生成
        private void CreateFooter()
        {
            VisualElement element = new VisualElement();
            rootVisualElement.Add(element);
            element.style.flexDirection = FlexDirection.Row;
            element.style.height = 20.0f;
            
            // メッセージ表示用
            message = new Label();
            element.Add(message);
        }

        private void OnEnable()
        {
            AdvEditorUndoRedo.instance.SetEditor(this);
            
            // メニュー
            CreateMenu();
            // グラフビューのツール
            CreateGraphViewToolMenu();
            // グラフビューの生成
            CreateGraphView();
            
            // フッターの生成
            CreateFooter();
            
            // 再生モード変更時のコールバック登録
            EditorApplication.playModeStateChanged -= OnChangePlayMode;
            EditorApplication.playModeStateChanged += OnChangePlayMode;
            // ファイルの読み込み
            Load(latestEditFile);
            
            // スクロール位置と拡縮を維持
            graphView.viewTransform.position = scroll;
            graphView.viewTransform.scale = new Vector3(scale, scale, 1.0f);
            
            instance = this;
        }
        
        protected virtual  void OnStartTestRun(AdvManager maanger)
        {
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnChangePlayMode;
            instance = null;
        }
        
        private void SetMessage(string msg)
        {
            message.text = msg;
            messageTimer = Time.realtimeSinceStartup;
        }
        
        
        [AdvDocument("編集中のファイルの保存。初回保存時のみアセットの書き出し先を指定してください。")]
        private void Save(bool isOverwrite = true)
        {
            // スケールを維持
            scale = graphView.scale;
            // スクロールを維持
            scroll = graphView.viewTransform.position;
            
            string guid = string.Empty;
            // アセットファイルを選択
            if(editFile == null || isOverwrite == false)
            {
                string path = EditorUtility.SaveFilePanelInProject("Save AdvFile", Path.GetFileName(latestEditFile), "asset", String.Empty, AdvFileDirectory);
                if(string.IsNullOrEmpty(path))return;
                guid = AssetDatabase.AssetPathToGUID(path);
                
                // 保存先がない
                if(string.IsNullOrEmpty(guid))
                {
                    AdvDataFile newAsset = ScriptableObject.CreateInstance<AdvDataFile>();
                    AssetDatabase.CreateAsset(newAsset, path);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    guid = AssetDatabase.AssetPathToGUID(path);
                }
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(editFile);
                guid = AssetDatabase.AssetPathToGUID(path);
            }
            
            
            // 保存先がない
            if(string.IsNullOrEmpty(guid))return;
            // 保存先ファイル
            editFile = AssetDatabase.LoadAssetAtPath<AdvDataFile>( AssetDatabase.GUIDToAssetPath(guid) );
            if(editFile == null)return;
            
            editFileField.value = editFile;
            // 保存
            AdvEditorSaveData saveData = graphView.Save();
            // Guid
            saveData.advDataFileGuid = guid;
            // Json化
            string json = JsonUtility.ToJson(saveData);
            // シリアライズ
            File.WriteAllText(latestEditFile, json);
            
            // コマンドの取得
            List<IAdvCommandObject> commands = graphView.GetCommands(false);
            List<IAdvCommandObject> commandsCopy = new List<IAdvCommandObject>();
            
            foreach(IAdvCommandObject c in commands)
            {
                string commandJson = JsonUtility.ToJson(c);
                commandsCopy.Add( (IAdvCommandObject)JsonUtility.FromJson(commandJson, c.GetType()) );
            }
            
            // アセット上書き
            editFile.SetCommands(commandsCopy.ToArray());
            EditorUtility.SetDirty(editFile);
            AssetDatabase.SaveAssets();

            SetMessage("保存しました");
        }
        
        [AdvDocument("ファイルの読み込み。.advファイルを選択してください。")]
        private void Load(string path)
        {
            if(string.IsNullOrEmpty(path))return;
            latestEditFile = path;
            // ファイル読み込み
            string json = File.ReadAllText(path);
            // データシリアライズ
            AdvEditorSaveData saveData = JsonUtility.FromJson<AdvEditorSaveData>(json);
            // 読み込み
            graphView.Load(saveData);
            // 保存先ファイル
            editFile = AssetDatabase.LoadAssetAtPath<AdvDataFile>( AssetDatabase.GUIDToAssetPath(saveData.advDataFileGuid) );
            editFileField.value = editFile;
            SetMessage("読み込みました");
        }
        
        [AdvDocument("New", "新規作成。.advファイルを作成します。")]
        private bool CreateNewFile()
        {
            string path = EditorUtility.SaveFilePanel("Create adv file", EditorFileDirectory, string.Empty, "adv");
            if(string.IsNullOrEmpty(path) == false)
            {
                CreateNewFile(path);
                Load(path);
                ulong entryPointId = graphView.CreateNode(typeof(AdvGraphNodeEntryPoint), Vector2.zero, false);
                ulong endId = graphView.CreateNode(typeof(AdvGraphNodeEnd), new Vector2(500, 0), false);
                graphView.ConnectPort(entryPointId, endId);
                return true;
            }
            return false;
        }
        
        private void CreateNewFile(string path)
        {
            AdvEditorSaveData saveData = new AdvEditorSaveData();
            File.WriteAllText(path, JsonUtility.ToJson(saveData));
        }
        
        private void LoadExcel()
        {
            string path = EditorUtility.OpenFilePanel("Import Excel file", string.Empty, "xlsx");
            if(string.IsNullOrEmpty(path) == false)
            {
                LoadExcel(path);
            }
        }
        
        [AdvDocument("エクセルファイルの読み込み。エクセル内のメッセージをコマンド化します。新しいファイルが作成されます")]
        private void LoadExcel(string path)
        {
            if(string.IsNullOrEmpty(path))return;
            // エクセルをCsvに変換
            Dictionary<string, string[,]> csv = ExcelUtility.ToCSV(path);
            // 読み込み失敗
            if(csv == null || csv.Count <= 0)return;
            // Save
            Save();

            foreach(KeyValuePair<string, string[,]> value in csv)
            {
                // 保存パス
                string savePath = EditorFileDirectory + "/" + value.Key + ".adv";
                // ファイル生成
                CreateNewFile(savePath);
                // 読み込み
                Load(savePath);
                
                Debug.Log(savePath);
                // インポート
                AdvExcelImporter.Import(value.Value, graphView, AdvConfigAsset);
                // 保存
                Save();
            }
        }
        
        private void ToTextFile()
        {
            StringBuilder sb = new StringBuilder();
            // コマンドの取得
            List<IAdvCommandObject> commands = graphView.GetCommands(false);
            
            string tab = string.Empty;
            
            foreach(IAdvCommandObject command in commands)
            {
                sb.Append( tab + command.GetType().Name );
                sb.Append("(");
                FieldInfo[] fields = command.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach(FieldInfo field in fields)
                {
                    sb.Append($"{field.Name} = {field.GetValue(command)}, ");
                }
                sb.AppendLine(")");
            }
            
            File.WriteAllText("AdvCommand.txt", sb.ToString() );
        }
        
        [AdvDocument("Config", "設定ファイルを選択する。")]
        private void OnConfigButton()
        {
            Selection.activeObject = AdvConfigAsset;
            AdvConfigEditor.Open(AdvConfigAsset);
        }

        
        [AdvDocument("Help", "ヘルプを開く。")]
        private void OnHelpButton()
        {
            AdvDocumentEditor.Open();
        }

        [AdvDocument("ErrorCheckResult", "ErrorCheckの実行結果を表示")]
        private void OnErrorCheckResultButton()
        {
            errorCheckResultRoot.style.display = errorCheckResultRoot.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        }

        [AdvDocument("ErrorCheck", "編集中のAdvの全ての分岐パターンをプログラム的にチェックします。現在開いているシーンにAdvManagerが必要です。<color=#ff0>＊時間かかります。</color>")]
        private void ExecuteErrorCheck()
        {
            // 停止中の場合は再生
            if(EditorApplication.isPlaying == false)
            {
                isErrorCheck = true;
                isTestRun = true;
                EditorApplication.isPlaying = true;
            }
        }
        
        [AdvDocument("デバッグモードで実行。現在開いているシーンにAdvManagerが必要です。実行すると現在編集中のAdvが自動で再生されてノードを追跡しながら確認できます。")]
        private void TestRun()
        {
            // 停止中の場合は再生
            if(EditorApplication.isPlaying == false)
            {
                isTestRun = true;
                EditorApplication.isPlaying = true;
            }
        }
        
        [AdvDocument("SearchField", "検索フィールドでコマンド名やパラメータを検索できます。")]
        private void OnChangeSearchText()
        {
            string low = searchText.ToLower();
            graphView.OnChangeSearchText(low);
        }
        
        private void Update()
        {
            UndoRedo.Update();
            graphView.OnUpdate();
            
            // メッセージの表示
            if(messageTimer > 0 && Time.realtimeSinceStartup - messageTimer >= 3.0f)
            {
                messageTimer = 0;
                message.text = string.Empty;
            }
        }

    }
}