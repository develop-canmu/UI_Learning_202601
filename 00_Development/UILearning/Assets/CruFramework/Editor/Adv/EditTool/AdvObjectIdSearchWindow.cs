using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;
using UnityEditor;

namespace CruFramework.Editor.Adv
{
    
    public class AdvObjectIdSearchWindow : AdvSearchWindow
    {
        public static AdvObjectIdSearchWindow Create(string title, List<GUIContent> menuList, int[] index, Action<int> onSelected)
        {
            AdvObjectIdSearchWindow window = ScriptableObject.CreateInstance<AdvObjectIdSearchWindow>();
            window.title = title;
            window.menuList = menuList;
            window.onSelected = onSelected;
            window.index = index;
            return window;
        }
        
        private string title = string.Empty;
        /// <summary>タイトル</summary>
        protected override string TreeName{get{return title;}}
        
        private List<GUIContent> menuList = null;
        private int[] index = null;
        private Action<int> onSelected = null;

        protected override Dictionary<string, object> GetTreeDatas()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            for(int i=0;i<menuList.Count;i++)
            {
                result.Add(menuList[i].text, i);
            }
            return result;
        }

        protected override void OnSelectEntry(object value, SearchWindowContext context)
        {
            onSelected( index[(int)value] );
        }
        
        
        public void Open(Vector2 position)
        {
            SearchWindow.Open(new SearchWindowContext(position), this);
        }
    }
}