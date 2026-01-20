using System.Collections.Generic;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class HelpCategoryScrollItem : ScrollDynamicItem
    {
        #region Params
        public class Data 
        {
            public string title;
            public List<HelpModalWindow.MHelp> ItemList;
            public bool defaultShow = false;
        }

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private RectTransform categoryContent;
        [SerializeField] private HelpItem itemPrefab;
        [SerializeField] private Image arrowImage;
        [SerializeField] private Image backgroundImage;
        
        private List<HelpItem> cacheList = new List<HelpItem>();
        private Data info;
        
        #endregion
       
        
        #region OverrideMethods
        
        protected override void OnSetView(object value)
        {
            info = value as Data;
            if(info == null) return;
            titleText.text = info.title;
            SetContentActive(info.defaultShow);
            UpdateItemContent();
            RefreshViewSize();
        }

        #endregion

        #region PrivateMethods

        private void UpdateItemContent()
        {
            for (int i = cacheList.Count; i < info.ItemList.Count; i++)
            {
                cacheList.Add(Instantiate(itemPrefab,categoryContent));
            }
           
            for (int i = 0; i < info.ItemList.Count || i < cacheList.Count; i++)
            {
                if (i >= info.ItemList.Count) 
                {
                    cacheList[i].gameObject.SetActive(false);
                    continue;
                }
                
                cacheList[i].Init(info.ItemList[i]);
                cacheList[i].gameObject.SetActive(true);
            }
        }

        private void SetContentActive(bool active)
        {
            categoryContent.gameObject.SetActive(active);
            backgroundImage.enabled = active;
            arrowImage.transform.localScale = (active) ? new Vector3(1,-1,1) : Vector3.one;
        }

        private void RefreshViewSize()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(UITransform);
            RecalculateSize();
        }
        
        #endregion
        
        #region EventListeners

        public void OnClickCategory()
        {
            bool showContent = !categoryContent.gameObject.activeSelf;
            info.defaultShow = showContent;
            SetContentActive(showContent);
            RefreshViewSize();
        }

        #endregion
    }
}