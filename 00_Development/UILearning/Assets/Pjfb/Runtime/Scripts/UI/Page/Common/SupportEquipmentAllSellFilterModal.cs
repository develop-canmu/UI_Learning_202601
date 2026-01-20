using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.Character;
using  Pjfb.UserData;

namespace Pjfb
{
    public class SupportEquipmentAllSellFilterModal : SupportEquipmentSortFilterModal
    {
        [Serializable]
        public class SellData : Data
        {
            private Action onSellCallback;
            public Action OnSellCallback
            {
                get { return onSellCallback; }
            }
            public SellData(SortFilterSheetType _sheetType,SortFilterUtility.SortFilterType _sortFilterType,Action onsellcallback) : base(_sheetType,_sortFilterType)
            {
                this.onSellCallback = onsellcallback;
            }
        }
        
        private HashSet<long> filterExcludeIdSet;
        private HashSet<long> formattingIdHashSet = new HashSet<long>();
 
        public void OnClickAllSellButton()
        {
            var filterData = CreateFilterData();
            //フィルターデータの保存
            SortFilterUtility.SaveFilterData(ModalData.sortFilterType, filterData);
            LocalSaveManager.Instance.SaveData();
            var selectId = GetSelectDataId();

            var selldata = (SellData) ModalData;
            
            if (selectId.Length > 0)
            {
                SupportEquipmentSellConfirmModalWindow.Open(new SupportEquipmentSellConfirmModalWindow.WindowParams()
                {
                    idList = selectId,
                    onConfirm =  selldata.OnSellCallback
                });
            }
            //フィルターデータが存在しない場合のモーダルを表示
            else
            {
                ConfirmModalWindow.Open(new ConfirmModalData()
                {
                    Title = StringValueAssetLoader.Instance["character.support_equipment_.sell_error_title"],
                    Message = StringValueAssetLoader.Instance["character.support_equipment_.sell_error_message"],
                    PositiveButtonParams = new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["common.close"],
                        window => { window.Close(); })
                });
            }
        }

        private long[] GetSelectDataId()
        {
            //デッキに装備されているID
            formattingIdHashSet = DeckUtility.GetSupportEquipmentIds().ToHashSet();
            // ユーザーが所持しているリスト
            List<UserDataSupportEquipment> equipmentList = new List<UserDataSupportEquipment>(UserDataManager.Instance.supportEquipment.data.Values);
            
            // 編成中のリスト
            List<UserDataSupportEquipment> formattingEquipmentList = new List<UserDataSupportEquipment>();
            //デッキに装備されているサポート器具を抽出
            foreach(long id in formattingIdHashSet)
            {
                UserDataSupportEquipment uEquipment = UserDataManager.Instance.supportEquipment.Find(id);
                formattingEquipmentList.Add(uEquipment);
                equipmentList.Remove(uEquipment);
            }

             var selectList = equipmentList.Where((data) => !data.isLocked).ToList();
            equipmentList = ApplySortFilter(selectList);
    
            return equipmentList.Select(m => m.id).ToArray();
        }
        
        private List<UserDataSupportEquipment> ApplySortFilter(List<UserDataSupportEquipment> userDataSupportEquipmentList)
        {
            var list = new List<UserDataSupportEquipment>(userDataSupportEquipmentList);

            list = list.GetAllSellFilterSupportEquipmentList(ModalData.sortFilterType, filterExcludeIdSet);
            return list;
        }
    }

}


