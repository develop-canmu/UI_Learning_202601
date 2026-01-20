using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.IO;
using CruFramework.Adv;
using UnityEditor;
using UnityEditor.UIElements;
using System.Reflection;
using UnityEngine.Assertions.Must;

namespace CruFramework.Editor.Adv
{
    public class AdvGraphView : GraphView
    {
        
        public const float FocusDuration = 0.5f;
        
        [System.Serializable]
        private class CopyEdgeData
        {
            [SerializeField]
            public int inNodeIndex = 0;
            [SerializeField]
            public int outNodeIndex = 0;
        }
        
        [System.Serializable]
        private class CopyData
        {
            [SerializeReference]
            public List<AdvCommandNode> nodes = new List<AdvCommandNode>();
            [SerializeField]
            public List<CopyEdgeData> edges = new List<CopyEdgeData>();
        }
        
        
        public static string ToNodeName(Type type, bool path)
        {
            // 属性を取得
            AdvCommandNodeAttribute attribute = type.GetCustomAttribute<AdvCommandNodeAttribute>();
            string name = string.Empty;
            // タイプ名を使用
            if(attribute == null)
            {
                name = type.Name;
                string removeStr = nameof(AdvGraphNode);
                if(name.StartsWith(removeStr))name = name.Substring(removeStr.Length);
            }
            // 属性からメニュー名を取得
            else
            {
                name = attribute.Path;
            }
            
            if(path == false)name = Path.GetFileName(name);
            
            return name;
        }
        
        // ノードに割り当てるId
        private ulong allocateNodeId = 0;
        // ペースト位置
        private Vector2 pastePosition = Vector2.zero;
        private bool isEnablePaste = false;
        
        // EditorWindow
        private AdvEditor advEditor = null;
        
        private List<AdvCommandNode> commandNodes = new List<AdvCommandNode>();
        /// <summary>コマンドノードリスト</summary>
        public IReadOnlyList<AdvCommandNode> CommandNodes{get{return commandNodes;}}

        // ハイライトノード
        private ulong highlightNodeId = 0;
        
        // フォーカス座標
        private Vector2 focusTargetPos = Vector2.zero;
        private float focusTimer = 0;
        
        private Edge insertNodeEdge = null;
        /// <summary>ノード生成時に挿入する位置</summary>
        public Edge InsertNodeEdge{get{return insertNodeEdge;}set{insertNodeEdge = value;}}

        public AdvGraphView(AdvEditor editorWindow) : base()
        {
            advEditor = editorWindow;
            // 拡縮対応
            SetupZoom(0.1f, ContentZoomer.DefaultMaxScale);
            // ノードのドラッグ
            this.AddManipulator(new SelectionDragger());
            // 全体のドラッグ
            this.AddManipulator(new ContentDragger());
            // 範囲選択
            this.AddManipulator(new RectangleSelector());
            // 
            this.AddManipulator(new ClickSelector());

            // 削除時の処理
            this.deleteSelection = (a, b) =>
            {
                List<ISelectable> selectableTemp = new List<ISelectable>(selection);
                // Undoに登録
                editorWindow.UndoRedo.RegisterDelete(selectableTemp);
                
                for(int i=0;i<selectableTemp.Count;i++)
                {
                    ISelectable s = selectableTemp[i];
                    
                    switch(s)
                    {
                        case AdvGraphNode node:
                        { 
                            DeleteNode(node.CommandNode.NodeId);
                            break;
                        }
                        
                        case Edge edge:
                        {
                            DeleteEdge(edge);
                            break;
                        }
                    }
                }
            };

            
            // ノードの検索ウィンドウ
            AdvGraphNodeSearchWindow searchWindow = AdvGraphNodeSearchWindow.Create(this, editorWindow);
            // ノード生成を選択
            nodeCreationRequest += (context)=>
            {
                SearchWindow.Open( new SearchWindowContext(context.screenMousePosition), searchWindow );
            };

            // コピー
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            // ビュー変更時
            graphViewChanged -= OnGraphViewChanged;
            graphViewChanged += OnGraphViewChanged;
        }
        
        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            
            AdvEditorUndoRedo.ChangeData changeData = new AdvEditorUndoRedo.ChangeData();
            
