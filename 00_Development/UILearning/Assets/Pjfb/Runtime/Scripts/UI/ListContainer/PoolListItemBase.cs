using System;
using UnityEngine;

namespace Pjfb.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class PoolListItemBase : MonoBehaviour
    {
        #region ItemParams
        public class ItemParamsBase
        {
            public int itemPosition;
            public int itemHeight;
            public PoolListItemBase nullableActiveListItem;

            public void SetItemPositionHeight(int itemPosition, int itemHeight)
            {
                this.itemPosition = itemPosition;
                this.itemHeight = itemHeight;
            }

            public virtual int GetItemHeight (PoolListItemBase poolListItemBase, int prefabHeight) => poolListItemBase.GetItemHeight(prefabHeight, this);

            public void ActivePoolListItemInvoke<T>(Action<T> action) where T : PoolListItemBase
            {
                if (nullableActiveListItem != null) action.Invoke(nullableActiveListItem as T);
            }
        }
        #endregion
        
        #region SerializeFields
        #endregion

        #region PublicProperties
        private RectTransform _rectTransform;
        public RectTransform rectTransform {
            get {
                if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }
        private CanvasGroup _canvasGroup;
        public CanvasGroup canvasGroup {
            get {
                if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }
        #endregion

        #region OverrideMethods
        public virtual void Init(ItemParamsBase itemParamsBase) { }
        
        /// <summary>
        /// 表示エリアから離れた際に実行される
        /// </summary>
        public virtual void Pooled() { }

        public virtual int GetItemHeight(int prefabHeight, ItemParamsBase itemParamsBase) => prefabHeight;
        #endregion

        #region PrivateMethods
        #endregion
    }
}