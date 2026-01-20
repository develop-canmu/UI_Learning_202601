using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CruFramework.UI;
using Pjfb.UserData;

namespace Pjfb
{
    [RequireComponent(typeof(ScrollGrid))]
    public abstract class ItemIconScroll<T> : MonoBehaviour
    {
        
        [SerializeField]
        private bool refreshOnAwake = true;
        
        protected ScrollGrid scrollGrid = null;
        /// <summary>Scroll</summary>
        public ScrollGrid Scroll{get{return scrollGrid;}}
        
        /// <summary>アイテムリストの取得</summary>
        protected abstract List<T> GetItemList();
        
        /// <summary>選択時のコールバック</summary>
        public event Action<T> OnSelectedItem = null;
        
        /// <summary>Awake</summary>
        protected virtual void OnAwake(){}
        
        private List<T> itemList = null;
        /// <summary>リスト</summary>
        public List<T> ItemList{get{return itemList;}}
        
        // アイテムリスト
        private List<T> itemListSrc = null;
        /// <summary>リスト</summary>
        public IReadOnlyList<T> ItemListSrc{get{return itemListSrc;}}
        
        
        protected virtual void OnSort(CharacterScrollSortType sort){}
        protected virtual void OnFilter(CharacterScrollFilterType filter){}
        
        private void Awake()
        {
            scrollGrid = gameObject.GetComponent<ScrollGrid>();
            OnAwake();
            // 押下時イベント
            scrollGrid.OnItemEvent += OnSelectedItemEvent;
            
            if(refreshOnAwake)
            {
                Refresh();
            }
        }
        
        public void Refresh()
        {
            // リスト更新
            itemList = GetItemList();
            // 元のリストを保持しておくためにコピー
            itemListSrc = new List<T>(itemList);
            // Scrollにセット
            scrollGrid.SetItems( itemList );
        }
        
        private void OnSelectedItemEvent(ScrollGridItem item, object value)
        {
            if(OnSelectedItem != null)OnSelectedItem.Invoke( (T)value );
        }

    }
}