using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ListContainer : MonoBehaviour {
        #region SerializeFields
        [SerializeField] private Transform listParent;
        [SerializeField] private ListItemBase itemPrefab;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private int verticalSpacing = 15;
        #endregion

        #region PrivateFields
        public List<ListItemBase> listItems { get; private set; }
        #endregion

        #region PublicMethods
        public void SetDataList<T>(List<T> itemParams, float scrollValue = 1) where T:ListItemBase.ItemParamsBase
        {
            verticalLayoutGroup.spacing = verticalSpacing;
            
            listItems?.ForEach(anItem => Destroy(anItem.gameObject));
            listItems = new List<ListItemBase>();
            foreach (var anItemParam in itemParams)
            {
                var instance = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, listParent);
                instance.gameObject.SetActive(true);
                instance.Init(anItemParam);
                listItems.Add(instance);
            }

            SetScrollValue(scrollValue);
        }
        #endregion

        #region OverrideMethods
        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        #endregion
        
        #region PrivateMethods
        /// <summary>
        /// 注意：スクロールバリュー設定は２フレームを待たないといけない場合があるため、２フレームの間は非表示にする
        /// </summary>
        private async void SetScrollValue(float scrollValue)
        {
            canvasGroup.alpha = 0;
            await UniTask.DelayFrame(2);
            if (gameObject == null) return;
            
            canvasGroup.alpha = 1;
            scrollRect.verticalScrollbar.value = scrollValue;
        }
        #endregion
    }
}