using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Adv
{
    public class AdvGraphNodeSearchWindow : AdvSearchWindow
    {
        
        public static AdvGraphNodeSearchWindow Create(AdvGraphView graphView, EditorWindow editorWindow)
        {
            // ノードの検索ウィンドウ
            AdvGraphNodeSearchWindow searchWindow = ScriptableObject.CreateInstance<AdvGraphNodeSearchWindow>();
            searchWindow.Initialize(graphView, editorWindow);
            return searchWindow;
        }
        
        private AdvGraphView graphView = null;
        private EditorWindow editorWindow = null;


        protected override string TreeName { get{return "Create Node";}}

        /// <summary>初期化</summary>
        private void Initialize(AdvGraphView graphView, EditorWindow editorWindow)
        {
            this.graphView = graphView;
            this.editorWindow = editorWindow;
        }

        protected override Dictionary<string, object> GetTreeDatas()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            // コマンドをリストに追加
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes())
                {
                    // 抽象クラスは省く
                    if(type.IsAbstract)continue;
                    //
                    if(type.IsSubclassOf(typeof(AdvCommandNode)))
                    {
                        string name = AdvGraphView.ToNodeName(type, true);
                        result.Add(name, type);
                    }
                }
            }
            
            return result;
        }

        protected override void OnSelectEntry(object value, SearchWindowContext context)
        {
            Type type = (Type)value;
            if(graphView.InsertNodeEdge != null)
            {
                graphView.InsertNode(type, graphView.InsertNodeEdge);
                graphView.InsertNodeEdge = null;
            }
            else
            {
                // 座標計算
                Vector2 windowMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);
                Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);
                graphView.CreateNode(type, graphMousePosition, true);
            }
        }
    }
}