            if(change.movedElements != null)
            {
                changeData.MoveElements.AddRange(change.movedElements);
                foreach(GraphElement e in change.movedElements)
                {
                    if(e is AdvGraphNode node)
                    {
                        node.CommandNode.SetPosition(e.GetPosition().position);
                    }
                }
            }
            
            if(change.edgesToCreate != null)
            {
                changeData.CreateEdges.AddRange(change.edgesToCreate);
                foreach(Edge edge in change.edgesToCreate)
                {
                    AdvGraphNode inputNode = (AdvGraphNode)edge.input.node;
                    AdvGraphNode outputNode = (AdvGraphNode)edge.output.node;
                    inputNode.CommandNode.UpdateOutputNodes();
                    outputNode.CommandNode.UpdateOutputNodes();
                }
            }
            
            if(change.elementsToRemove != null)
            {
                changeData.RemoveElements.AddRange(change.elementsToRemove);
                foreach(GraphElement e in change.elementsToRemove)
                {
                    if(e is Edge edge)
                    {
                        AdvGraphNode inputNode = (AdvGraphNode)edge.input.node;
                        AdvGraphNode outputNode = (AdvGraphNode)edge.output.node;
                        inputNode.CommandNode.UpdateOutputNodes();
                        outputNode.CommandNode.UpdateOutputNodes();
                    }
                }
            }
            
            advEditor.UndoRedo.Register(changeData);
            return change;
        }

