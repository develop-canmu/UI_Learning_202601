using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEditor;

using CruFramework.Adv;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Adv
{
    
    internal class AdvEditorRouteData
    {
        public ulong entryNodeId = 0;
        // 分岐リスト
        public Dictionary<ulong, ulong[]> switchList = new Dictionary<ulong, ulong[]>();
        // 分岐リスト
        public Dictionary<ulong, ulong> switchCaseId = new Dictionary<ulong, ulong>();
        // 終了コマンド分岐
        public Dictionary<ulong, ulong> endCaseId = new Dictionary<ulong, ulong>();
        // ルートリスト
        public List<ulong[]> routeList = new List<ulong[]>();
    }
    
    public class AdvEditorTestRun
    {
        // チェックするノード数
        // これ以上のチェック数は無限ループとする
        private const int MaxErrorCheckNode = 10000;
        
        // ルートごとの結果
        internal class ErrorCheckRouteResult
        {
            // ログ
            public Dictionary<ulong, string> log = new Dictionary<ulong, string>();
            // 通過したノード
            public List<ulong> nodes = new List<ulong>();
        }
    
        // エラーチェック結果
        internal class ErrorCheckResult
        {
            // スイッチリスト
            public List<ulong> switchList = new List<ulong>();
            // ノードごとのログ
            public Dictionary<ulong, StringBuilder> log = new Dictionary<ulong, StringBuilder>();
            // 通過したノードId
            public List<ulong> currentNodes = new List<ulong>();
            // ルートごとの結果
            public List<ErrorCheckRouteResult> route = new List<ErrorCheckRouteResult>();
        }
        
        public AdvEditorTestRun(AdvEditor editor, bool isErrorCheck)
        {
            this.editor = editor;
            this.isErrorCheck = isErrorCheck;
            // 初期化
            latestTestRunCommandNodeId = 0;
            testRunRouteIndex = 0;
            testRunNodeExecuteCount = 0;
            testRunRouteSwitchIndex = 0;
            // マネージャの取得
            testRunManager = (AdvManager)GameObject.FindObjectOfType(editor.GetManagerType(), true);
            // マネージャなし
            if(testRunManager == null)
            {
                Debug.Log( $"{editor.GetManagerType()}がみつかりません" );
                return;
            }
            
            // スキップ
            testRunManager.AutoMode = isErrorCheck ? AdvAutoMode.Fast : AdvAutoMode.None;
#if CRUFRAMEWORK_DEBUG || UNITY_EDITOR
            testRunManager.IsDebugMode = true;
#endif
            // エラーチェック用結果
            errorCheckResult = new ErrorCheckResult();
                
            // マネージャにコールバック追加
            testRunManager.OnExecuteCommand += OnExecuteTestRunCommand;
            // エラーハンドリング
            testRunManager.OnError += OnErrorTestRunCommand;
            // デバッグ用のファイルを生成
            IAdvCommandObject[] commands = editor.GraphView.GetCommands(true, out testRunRouteData).ToArray();
            // マネージャに読み込ませる
            testRunManager.LoadCommandsAsync(commands);
        }
        
        
        
        private AdvEditor editor = null;
        // エラーチェック
        private bool isErrorCheck = false;
        
        private AdvManager testRunManager = null;
        // テスト実行しているマネージャー
        public AdvManager TestRunManager{get{return testRunManager;}}
        
        // 最後に実行したノードId
        private ulong latestTestRunCommandNodeId = 0;
        // テスト実行中の分岐リスト
        private AdvEditorRouteData testRunRouteData = new AdvEditorRouteData();
        // ノード実行回数
        private int testRunNodeExecuteCount = 0;
        // テスト中のルート番号
        private int testRunRouteIndex = 0;
        // テスト中のルート分岐番号
        private int testRunRouteSwitchIndex = 0;
        // デバッグ実行中のログ
        private StringBuilder testRunLog = new StringBuilder();
        
        private ErrorCheckResult errorCheckResult = null;
        /// <summary>エラーチェック結果</summary>
        internal ErrorCheckResult Result{get{return errorCheckResult;}}

        
        private void NextTestRunRoute()
        {
            // 実行回数を初期化
            testRunNodeExecuteCount = 0;
            testRunRouteSwitchIndex = 0;
            // 次のルートへ
            testRunRouteIndex++;
            
            // 通過したノードを保持
            ErrorCheckRouteResult r = new ErrorCheckRouteResult();
            r.nodes.AddRange( errorCheckResult.currentNodes );
            foreach(KeyValuePair<ulong, StringBuilder> log in errorCheckResult.log)
            {
                r.log.Add(log.Key, log.Value.ToString().Trim());
            }
            errorCheckResult.route.Add(r);
            
            errorCheckResult.currentNodes.Clear();
            errorCheckResult.log.Clear();

            // Switch CaseEndが数が一致しない
            foreach(ulong node in errorCheckResult.switchList)
            {
                r.log.Add(node, "対応するCaseEndがみつかりません");
            }
            errorCheckResult.switchList.Clear();
            
            // 全てのルートのチェックが終わったら再生を停止
            if(testRunRouteData.routeList.Count <= testRunRouteIndex)
            {
                testRunManager.IsStopCommand = true;
                testRunManager.AutoMode = AdvAutoMode.None;
                EditorApplication.isPlaying = false;
            }
            else
            {
                // コマンドの位置を初期化
                testRunManager.Restart();
                testRunManager.ForceNextCommand();
            }
        }
        
        // テスト実行中に実行されたコマンド
        private void OnExecuteTestRunCommand(IAdvCommandObject command)
        {
            AdvCommandDebug commandDebug = command as AdvCommandDebug;
            
            
            AdvCommandNode graphViewNode = null;
            
            if(commandDebug != null)
            {
                graphViewNode = editor.GraphView.GetNode(commandDebug.NodeId);
                
                // 最後に実行したノードIdを保持
                latestTestRunCommandNodeId = commandDebug.NodeId;
                
                // 通過したノード
                errorCheckResult.currentNodes.Add(commandDebug.NodeId);
                
                if(isErrorCheck == false)
                { 
                    // ハイライト
                    editor.GraphView.HighlightNode(commandDebug.NodeId);
                }
            }
            
            if(isErrorCheck)
            {
                // コマンドを強制的に次に進める
                testRunManager.ForceNextCommand();
                
                if(graphViewNode is AdvGraphNodeCaseEnd)
                {
                    if(errorCheckResult.switchList.Count <= 0)
                    {
                        AddTestRunLog("CaseEndが多すぎます。");
                    }
                    else
                    {
                        errorCheckResult.switchList.RemoveAt( errorCheckResult.switchList.Count-1 );
                    }
                }
                
                if(commandDebug != null)
                {
                    if(testRunRouteData.switchList.TryGetValue(commandDebug.NodeId, out ulong[] nextNodes))
                    {
                        // スイッチリスト
                        errorCheckResult.switchList.Add( commandDebug.NodeId );
                        
                        ulong[] routeIds = testRunRouteData.routeList[testRunRouteIndex];
                        testRunRouteSwitchIndex++;
                        ulong nextRouteId = routeIds[testRunRouteSwitchIndex];
                        testRunManager.Goto(nextRouteId);
                        // 条件ノードをスキップ
                        testRunManager.ExecuteCommandIndex++;
                        
                    }
                }
                
                // 実行回数を記録
                testRunNodeExecuteCount++;
                // 無限ループ判定
                if(testRunNodeExecuteCount >= MaxErrorCheckNode)
                {
                    AddTestRunLog($"コマンドの実行回数が{MaxErrorCheckNode}を超えたので無限ループと判断して停止しました");
                    NextTestRunRoute();
                }
                // 終了した
                if(command is AdvCommandEnd)
                {
                    NextTestRunRoute();
                }
                else
                {
                    if(graphViewNode != null && graphViewNode is AdvGraphNodeEnd == false && graphViewNode.GetNextNodes().Length == 0)
                    {
                        AddTestRunLog($"Endコマンドに到達できませんでした");
                        NextTestRunRoute();
                    }
                }

            }
        }

        private void AddTestRunLog(string log)
        {
            if(isErrorCheck)
            {
                if(errorCheckResult.log.ContainsKey(latestTestRunCommandNodeId) == false)
                {
                    errorCheckResult.log.Add(latestTestRunCommandNodeId, new StringBuilder());
                }
                errorCheckResult.log[latestTestRunCommandNodeId].AppendLine(log);
            }
            else
            {
                testRunLog.AppendLine(log);
            }
        }
        
        // テスト実行中に出力されたエラー
        private void OnErrorTestRunCommand(string log)
        {
            AddTestRunLog(log);
        }
        
        
        public void End()
        {
            // マネージャからコールバック削除
            testRunManager.OnExecuteCommand -= OnExecuteTestRunCommand;
            testRunManager.OnError -= OnErrorTestRunCommand;
            // マネージャー初期化
            testRunManager = null;
            // ログを出力
            if(testRunLog.Length > 0)
            {
                Debug.Log(testRunLog);
            }
            // ログをクリア
            testRunLog.Clear();
            
            
            if(isErrorCheck)
            {
                // 結果を出力
                VisualElement root = editor.ErrorCheckResultRoot;
                root.style.display = DisplayStyle.Flex;
                root.Clear();
                
                int index = 0;

                foreach(ErrorCheckRouteResult route in errorCheckResult.route)
                {
                    // エラーログ
                    VisualElement routeElements = new VisualElement();
                    routeElements.style.backgroundColor = index % 2 == 0 ? new Color(0.2f, 0.2f, 0.2f, 1.0f) : new Color(0.1f, 0.1f, 0.1f, 1.0f);
                    root.Add(routeElements);
                    index++;
                    
                    // ルート番号
                    Label label = new Label();
                    label.text = "ルート" + index;
                    label.style.fontSize = 16.0f;
                    routeElements.Add(label);
                    
                    // ルートごとに出力
                    foreach(KeyValuePair<ulong, string> log in route.log)
                    {
                        if(string.IsNullOrEmpty(log.Value))continue;
                        // ログ表示
                        Button button = new Button();
                        string name = AdvGraphView.ToNodeName(editor.GraphView.GetNode(log.Key).GetType(), false);
                        button.text = $"<color=#0ff>{name}[{log.Key}]</color> <color=#f33>{log.Value}</color>";
                        button.style.unityTextAlign = TextAnchor.MiddleLeft;
                        // 押下時にノードにフォカスさせる
                        button.clicked += ()=>
                        {
                            foreach(ulong nodeId in route.nodes)
                            {
                                editor.GraphView.HighlightNode(nodeId);
                                if(nodeId == log.Key)break;
                            }
                            editor.GraphView.HighlightNode(log.Key);
                        };
                        routeElements.Add(button);
                    }
                }
            }
        }
    }
}
