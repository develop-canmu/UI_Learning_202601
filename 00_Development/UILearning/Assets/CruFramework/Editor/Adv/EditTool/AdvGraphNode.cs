
using System.Timers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace CruFramework.Editor.Adv
{
    public sealed class AdvGraphNode : Node
    {
        /// <summary>ボーダーの色</summary>
        private static readonly Color BorderColor = Color.gray;
        /// <summary>ハイライト時のボーダーの色</summary>
        private static readonly Color HighlightBorderColor = Color.green;
        /// <summary>ハイライト終了時のボーダーの色</summary>
        private static readonly Color HighlightEndBorderColor = Color.cyan;
        
        /// <summary>ハイライトの時間</summary>
        private const float HighlightDuration = 2.0f;
        /// <summary>ハイライト終了時のフェード時間</summary>
        private const float HighlightFadeDuration = 0.5f;
        
        private AdvCommandNode commandNode = null;
        /// <summary>コマンドノード</summary>
        public AdvCommandNode CommandNode{get{return commandNode;}}
        
        private float highlightTimer = 0;
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Clear();
        }
        
        public void SetBorderColor(Color color)
        {
            style.borderBottomColor = color;
            style.borderTopColor = color;
            style.borderLeftColor = color;
            style.borderRightColor = color;
        }

        public AdvGraphNode(AdvCommandNode commandNode) : base()
        {
            this.commandNode = commandNode;
            
            style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
            
            // ボーダー
            style.borderBottomWidth = 4.0f;
            style.borderTopWidth = 4.0f;
            style.borderLeftWidth = 4.0f;
            style.borderRightWidth = 4.0f;
            
            SetBorderColor( BorderColor );
        }
        
        public void BeginHighlight()
        {
            SetBorderColor( HighlightBorderColor );
            highlightTimer = 0;
        }
        
        public void EndHighlight()
        {
            SetBorderColor( BorderColor );
            highlightTimer = Time.realtimeSinceStartup;
        }
        
        
        internal void OnUpdate()
        {
            if(highlightTimer > 0)
            {
                float time = Time.realtimeSinceStartup - highlightTimer;
                if(time >= HighlightDuration)
                {
                    highlightTimer = 0;
                }
                
                Color diff = (HighlightEndBorderColor - BorderColor);
                float v = 1.0f - Mathf.Min(1.0f, time / (HighlightDuration - HighlightFadeDuration));
                SetBorderColor( BorderColor + diff * v );
            }
        }
    }
}