        /// <summary>キーが押された</summary>
        private void OnKeyDown(KeyDownEvent e)
        {
            if(e.commandKey || e.ctrlKey)
            {
                switch(e.keyCode )
                {
                    case KeyCode.C:
                        Copy();
                        break; 
                    case KeyCode.V:
                        if(e.target is GraphView)
                        {
                            Paste(e.originalMousePosition);
                        }
                        break;
                    
                    case KeyCode.R:
                    {
                        AdvGraphNodeEntryPoint entryPoint = GetNode<AdvGraphNodeEntryPoint>();
                        if(entryPoint != null)
                        {
                            FocusNode(entryPoint.NodeId);
                        }
                        break;
                    }
                }
            }
            
            switch(e.keyCode )
            {
                case KeyCode.LeftArrow:
                    viewTransform.position += new Vector3( viewport.worldBound.width * 0.5f, 0, 0);
                    break;
                case KeyCode.RightArrow:
                    viewTransform.position += new Vector3(-viewport.worldBound.width * 0.5f, 0, 0);
                    break;
                case KeyCode.DownArrow:
                    viewTransform.position += new Vector3( 0, -viewport.worldBound.height * 0.5f, 0);
                    break;
                case KeyCode.UpArrow:
                    viewTransform.position += new Vector3( 0, viewport.worldBound.height * 0.5f, 0);
                    break;

            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // ノード生成
            if(evt.target is GraphView && nodeCreationRequest != null)
            {
                evt.menu.AppendAction("Create Node", OnContextMenuCreateNode, DropdownMenuAction.AlwaysEnabled);
                evt.menu.AppendSeparator();
            }

            // コピー
            if(evt.target is Node)
            {
                evt.menu.AppendAction("Copy", (a)=>Copy(), DropdownMenuAction.AlwaysEnabled);
            }
            
            // ペースト
            if(evt.target is GraphView)
            {
                evt.menu.AppendAction("Paste", (a)=>Paste(a.eventInfo.mousePosition), (a)=>canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }
            
            evt.menu.AppendSeparator();
            
            // スライド
            {
                if(evt.target is AdvGraphNode node)
                {
                    evt.menu.AppendAction("右へスライド", (a)=>
                        {
                            AdvEditorUndoRedo.ChangeData changeData = new AdvEditorUndoRedo.ChangeData();
                            SlideNodes(node.CommandNode.NodeId, changeData);
                            AdvEditorUndoRedo.instance.Register(changeData);
                        },
                        DropdownMenuAction.AlwaysEnabled);
                }
            }
            
            evt.menu.AppendSeparator();
            
            // 次の分岐
            {
                if(evt.target is AdvGraphNode node)
                {
                    evt.menu.AppendAction("次の分岐へ", (a)=>FocusNextSwitch(node.CommandNode.NodeId), DropdownMenuAction.AlwaysEnabled);
                }
            }
            
            // 分岐の最初へ
            {
                if(evt.target is AdvGraphNode node)
                {
                    evt.menu.AppendAction("分岐点の最初へ", (a)=>FocusFirstCommand(node.CommandNode.NodeId), DropdownMenuAction.AlwaysEnabled);
                }
            }
            
            {
                // ドキュメント
                if(evt.target is AdvGraphNode node)
                {
                    evt.menu.AppendSeparator();
                    evt.menu.AppendAction("Document", (a)=>AdvDocumentEditor.Open(node.CommandNode.GetType()), DropdownMenuAction.AlwaysEnabled);
                }
            }
            
            // 挿入
            if(evt.target is Edge e)
            {
                bool enable = true;
                if(e.output.node is AdvGraphNode outputNode && outputNode.CommandNode is AdvGraphNodeSwitch)
                {
                    enable = false;
                }
                
                if(e.input.node is AdvGraphNode inputNode && inputNode.CommandNode is AdvGraphNodeCase)
                {
                    enable = false;
                }
                
                evt.menu.AppendAction("Insert Node", (a)=>OnContextMenuInsert(a, e), (a)=>enable ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }
            
            // 削除
            if(evt.target is Node || evt.target is Edge)
            {
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Delete", OnContextMenuDelete, DropdownMenuAction.AlwaysEnabled);
            }
        }
        
        private void OnContextMenuInsert(DropdownMenuAction action, Edge edge)
        {
            // コールバックなし
            if(nodeCreationRequest == null)return;
            insertNodeEdge = edge;
            Vector2 screenPosition = advEditor.position.position + action.eventInfo.mousePosition;
            nodeCreationRequest(new NodeCreationContext(){ screenMousePosition = screenPosition, target = null, index = -1});
        }
        
        private void OnContextMenuCreateNode(DropdownMenuAction action)
        {
            // コールバックなし
            if(nodeCreationRequest == null)return;
            
            Vector2 screenPosition = advEditor.position.position + action.eventInfo.mousePosition;
            nodeCreationRequest(new NodeCreationContext(){ screenMousePosition = screenPosition, target = null, index = -1});
        }
        
        private void OnContextMenuDelete(DropdownMenuAction action)
        {
            DeleteSelectionCallback(AskUser.DontAskUser);
        }
        
        private void Copy()
        {
            serializeGraphElements = new SerializeGraphElementsDelegate(Copy);
            CopySelectionCallback();
        }
        
        private string Copy(IEnumerable<GraphElement> elements)
        {
            CopyData copyData = new CopyData();
            
            Dictionary<ulong, int> nodeIds = new Dictionary<ulong, int>();

            foreach(GraphElement e in elements)
            {
                switch(e)
                {
                    case AdvGraphNode node: 
                    {
                        node.CommandNode.UpdateNodePosition();
                        nodeIds.Add( node.CommandNode.NodeId, copyData.nodes.Count );
                        copyData.nodes.Add(node.CommandNode);
                        break;
                    }
                }
            }
            
            foreach(GraphElement e in elements)
            {
                switch(e)
                {
                    case Edge edge:
                    {
                        CopyEdgeData edgeData = new CopyEdgeData();
                        if(edge.input.node is AdvGraphNode inNode && nodeIds.ContainsKey(inNode.CommandNode.NodeId))
                        {
                            edgeData.inNodeIndex = nodeIds[inNode.CommandNode.NodeId];
                        }
                        else
                        {
                            break;
                        }
                        
                        if(edge.output.node is AdvGraphNode outNode &&  nodeIds.ContainsKey(outNode.CommandNode.NodeId))
                        {
                            edgeData.outNodeIndex = nodeIds[outNode.CommandNode.NodeId];
                        }
                        else
                        {
                            break;
                        }
                        
                        copyData.edges.Add( edgeData );
                        break;
                    }
                }
            }
            
            return  JsonUtility.ToJson( copyData );
        }

        private void Paste(Vector2 position)
        {
            isEnablePaste = true;
            pastePosition = position;
            unserializeAndPaste = new UnserializeAndPasteDelegate(Paste);
            PasteCallback();
        }
        
        private void Paste(string operationName, string data)
        {
            if(string.IsNullOrEmpty(data) || isEnablePaste == false)return;
            
            isEnablePaste = false;
            // 選択解除
            ClearSelection();
            
            // 座標計算
            Vector2 pos = advEditor.rootVisualElement.ChangeCoordinatesTo(advEditor.rootVisualElement.parent, pastePosition);
            pos = contentViewContainer.WorldToLocal(pos);
            
            // コピーデータ
            CopyData copyData = JsonUtility.FromJson<CopyData>(data);
            // ノードの座標を取っておく
            Vector2 basePos = copyData.nodes.Count > 0 ? copyData.nodes[0].NodePosition : Vector2.zero;
            
            // ノードを生成
            for(int i=0;i<copyData.nodes.Count;i++)
            {
                AdvCommandNode node = copyData.nodes[i];
                // Idを割り振り
                node.SetNodeId(++allocateNodeId);
                // ノードの生成
                CreateNode(node, (i != 0 ? (node.NodePosition - basePos) : Vector2.zero) + pos);
                // 選択する
                selection.Add(node.Node);
                node.Node.selected = true;
            }
            
            // Edgeを生成
            for(int i=0;i<copyData.edges.Count;i++)
            {
                CopyEdgeData edgeData = copyData.edges[i];
                AdvCommandNode inputNode = copyData.nodes[edgeData.inNodeIndex];
                AdvCommandNode outputNode = copyData.nodes[edgeData.outNodeIndex];
                
                ConnectPort(outputNode.NodeId, inputNode.NodeId);
            }
        }
        
        private void GetSlideNodes(ulong nodeId, List<AdvCommandNode> result)
        {
            AdvCommandNode node = GetNode(nodeId);
            if(node == null)return;
            result.Add(node);
            // 次のノードへ
            if(node.HasOutputPort)
            {
                node.UpdateOutputNodes();
                foreach(ulong id in node.OutputNodes)
                {
                    GetSlideNodes(id, result);
                }
            }
        }
        
        public void SlideNodes(ulong nodeId, AdvEditorUndoRedo.ChangeData changeData, float slide = 0)
        {
            AdvCommandNode node = GetNode(nodeId);
            // スライド量
            Vector2 slideValue = new Vector2(slide <= 0 ? node.Node.GetPosition().size.x : slide, 0);
            
            // 移動量
            changeData.MoveDelta = slideValue;
            
            List<AdvCommandNode> nodes = new List<AdvCommandNode>();
            // 移動させるノードを取得
            GetSlideNodes(nodeId, nodes);
            
            
            foreach(AdvCommandNode targetNode in nodes)
            {
                // ノードの移動
                targetNode.SetPosition( targetNode.NodePosition + slideValue  );
                // 移動したノード
                changeData.MoveElements.Add(targetNode.Node);
            }
        }
        
        /// <summary>ノードをハイライト</summary>
        public void HighlightNode(ulong nodeId)
        {
            // ノード取得
            AdvCommandNode node = GetNode(highlightNodeId);
            if(node != null)
            {
                node.EndHighlight();
            }
            
            // ノード取得
            node = GetNode(nodeId);
            if(node != null)
            {
                node.BeginHighlight();
                highlightNodeId = nodeId;
            }
            
            FocusNode(nodeId);
        }
        
        
        /// <summary>ノードのフォーカス</summary>
        public void FocusNode(ulong nodeId)
        {
            // ノード取得
            AdvCommandNode node = GetNode(nodeId);
            if(node == null)return;
            
            // ビューのサイズ
            Vector2 viewSize = contentRect.size;
            // 対象の座標を取得
            Rect targetRect = node.Node.GetPosition();
            Vector2 target = -(targetRect.position + targetRect.size * 0.5f) * scale;
            
            focusTargetPos = target + new Vector2(viewSize.x, viewSize.y) * 0.5f;
            focusTimer = Time.realtimeSinceStartup;
        }
        
        public void FocusNextSwitch(ulong nodeId)
        {
            while(true)
            {
                AdvCommandNode node = GetNode(nodeId);
                if(node == null)return;
                
                // 分岐点の場合はフォーカス
                if(node is AdvGraphNodeSwitch)
                {
                    HighlightNode(nodeId);
                    return;
                }
                
                AdvCommandNode[] nextNodes = node.GetNextNodes();
                if(nextNodes.Length == 0)return;
                nodeId = nextNodes[0].NodeId;
            }
        }
        
        public void FocusFirstCommand(ulong nodeId)
        {
            while(true)
            {
                AdvCommandNode node = GetNode(nodeId);
                if(node == null)return;
                
                // 分岐点の場合はフォーカス
                if(node is AdvGraphNodeCase || node is AdvGraphNodeCaseEnd || node is AdvGraphNodeEntryPoint)
                {
                    HighlightNode(nodeId);
                    return;
                }
                
                AdvCommandNode[] preNodes = node.GetPreNodes();
                if(preNodes.Length == 0)return;
                nodeId = preNodes[0].NodeId;
            }
        }

        /// <summary>検索文字列が変わった</summary>
        public void OnChangeSearchText(string searchText)
        {
            foreach(AdvCommandNode node in commandNodes)
            {
                if(node.IsHitSearchText(searchText))
                {
                    node.SetHeaderColor(new Color(0, 0.5f, 0, 1.0f));
                }
                else
                {
                    node.SetDefaultHeaderColor();
                }
            }
        }

        /// <summary>ノードの取得</summary>
        public AdvCommandNode GetNode(ulong id)
        {
            foreach(AdvCommandNode node in commandNodes)
            {
                if(node.NodeId == id)return node;
            }
            return null;
        }
        
        /// <summary>ノードの取得</summary>
        public T GetNode<T>() where T : AdvCommandNode
        {
            foreach(AdvCommandNode node in commandNodes)
            {
                if(node is T v)return v;
            }
            return null;
        }
        
        // 保存処理
        internal AdvEditorSaveData Save()
        {
            foreach(AdvCommandNode node in commandNodes)
            {
                node.OnPreSave();
            }
            
            // データ保存
            AdvEditorSaveData result = new AdvEditorSaveData();
            result.allocateNodeId = allocateNodeId;
            result.commandNodes = commandNodes;
            return result;
        }
        
        // 読み込み
        internal void Load(AdvEditorSaveData saveData)
        {
            // 既存のノードを破棄
            foreach(AdvCommandNode node in commandNodes)
            { 
                RemoveElement(node.Node);
                
                if(node.HasOutputPort) 
                {
                    foreach(Edge edge in node.OutputPort.connections)
                    {
                        RemoveElement(edge);
                    }
                }
            }
            // リストクリア
            commandNodes.Clear();
            
            // Idをコピー
            allocateNodeId = saveData.allocateNodeId;
            // ノードを生成
            foreach(AdvCommandNode node in saveData.commandNodes)
            {
                CreateNode(node, node.NodePosition);
            }
            
            // ノードの接続を生成
            foreach(AdvCommandNode node in saveData.commandNodes)
            {
                foreach(ulong id in node.OutputNodes)
                {
                    ConnectPort(node.NodeId, id);
                }
            }
        }
        
        /// <summary>
        /// コマンドリストを生成
        /// </summary>
        private void GetCommands(AdvCommandNode node, ulong caseStartNodeId, bool isDebug, List<AdvCommandNode> checkList, AdvEditorRouteData routeData, List<IAdvCommandObject> result, List<AdvGraphNodeCaseEnd> caseEndResult)
        {
            // 次のコマンド
            AdvCommandNode[] nextNodes = node.GetNextNodes();
            
            if(node is AdvGraphNodeCaseEnd c)
            {
                // リストに追加
                if(caseEndResult.Contains(c) == false)
                {
                    caseEndResult.Add(c);
                }
                // デバッグ用のコマンド
                if(isDebug)
                {
                    result.Add( new AdvCommandDebug(node.NodeId) );
                }
                // Gotoコマンド
                result.Add( new AdvCommandGoto(node.NodeId) );
                return;
            }
            
            // 同じノードはチェックしない
            if(checkList.Contains(node))return;
            checkList.Add(node);
            
            if(node is AdvGraphNodeEnd end)
            {
                routeData.endCaseId.Add(caseStartNodeId, end.NodeId);
            }
            
            // デバッグ用のコマンド
            if(isDebug)
            {
                result.Add( new AdvCommandDebug(node.NodeId) );
            }
            
            List<IAdvCommandObject> commands = node.GetCommandsInternal();
            if(commands != null)
            {
                // ノードのId
                foreach(IAdvCommandObject command in commands)
                {
                    if(command is IAdvCommandSaveNodeId saveNode)
                    {
                        saveNode.SetNodeId(node.NodeId);
                    }
                }
                result.AddRange(commands);
            }
            
            if(node is AdvGraphNodeSwitch s)
            {
                // 分岐リストを作成
                ulong[] switchNodeList = new ulong[nextNodes.Length];
                // switchノードと接続先のId紐付け
                routeData.switchList.Add(s.NodeId, switchNodeList);
                // switchノードとそのノードの分岐開始ノードIdの紐付け
                routeData.switchCaseId.Add(caseStartNodeId, s.NodeId);
                
                for(int i=0;i<nextNodes.Length;i++)
                {
                    // 分岐点を記録
                    switchNodeList[i] = nextNodes[i].NodeId;
                    // コマンド位置を記録
                    result.Add( new AdvCommandLocation(nextNodes[i].NodeId) );
                    // 次の分岐の位置
                    if(i + 1 < nextNodes.Length)
                    {
                        result.Add( new AdvCommandNextCase(nextNodes[i+1].NodeId) );
                    }
                    else
                    {
                        result.Add( new AdvCommandNextCase(0) );
                    }
                    
                    GetCommands(nextNodes[i], nextNodes[i].NodeId, isDebug, checkList, routeData, result, caseEndResult);
                }
            }
            else
            {
                // 接続しているノードを追加
                foreach(AdvCommandNode nextNode in nextNodes)
                {
                    GetCommands(nextNode, caseStartNodeId, isDebug, checkList, routeData, result, caseEndResult);
                }
            }
        }
        
        /// <summary>コマンドの取得</summary>
        internal List<IAdvCommandObject> GetCommands(bool isDebug)
        {
            return GetCommands(isDebug, out _);
        }
        
        /// <summary>コマンドの取得</summary>
        internal List<IAdvCommandObject> GetCommands(bool isDebug, out AdvEditorRouteData routeData)
        {
            // 分岐リスト
            routeData = new AdvEditorRouteData();
            // 結果
            List<IAdvCommandObject> result = new List<IAdvCommandObject>();
            // エントリーポイント
            AdvGraphNodeEntryPoint entryPoint = GetNode<AdvGraphNodeEntryPoint>();
            // エントリーポイントがない
            if(entryPoint == null)
            {
                Debug.LogError("エントリーポイントがありません");
                return result;
            }
            
            // チェックしたノードId
            List<AdvCommandNode> checkList = new List<AdvCommandNode>(); 
            // 分岐の終了地点リスト
            List<AdvGraphNodeCaseEnd> caseEndResult = new List<AdvGraphNodeCaseEnd>();

            // エントリーポイント
            routeData.entryNodeId = entryPoint.NodeId;
            // ノードをたどってコマンドを生成
            GetCommands(entryPoint, entryPoint.NodeId, isDebug, checkList, routeData, result, caseEndResult);
            
            // 分岐点の処理
            while(true)
            {
                if(caseEndResult.Count <= 0)break;
                AdvGraphNodeCaseEnd caseEnd = caseEndResult[0];
                caseEndResult.RemoveAt(0);
                
                // 位置情報
                result.Add( new AdvCommandLocation(caseEnd.NodeId) );
                // 接続先
                AdvCommandNode[] nextNodes = caseEnd.GetNextNodes();
                foreach(AdvCommandNode nextNode in nextNodes)
                {
                    // ノードをたどってコマンドを生成
                    GetCommands(nextNode, nextNode.NodeId, isDebug, checkList, routeData, result, caseEndResult);
                }
            }
            
            
            // コマンド結果調整
            foreach(IAdvCommandObject command in result)
            {
                // Index
                if(command is IAdvCommandLocationIndex locationIndex)
                {
                    for(int i=0;i<result.Count;i++)
                    {
                        if(result[i] is AdvCommandLocation location && location.Id == locationIndex.LocationId)
                        {
                            locationIndex.SetCommandIndex(i);
                            break;
                        }
                    }
                }
            }
            
            if(isDebug)
            {
                // ルートの計算
                CalcRoute(routeData, entryPoint.NodeId, new List<ulong>());
            }
            
            return result;
        }
        
        // ルート数の計算
        private void CalcRoute(AdvEditorRouteData routeData, ulong nodeId, List<ulong> routeList)
        {
            
            routeList.Add(nodeId);
            // 分岐のないルート
            if(routeData.switchCaseId.TryGetValue(nodeId, out ulong switchNodeId) == false)
            {
                routeData.routeList.Add( routeList.ToArray() );
                return;
            }
            
            // 分岐あり
            
            // 分岐先ノードId
            ulong[] nextNodes = routeData.switchList[switchNodeId];

            foreach(ulong nextNode in nextNodes)
            {
                CalcRoute(routeData, nextNode, new List<ulong>(routeList));
            }
        }
        
        /// <summary>ノードの生成</summary>
        public ulong InsertNode(Type nodeType, Edge edge)
        {
            // 入出力先
            AdvGraphNode outputNode = (AdvGraphNode)edge.input.node;
            AdvGraphNode inputNode = (AdvGraphNode)edge.output.node;
            
            // 変更点
            AdvEditorUndoRedo.ChangeData changeData = new AdvEditorUndoRedo.ChangeData();
            
            // ノードの生成
            ulong nodeId = CreateNode(nodeType, (inputNode.CommandNode.NodePosition + outputNode.CommandNode.NodePosition) * 0.5f, false);
            // ノード取得
            AdvCommandNode node = GetNode(nodeId);
            // 変更点に追加
            changeData.CreateNodes.Add(node.Node);
            
            // 横幅
            float width = node.Node.style.width.value.value;
            
            // 移動させた分位値調整
            node.SetPosition( node.NodePosition + new Vector2(width * 0.5f, 0) );
            
            // エッジの削除
            DeleteEdge(edge);
            // 変更点に追加
            changeData.RemoveElements.Add(edge);
            // エッジの接続
            changeData.CreateEdges.Add( ConnectPort(inputNode.CommandNode.NodeId, nodeId) );
            changeData.CreateEdges.Add( ConnectPort(nodeId, outputNode.CommandNode.NodeId) );
            
            // ノードをスライド
            SlideNodes(outputNode.CommandNode.NodeId, changeData, width);
            
            // Undo登録
            AdvEditorUndoRedo.instance.Register(changeData);

            return nodeId;
        }
        
        /// <summary>ノードの生成</summary>
        public ulong CreateNode(Type nodeType, Vector2 position, bool registerUndo = false)
        {
            // ノードをインスタンス
            AdvCommandNode node = (AdvCommandNode)Activator.CreateInstance(nodeType);
            // Idを割り振り
            node.SetNodeId(++allocateNodeId);
            // ノードの生成
            CreateNode(node, position);
            // Undo登録
            if(registerUndo)
            {
                advEditor.UndoRedo.RegisterCreateNode(node.NodeId, position);
            }
            
            return node.NodeId;
        }
        
        public void CreateNode(AdvCommandNode node, Vector2 position)
        {
            
            // 初期化
            node.Initialize();
            // UI追加
            AddElement(node.Node);
            // 座標
            node.SetPosition(position);
            // リストに追加
            commandNodes.Add(node);
            
            node.Node.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }
        
        public void DeleteNode(ulong nodeId)
        {
            AdvCommandNode node = GetNode(nodeId);
            if(node == null)return;
            
            // Edgeの削除
            if(node.HasOutputPort)
            {
                foreach(Edge e in node.OutputPort.connections)
                {
                    RemoveElement(e);
                    AdvGraphNode outputNode = (AdvGraphNode)e.input.node;
                    outputNode.CommandNode.UpdateOutputNodes();
                }
            }
            
            if(node.HasInputPort)
            {
                foreach(Edge e in node.InputPort.connections)
                {
                    RemoveElement(e);
                    AdvGraphNode inputNode = (AdvGraphNode)e.output.node;
                    inputNode.CommandNode.UpdateOutputNodes();
                }
            }
            // ノードの削除
            this.RemoveElement(node.Node);
            // リストから削除
            commandNodes.Remove(node);
        }
        
        public void DeleteEdge(Edge edge)
        {
            edge.output.Disconnect(edge);
            edge.input.Disconnect(edge);
            RemoveElement(edge);
        }
        
        /// <summary>ノードの接続</summary>
        public Edge ConnectPort(ulong outputNodeId, ulong inputNodeId)
        {
            AdvCommandNode inputNode = GetNode(inputNodeId);
            AdvCommandNode outputNode = GetNode(outputNodeId);
            if(inputNode == null || outputNode == null)return null;

            // 重複チェック
            foreach(Edge edge in outputNode.OutputPort.connections)
            {
                if(edge.input == inputNode.InputPort)return edge;
            }
            // 接続
            Edge connectEdge = outputNode.OutputPort.ConnectTo(inputNode.InputPort);
            AddElement(connectEdge);
            
            return connectEdge;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> campatiblePorts = new List<Port>();
            foreach(Port port in ports)
            {
                // 同じノード
                if(startPort.node == port.node)continue;
                // 同じ入出力タイプ
                if(startPort.direction == port.direction)continue;
                
                if(startPort.node is AdvGraphNode node1 && port.node is AdvGraphNode node2)
                { 
                    // Switchノード
                    if(node1.CommandNode is AdvGraphNodeSwitch && startPort.direction == Direction.Output)
                    {
                        if(node2.CommandNode is AdvGraphNodeCase == false)continue;
                    }

                    if(node1.CommandNode is AdvGraphNodeCase && startPort.direction == Direction.Input)
                    {
                        
                        if(node2.CommandNode is AdvGraphNodeSwitch == false)continue;
                    }
                }
                
                // リストに追加
                campatiblePorts.Add(port);
            }
            
            return campatiblePorts;
        }
        
        /// <summary>
        /// Update
        /// </summary>
        internal void OnUpdate()
        {
            // フォーカス処理
            if(focusTimer > 0)
            {
                float time = Time.realtimeSinceStartup - focusTimer;
                if(time >= FocusDuration) 
                {
                    focusTimer = 0;
                }
                
                viewTransform.position = Vector3.Lerp(viewTransform.position, focusTargetPos, time / FocusDuration);
            }
            
            foreach(AdvCommandNode node in commandNodes)
            {
                node.OnUpdate();
            }
        }
    }
}