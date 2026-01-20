using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace CruFramework.Editor.Adv
{
    
    public class AdvEditorUndoRedo : ScriptableSingleton<AdvEditorUndoRedo>
    {
        
        public class ChangeData
        {
            private Vector2 moveDelta = Vector2.zero;
            /// <summary>移動量</summary>
            public Vector2 MoveDelta{get{return moveDelta;}set{moveDelta = value;}}
            
            private List<GraphElement> moveElements = new List<GraphElement>();
            /// <summary>移動した要素</summary>
            public List<GraphElement> MoveElements{get{return moveElements;}}
            
            private List<Edge> createEdges = new List<Edge>();
            /// <summary>生成したエッジ</summary>
            public List<Edge> CreateEdges{get{return createEdges;}}
            
            private List<AdvGraphNode> createNodes = new List<AdvGraphNode>();
            /// <summary>生成したノード</summary>
            public List<AdvGraphNode> CreateNodes{get{return createNodes;}}
            
            private List<GraphElement> removeElements = new List<GraphElement>();
            /// <summary>削除した要素</summary>
            public List<GraphElement> RemoveElements{get{return removeElements;}}
        }
        
        public void SetEditor(AdvEditor editor)
        {
            this.editor = editor;
            if(undoData == null)
            {
                undoData = ScriptableObject.CreateInstance<AdvEditorUndoRedoData>();
            }
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }


        
        // Editor
        private AdvEditor editor = null;
        // UndoData
        [SerializeField]
        private AdvEditorUndoRedoData undoData = null;

        // id
        [SerializeField]
        private int allocateId = 0;
        // 現在のId
        [SerializeField]
        private int currentId = 0;
        
        
        private bool isDirty = false;
        
        private List<AdvEditorUndoRedoData.UndoNodePosition> nodePositions = new List<AdvEditorUndoRedoData.UndoNodePosition>();
        private List<AdvEditorUndoRedoData.UndoNodeCreate> nodeCreates = new List<AdvEditorUndoRedoData.UndoNodeCreate>();
        private List<AdvEditorUndoRedoData.UndoNodeDelete> nodeDeletes = new List<AdvEditorUndoRedoData.UndoNodeDelete>();
        private List<AdvEditorUndoRedoData.UndoEdgeCreate> edgeCreates = new List<AdvEditorUndoRedoData.UndoEdgeCreate>();
        private List<AdvEditorUndoRedoData.UndoEdgeDelete> edgeDeletes = new List<AdvEditorUndoRedoData.UndoEdgeDelete>();
        private List<AdvEditorUndoRedoData.UndoEdgeDelete> edgeSystemDeletes = new List<AdvEditorUndoRedoData.UndoEdgeDelete>();
        
        private void RegisterUndoData()
        {
            undoData.nodePositions.Clear();
            undoData.nodeCreates.Clear();
            undoData.nodeDeletes.Clear();
            undoData.edgeCreates.Clear();
            undoData.edgeDeletes.Clear();
            undoData.edgeSystemDeletes.Clear();
            
            undoData.nodePositions.AddRange(nodePositions);
            undoData.nodeCreates.AddRange(nodeCreates);
            undoData.nodeDeletes.AddRange(nodeDeletes);
            undoData.edgeCreates.AddRange(edgeCreates);
            undoData.edgeDeletes.AddRange(edgeDeletes);
            undoData.edgeSystemDeletes.AddRange(edgeSystemDeletes);
            
           nodePositions.Clear();
           nodeCreates.Clear();
           nodeDeletes.Clear();
           edgeCreates.Clear();
           edgeDeletes.Clear();
           edgeSystemDeletes.Clear();
            
            // 現在の状態をUndoに登録
            Undo.RegisterCompleteObjectUndo(undoData, "AdvEditor");
            // Id
            undoData.id = ++allocateId;
            // 現在のId
            currentId = undoData.id;
        }
        
        /// <summary>UndoRedo時のコールバック</summary>
        private void UndoRedoPerformed()
        {
            
            // 再描画
            editor.Repaint();
            // データの変更チェック
            if(currentId == undoData.id)return;
            // Undo?
            bool isUndo = currentId > undoData.id;
            // Idを保持
            currentId = undoData.id;
            
            
            // ノードの移動
            foreach(AdvEditorUndoRedoData.UndoNodePosition nodePosition in undoData.nodePositions)
            {
                // ノード取得
                AdvCommandNode node = editor.GraphView.GetNode(nodePosition.nodeId);
                if(node == null)continue;
                // 座標を戻す
                Vector2 p = nodePosition.position;
                if(isUndo)p -= undoData.moveDelta;
                node.SetPosition(p);
            }
            
            // ノードの生成
            foreach(AdvEditorUndoRedoData.UndoNodeCreate nodeCreate in undoData.nodeCreates)
            {
                if(isUndo)
                {
                    editor.GraphView.DeleteNode(nodeCreate.node.NodeId);
                }
                else
                {
                    editor.GraphView.CreateNode(nodeCreate.node, nodeCreate.position);
                }
            }
            
            // ノードの削除
            foreach(AdvEditorUndoRedoData.UndoNodeDelete nodeRemove in undoData.nodeDeletes)
            {
                if(isUndo)
                { 
                    // 削除したノードを戻す
                    editor.GraphView.CreateNode(nodeRemove.node, nodeRemove.node.NodePosition);
                }
                else
                {
                    // ノードを消す
                    editor.GraphView.DeleteNode(nodeRemove.node.NodeId);
                }
            }
            
            // Edgeの削除
            foreach(AdvEditorUndoRedoData.UndoNodeDelete nodeRemove in undoData.nodeDeletes)
            {
                if(isUndo)
                { 
                    
                    // 削除したEdge
                    foreach(ulong node in nodeRemove.node.OutputNodes)
                    {
                        editor.GraphView.ConnectPort(nodeRemove.node.NodeId, node);
                    }
                    
                    // 自身に接続しているノード
                    foreach(ulong connectionNodeId in nodeRemove.connectionNodes)
                    {
                        editor.GraphView.ConnectPort(connectionNodeId, nodeRemove.node.NodeId);
                    }
                }
            }
            
            // Edgeの生成
            foreach(AdvEditorUndoRedoData.UndoEdgeCreate edgeCreate in undoData.edgeCreates)
            {
                if(isUndo)
                {
                    // Edgeを探して削除する
                    AdvCommandNode inputNode = editor.GraphView.GetNode(edgeCreate.inputNodeId);
                    AdvCommandNode outputNode = editor.GraphView.GetNode(edgeCreate.outputNodeId);
                    if(inputNode == null || outputNode == null)continue;
                    foreach(Edge edge in outputNode.OutputPort.connections)
                    {
                        
                        if( ((AdvGraphNode)edge.input.node).CommandNode.NodeId == inputNode.NodeId)
                        {
                            editor.GraphView.DeleteEdge(edge);
                            break;
                        }
                    }
                }
                else
                {
                    // ノードをつなぎ直す
                    editor.GraphView.ConnectPort(edgeCreate.outputNodeId, edgeCreate.inputNodeId);
                }
            }
            
            
            // Edgeの削除
            List<AdvEditorUndoRedoData.UndoEdgeDelete> edgeDeletes = new List<AdvEditorUndoRedoData.UndoEdgeDelete>();
            edgeDeletes.AddRange(undoData.edgeDeletes);
            edgeDeletes.AddRange(undoData.edgeSystemDeletes);
            foreach(AdvEditorUndoRedoData.UndoEdgeDelete edgeRemove in edgeDeletes)
            {
                if(isUndo)
                { 
                    // ノードをつなぎ直す
                    editor.GraphView.ConnectPort(edgeRemove.outputNodeId, edgeRemove.inputNodeId);
                }
                else
                {
                    // Edgeを探して削除する
                    AdvCommandNode inputNode = editor.GraphView.GetNode(edgeRemove.inputNodeId);
                    AdvCommandNode outputNode = editor.GraphView.GetNode(edgeRemove.outputNodeId);

                    if(inputNode == null || outputNode == null)continue;
                    
                    foreach(Edge edge in outputNode.OutputPort.connections)
                    {
                        if( ((AdvGraphNode)edge.input.node).CommandNode.NodeId == inputNode.NodeId)
                        {
                            editor.GraphView.DeleteEdge(edge);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>生成登録</summary>
        public void RegisterCreateNode(ulong nodeId, Vector2 position)
        {
            // ノードを取得
            AdvCommandNode node = editor.GraphView.GetNode(nodeId);
            if(node == null)return;
            nodeCreates.Clear();
            // 生成データ
            AdvEditorUndoRedoData.UndoNodeCreate undoCreate = new AdvEditorUndoRedoData.UndoNodeCreate();
            undoCreate.node = node;
            undoCreate.position = position;
            nodeCreates.Add(undoCreate);
            
            isDirty = true;
        }
        
        /// <summary>削除登録</summary>
        public void RegisterDelete(List<ISelectable> elements)
        {
            nodeDeletes.Clear();
            edgeDeletes.Clear();
            
            foreach(ISelectable e in elements)
            {
                // ノードの削除
                if(e is AdvGraphNode node)
                {
                    node.CommandNode.UpdateOutputNodes();
                    AdvEditorUndoRedoData.UndoNodeDelete nodeDelete = new AdvEditorUndoRedoData.UndoNodeDelete();
                    // ノードを保持
                    nodeDelete.node = node.CommandNode;
                    // リストに追加
                    nodeDeletes.Add(nodeDelete);
                    
                    // 接続しているノードを取得
                    foreach(AdvCommandNode n in editor.GraphView.CommandNodes)
                    {
                        n.UpdateOutputNodes();
                        foreach(ulong outputNodeId in n.OutputNodes)
                        {
                            if(node.CommandNode.NodeId == outputNodeId)
                            {
                                nodeDelete.connectionNodes.Add(n.NodeId);
                                break;
                            }
                        }
                    }
                }

                
                // Edge
                if(e is Edge edge)
                {
                    AdvEditorUndoRedoData.UndoEdgeDelete edgeDelete = new AdvEditorUndoRedoData.UndoEdgeDelete();
                    // ノードを保持
                    edgeDelete.inputNodeId = ((AdvGraphNode)edge.input.node).CommandNode.NodeId;
                    edgeDelete.outputNodeId = ((AdvGraphNode)edge.output.node).CommandNode.NodeId;
                    
                    // リストに追加
                    edgeDeletes.Add(edgeDelete);
                }
            }
            
            isDirty = true;
        }
        
        /// <summary>登録</summary>
        public void Register(ChangeData change)
        {
            // 移動量
            undoData.moveDelta = change.MoveDelta;
            // 移動ノードの記録
            if(change.MoveElements.Count > 0)
            {
                nodePositions.Clear();
                
                foreach(GraphElement e in change.MoveElements)
                {
                    if(e is AdvGraphNode node)
                    {
                        AdvEditorUndoRedoData.UndoNodePosition nodePosition = new AdvEditorUndoRedoData.UndoNodePosition();
                        nodePosition.nodeId = node.CommandNode.NodeId;
                        nodePosition.position = node.CommandNode.NodePosition;
                        nodePositions.Add(nodePosition);
                    }
                }
            }
            
            if(change.CreateEdges.Count > 0)
            {
                edgeCreates.Clear();
                foreach(Edge edge in change.CreateEdges)
                {
                    AdvGraphNode inputNode = (AdvGraphNode)edge.input.node;
                    AdvGraphNode outputNode = (AdvGraphNode)edge.output.node;
                    
                    AdvEditorUndoRedoData.UndoEdgeCreate edgeCreate = new AdvEditorUndoRedoData.UndoEdgeCreate();
                    edgeCreate.inputNodeId = inputNode.CommandNode.NodeId;
                    edgeCreate.outputNodeId = outputNode.CommandNode.NodeId;
                    edgeCreates.Add(edgeCreate);
                }
            }
            
            if(change.CreateNodes.Count > 0)
            {
                nodeCreates.Clear();
                foreach(AdvGraphNode node in change.CreateNodes)
                {
                    // 生成データ
                    AdvEditorUndoRedoData.UndoNodeCreate undoCreate = new AdvEditorUndoRedoData.UndoNodeCreate();
                    undoCreate.node = node.CommandNode;
                    undoCreate.position = node.CommandNode.NodePosition;
                    nodeCreates.Add(undoCreate);
                }
                
            }
            
            if(change.RemoveElements.Count > 0)
            {
                edgeSystemDeletes.Clear();
                foreach(GraphElement e in change.RemoveElements)
                {
                    if(e is Edge edge)
                    {
                        AdvGraphNode inputNode = (AdvGraphNode)edge.input.node;
                        AdvGraphNode outputNode = (AdvGraphNode)edge.output.node;
                        
                        AdvEditorUndoRedoData.UndoEdgeDelete edgeDelete = new AdvEditorUndoRedoData.UndoEdgeDelete();
                        edgeDelete.inputNodeId = inputNode.CommandNode.NodeId;
                        edgeDelete.outputNodeId = outputNode.CommandNode.NodeId;
                        edgeSystemDeletes.Add(edgeDelete);
                    }
                }
            }
            
            isDirty = true;
        }
        
        internal void Update()
        {
            if(isDirty)
            {
                isDirty = false;
                RegisterUndoData();
            }
            
        }
    }
}