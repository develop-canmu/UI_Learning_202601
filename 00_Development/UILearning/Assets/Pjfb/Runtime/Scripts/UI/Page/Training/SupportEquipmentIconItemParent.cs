using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Character;

namespace Pjfb
{
    public class SupportEquipmentIconItemParent : MonoBehaviour
    {
        public class SupportEquipmentIconItemData
        {
            // アイコンのデータ
            private SupportEquipmentDetailData detailData;
            public SupportEquipmentDetailData DetailData => detailData;
            
            private SwipeableParams<SupportEquipmentDetailData> swipeParams;
            public SwipeableParams<SupportEquipmentDetailData> SwipeableParams => swipeParams;
            
            public SupportEquipmentIconItemData(SupportEquipmentDetailData equipmentDetailData, SwipeableParams<SupportEquipmentDetailData> swipeableParams)
            {
                detailData = equipmentDetailData;
                swipeParams = swipeableParams;
            }
        }
        
        [SerializeField]
        private SupportEquipmentIcon iconBase = null;
        
        private SupportEquipmentIcon[] equipmentItemArray = null;
        
        /// <summary>アイテムの生成</summary>
        public void CreateItem(List<SupportEquipmentIconItemData> dataList)
        {
            ResetItemArray();
            equipmentItemArray = new SupportEquipmentIcon[dataList.Count];
            for (int i = 0; i < dataList.Count; i++)
            {
                SupportEquipmentIcon equipmentItem = GameObject.Instantiate(iconBase, this.gameObject.transform);
                equipmentItem.SetIconAsync(dataList[i].DetailData).Forget();
                equipmentItem.SwipeableParams = dataList[i].SwipeableParams;
                equipmentItem.gameObject.SetActive(true);
                equipmentItemArray[i] = equipmentItem;
            }
        }

        /// 要素のリセット
        private void ResetItemArray()
        {
            if (equipmentItemArray != null)
            {
                foreach (SupportEquipmentIcon item in equipmentItemArray)
                {
                    Destroy(item.gameObject);
                }
                equipmentItemArray = null;
            }
        }
        
        private void OnDestroy()
        {
            ResetItemArray();
        }
        
    }
}