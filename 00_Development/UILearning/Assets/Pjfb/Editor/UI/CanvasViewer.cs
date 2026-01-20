using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEditor;

using System.Linq;

namespace Pjfb.Editor
{
    public class CanvasViewer : EditorWindow
    {
        [MenuItem("Pjfb/CanvasViewer")]
        public static void Open()
        {
            GetWindow<CanvasViewer>();
        }
        
        public enum SortType
        {
            Order, Layer
        }
        
        /// <summary>名前表示幅</summary>
        private const float NameFieldWidth = 100;
        /// <summary>レイヤー表示幅</summary>
        private const float LayerFieldWidth = 100;
        /// <summary>オーダー表示幅</summary>
        private const float OrderFieldWidth = 60;
        
        // 収集したキャンバス
        private Canvas[] canvases = null;
        // スクロール量
        private Vector2 scrollValue = Vector2.zero;
        // 自動更新
        private bool isAutoUpdate = true;
        // ソート
        private SortType sortType = SortType.Order;
        
        private void UpdateCanvasList()
        {
            canvases = GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        }
        
        private void OnGUI()
        {
            // ヘッダー部分
            EditorGUILayout.BeginHorizontal();
            // 自動更新
            isAutoUpdate = EditorGUILayout.Toggle("AutoUpdate", isAutoUpdate);
            // 更新
            if(isAutoUpdate == false)
            {
                if(GUILayout.Button("Update"))
                {
                    UpdateCanvasList();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            // キャンバスリスト更新
            if(canvases == null || isAutoUpdate)
            {
                UpdateCanvasList();
            }
            // キャンバスなし
            if(canvases == null || canvases.Length <= 0)return;
            
            Color guiColor = GUI.color;
            EditorGUILayout.BeginVertical();
            scrollValue = EditorGUILayout.BeginScrollView(scrollValue);
            
            EditorGUILayout.BeginHorizontal();
            // 名前
            EditorGUILayout.LabelField("Name", GUILayout.Width(NameFieldWidth));
            // レイヤー
            GUI.color = sortType == SortType.Layer ? Color.green : guiColor;
            if(GUILayout.Button("Layer", GUILayout.Width(LayerFieldWidth)))
            {
                sortType = SortType.Layer;
            }
            // オーダー
            GUI.color = sortType == SortType.Order ? Color.green : guiColor;
            if(GUILayout.Button("Order", GUILayout.Width(OrderFieldWidth)))
            {
                sortType = SortType.Order;
            }
            // オーダーインレイヤー
            GUI.color = guiColor;
            EditorGUILayout.LabelField("InLayer", GUILayout.Width(OrderFieldWidth));
            EditorGUILayout.EndHorizontal();
            
            
            // ソート
            IEnumerable<Canvas> sortedList = null;

            switch(sortType)
            {
                case SortType.Order:
                {
                    sortedList = canvases.OrderBy(v=>v == null ? int.MaxValue : v.renderOrder);
                    break;
                }
                
                case SortType.Layer:
                {
                    sortedList = canvases.
                        // レイヤー順
                        OrderBy(v=>v == null ? int.MaxValue : v.sortingLayerID).
                        // オーダー順
                        ThenBy(v=>v == null ? int.MaxValue : v.renderOrder);
                    break;
                }
            }
            
            
            
            // 表示
            foreach(Canvas c in sortedList)
            {
                if(c == null)continue;
                
                EditorGUILayout.BeginHorizontal("TextArea");
                
                // GameObject
                if(GUILayout.Button(c.name, GUILayout.Width(NameFieldWidth)))
                {
                    Selection.activeGameObject = c.gameObject;
                }
                // レイヤー名
                EditorGUILayout.LabelField(c.sortingLayerName, GUILayout.Width(LayerFieldWidth));
                // オーダー値
                EditorGUILayout.LabelField(c.renderOrder.ToString(), GUILayout.Width(OrderFieldWidth));
                
                // オーダーインレイヤー値
                EditorGUILayout.LabelField(c.sortingOrder.ToString(), GUILayout.Width(OrderFieldWidth));
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}