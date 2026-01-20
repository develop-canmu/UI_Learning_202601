using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Adv
{
    [System.Serializable]
    public abstract class AdvCommandNode
    {
        
        private class Inspector : ScriptableObject
        {
            [SerializeReference]
            public AdvCommandNode parameters = null;
        }
        
        
        [SerializeField][HideInInspector]
        private ulong nodeId = 0;
        /// <summary>NodeId</summary>
        public ulong NodeId{get{return nodeId;}}
        
        [SerializeField][HideInInspector]
        private Vector2 nodePosition = Vector2.zero;
        /// <summary>ノードの位置</summary>
        public Vector2 NodePosition{get{return nodePosition;}}
        
        [SerializeField][HideInInspector]
        private List<ulong> outputNodes = new List<ulong>();
        /// <summary>出力ノード</summary>
        public List<ulong> OutputNodes{get{return outputNodes;}}

        /// <summary>Inのキャパシティ</summary>
        protected virtual Port.Capacity InputPortCapacity{get{return Port.Capacity.Single;}}
        /// <summary>Outのキャパシティ</summary>
        protected virtual Port.Capacity OutputPortCapacity{get{return Port.Capacity.Single;}}
        
        /// <summary>入力あり</summary>
        public virtual bool HasInputPort{get{return true;}}
        /// <summary>出力あり</summary>
        public virtual bool HasOutputPort{get{return true;}}
        
        /// <summary>コマンドを取得</summary>
        protected abstract List<IAdvCommandObject> GetCommands();
        
        private Port inputPort = null;
        /// <summary>入力ポート</summary>
        public Port InputPort{get{return inputPort;}}
        
        private Port outputPort = null;
        /// <summary>出力ポート</summary>
        public Port OutputPort{get{return outputPort;}}
        
        
        private AdvGraphNode node = null;
        /// <summary>Node</summary>
        public AdvGraphNode Node{get{return node;}}
        
        private string typeName = string.Empty;
        
        private StyleColor defaultHeaderColor;
        
        /// <summary>パラメータの検索ヒット</summary>
        protected virtual bool IsHitSearchParameters(string text)
        {
            return false;
        }

        /// <summary>検索に引っかかるか</summary>
        public bool IsHitSearchText(string text)
        {
            if(string.IsNullOrEmpty(text))return false;
            return typeName.Contains(text) || IsHitSearchParameters(text);
        }
        
        public void SetHeaderColor(Color color)
        {
            node.titleContainer.style.backgroundColor = color;
        }
        
        public void BeginHighlight()
        {
            node.BeginHighlight();
        }
        
        public void EndHighlight()
        {
            node.EndHighlight();

        }

        public void SetDefaultHeaderColor()
        {
            node.titleContainer.style.backgroundColor = defaultHeaderColor;
        }
        
        /// <summary>接続先のノードを取得</summary>
        public AdvCommandNode[] GetNextNodes()
        {
            List<AdvCommandNode> result = new List<AdvCommandNode>();
            
            if(HasOutputPort)
            {
                foreach(Edge edge in outputPort.connections)
                {
                    if(edge.input.node is AdvGraphNode node)
                    {
                        result.Add(node.CommandNode);
                    }
                }
                
                // ノードを上から順番にソート
                result.Sort((v1, v2)=>v1.node.GetPosition().position.y.CompareTo(v2.node.GetPosition().position.y));
            }
            
            return result.ToArray();
        }
        
        /// <summary>接続元のノードを取得</summary>
        public AdvCommandNode[] GetPreNodes()
        {
            List<AdvCommandNode> result = new List<AdvCommandNode>();
            
            if(HasOutputPort)
            {
                foreach(Edge edge in inputPort.connections)
                {
                    if(edge.output.node is AdvGraphNode node)
                    {
                        result.Add(node.CommandNode);
                    }
                }
                
                // ノードを上から順番にソート
                result.Sort((v1, v2)=>v1.node.GetPosition().position.y.CompareTo(v2.node.GetPosition().position.y));
            }
            
            return result.ToArray();
        }
        
        internal List<IAdvCommandObject> GetCommandsInternal()
        {
            return GetCommands();
        }

        internal void SetNodeId(ulong nodeId)
        {
            this.nodeId = nodeId;
        }
        
        /// <summary>保存前の処理</summary>
        internal void OnPreSave()
        {
            //UpdateNodePosition();
            UpdateOutputNodes();

        }
        
        internal void UpdateNodePosition()
        {
            nodePosition = node.GetPosition().position;
        }
        
        internal void UpdateOutputNodes()
        {
            outputNodes.Clear();
            
            if(HasOutputPort) 
            {
                // 出力ノード
                foreach(Edge edge in outputPort.connections)
                {
                    if(edge.input.node is AdvGraphNode advNode)
                    {
                        outputNodes.Add( advNode.CommandNode.nodeId );
                    }
                }
            }
        }
        
        internal void Initialize()
        {
            node = new AdvGraphNode(this);
            // タイトルから取り除き文字列
            string removeStr = nameof(AdvGraphNode);
            // タイトル
            node.title = GetType().Name + $" [{nodeId}]";
            // 不要な文字を削除
            if(node.title.StartsWith(removeStr))node.title = node.title.Substring(removeStr.Length);
            
            // タイプ名
            typeName = node.title.ToLower();
            // デフォルトの色
            defaultHeaderColor = node.titleContainer.style.backgroundColor;
            
            // ノードのサイズ
            if(this is IAdvGraphNodeWidth w)
            {
                node.style.width = w.NodeWidth;
            }
            else
            {
                node.style.width = 300.0f;
            }
            
            
            
            
            // 入力ポートの生成
            if(HasInputPort)
            {
                inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, InputPortCapacity, typeof(Port));
                inputPort.portName = "In";
                node.inputContainer.Add(inputPort);
            }
            
            // 出力ポートの生成
            if(HasOutputPort)
            {
                outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, OutputPortCapacity, typeof(Port));
                outputPort.portName = "Next";
                node.outputContainer.Add( outputPort);
            }

            Inspector inspector = null;
            UnityEditor.Editor inspectorElement = null;
            
            IMGUIContainer gui = new IMGUIContainer(()=>
            {
                if(inspector == null)
                {
                    inspector = ScriptableObject.CreateInstance<Inspector>();
                    inspector.parameters = this;
                    inspectorElement = UnityEditor.Editor.CreateEditor( inspector );
                }
                inspectorElement.OnInspectorGUI();
            });
            
            gui.style.marginBottom = 8.0f;
            gui.style.marginTop = 8.0f;
            gui.style.marginLeft = 8.0f;
            gui.style.marginRight = 8.0f;

            node.contentContainer.Add( gui );
            
        }
        
        public void SetPosition(Vector2 position)
        {
            Rect rect = node.GetPosition();
            rect.position = position;
            nodePosition = position;
            node.SetPosition(rect);
        }

                
        internal void OnUpdate()
        {
            node.OnUpdate();
        }
    }
}