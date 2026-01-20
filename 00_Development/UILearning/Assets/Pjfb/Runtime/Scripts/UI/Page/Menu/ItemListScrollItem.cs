using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UI;
using TMPro;
using UnityEngine;

namespace Pjfb.Menu
{
    public class ItemListScrollItem : ListItemBase
    {
        public class ItemParams : ItemParamsBase
        {
            public string Category;
            public List<ItemIconParams> CategoryItemList;
        }
        
        #region SerializeFields
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Transform categoryContent;
        [SerializeField] private ItemIconContainer itemPrefab;

        private ItemParams info;
        private List<ItemIconContainer> itemCacheList = new List<ItemIconContainer>();
        
        #endregion
        
        #region OverrideMethods
        public override void Init(ItemParamsBase param)
        {
            info = param as ItemParams;
            titleText.text = info.Category;
            UpdateCategoryContent();
            base.Init(param);
        }
        #endregion

        #region PrivateMethods
        private void UpdateCategoryContent()
        {
            for (int i = itemCacheList.Count; i < info.CategoryItemList.Count; i++)
            {
                itemCacheList.Add(Instantiate(itemPrefab,categoryContent));
            }
           
            for (int i = 0; i < info.CategoryItemList.Count || i < itemCacheList.Count; i++)
            {
                if (i >= info.CategoryItemList.Count) 
                {
                    itemCacheList[i].gameObject.SetActive(false);
                    continue;
                }
                
                //アイテムアイコン設定
                long iconId = info.CategoryItemList[i].iconId;
                switch ( info.CategoryItemList[i].iconType)
                {
                    case ItemIconType.Item:
                        itemCacheList[i].SetIcon(ItemIconType.Item,iconId);
                        break;
                    case ItemIconType.CharacterPiece:
                        itemCacheList[i].SetIcon(ItemIconType.CharacterPiece,iconId);
                        break;
                }
                itemCacheList[i].SetCount(info.CategoryItemList[i].count);
                itemCacheList[i].gameObject.SetActive(true);
            }
        }
        
        private void OnClickItemDetail(int id)
        {
            ItemConfirmModalWindow.Open(new ItemConfirmModalWindow.WindowParams { Id =  id});
        }

        #endregion
    }
}