using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.UI;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class PrizeJsonPoolListItem : PoolListItemBase
    {
        public class ItemParams : ItemParamsBase
        {
            public List<Networking.App.Request.PrizeJsonWrap> requestPrizeList;
            public List<Master.PrizeJsonWrap> masterPrizeList;

            public ItemParams(List<Master.PrizeJsonWrap> _wrapList)
            {
                masterPrizeList = _wrapList;
                requestPrizeList = new List<Networking.App.Request.PrizeJsonWrap>();
            }

            public ItemParams(List<Networking.App.Request.PrizeJsonWrap> _wrapList)
            {
                requestPrizeList = _wrapList;
                masterPrizeList = new List<Master.PrizeJsonWrap>();
            }
        }
        
        #region SerializeFields
        [SerializeField] private Transform categoryContent;
        [SerializeField] private PrizeJsonView itemPrefab;

        private ItemParams info;
        private List<PrizeJsonView> itemCacheList = new List<PrizeJsonView>();
        
        #endregion
        
        #region OverrideMethods
        public override void Init(ItemParamsBase param)
        {
            info = param as ItemParams;
            UpdateCategoryContent();
            base.Init(param);
        }
        #endregion

        #region PrivateMethods
        private void UpdateCategoryContent()
        {
            for (int i = itemCacheList.Count; i < info.masterPrizeList.Count; i++)
            {
                itemCacheList.Add(Instantiate(itemPrefab,categoryContent));
            }
           
            for (int i = 0; i < info.masterPrizeList.Count || i < itemCacheList.Count; i++)
            {
                if (i >= info.masterPrizeList.Count) 
                {
                    itemCacheList[i].gameObject.SetActive(false);
                    continue;
                }
                
                itemCacheList[i].SetView(info.masterPrizeList[i]);
                itemCacheList[i].gameObject.SetActive(true);
            }

            var currentCount = info.masterPrizeList.Count;

            for (int i = itemCacheList.Count; i < info.requestPrizeList.Count; i++)
            {
                itemCacheList.Add(Instantiate(itemPrefab,categoryContent));
            }
           
            for (int i = 0; i < info.requestPrizeList.Count || i < itemCacheList.Count; i++)
            {
                var j = i + currentCount;
                if (i >= info.requestPrizeList.Count) 
                {
                    itemCacheList[j].gameObject.SetActive(false);
                    continue;
                }
                
                itemCacheList[j].SetView(info.requestPrizeList[i]);
                itemCacheList[j].gameObject.SetActive(true);
            }
        }
        #endregion
    }
}