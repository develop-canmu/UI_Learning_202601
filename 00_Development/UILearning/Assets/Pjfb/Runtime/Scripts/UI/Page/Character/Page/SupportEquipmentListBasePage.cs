using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public abstract class SupportEquipmentListBasePage : Page
    {
        #region SerializeFields
        [SerializeField] protected UserSupportEquipmentScroll scroll;
        #endregion

        protected HashSet<long> selectingHashSet = new HashSet<long>();

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            scroll.OnSelectSupportEquipment = OnSelectSupportEquipment;
            scroll.OnDeselectSupportEquipment = OnDeselectSupportEquipment;
            scroll.OnSortFilter = OnSortFilter;
            scroll.OnScrollInitialized = OnScrollInitialized;
            scroll.OnSwipeDetailModal = OnSwipeDetailModal;
            scroll.OnSellCompleted = OnSellCompleted;
            scroll.SetFormattingIds(DeckUtility.GetSupportEquipmentIds().ToHashSet());
            scroll.originFavoriteHashSet = selectingHashSet;
            
            scroll.InitializeScroll(selectingHashSet);
            await base.OnPreOpen(args, token);
        }

        protected virtual void OnScrollInitialized()
        {
            
        }
        

        protected virtual void OnSelectSupportEquipment(SupportEquipmentScrollData data)
        {
            OnSelectSupportEquipmentAsync(data).Forget();
        }

        protected virtual async UniTask OnSelectSupportEquipmentAsync(SupportEquipmentScrollData data)
        {
            await UniTask.CompletedTask;
        }

        protected virtual void OnDeselectSupportEquipment(SupportEquipmentScrollData data)
        {
            OnDeselectSupportEquipmentAsync(data).Forget();
        }

        protected virtual async UniTask OnDeselectSupportEquipmentAsync(SupportEquipmentScrollData data)
        {
            await UniTask.CompletedTask;
        }
        protected virtual void OnSortFilter(){}
        protected virtual void OnReverseSupportEquipmentOrder(){}
        
        protected SupportEquipmentScrollData GetItemById(long id)
        {
            return GetItems().FirstOrDefault(x => x.Id == id);
        }
        
        protected virtual List<SupportEquipmentScrollData> GetItems()
        {
            return scroll.Scroll.GetItems().Cast<SupportEquipmentScrollData>().ToList();
        }
        protected virtual void Refresh()
        {
            scroll.Refresh();
        }
        
        protected virtual void RefreshItemView()
        {
            scroll.Scroll.RefreshItemView();
        }
        
        protected void SetFilterExcludeIdSet(IEnumerable<long> idList)
        {
            scroll.SetFilterExcludeIdSet(idList);
        }
        
        protected virtual void OnSwipeDetailModal(SupportEquipmentScrollData scrollData) { }

        protected virtual void OnSellCompleted(){
            
        }
    }

}
