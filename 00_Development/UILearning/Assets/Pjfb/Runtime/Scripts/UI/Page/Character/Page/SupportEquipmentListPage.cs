using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public class SupportEquipmentListPage : SupportEquipmentListBasePage
    {
        [SerializeField] private UIButton sellButton;
        [SerializeField] private UIButton allSellButton;
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            SellButtonInteractive();
            AllSellButtonInteractive();
            await base.OnPreOpen(args, token);
        }
        
        protected override async UniTask OnSelectSupportEquipmentAsync(SupportEquipmentScrollData data)
        {
            if (data is null) return;

            var modalParam = new SupportEquipmentDetailModalParams(data.SwipeableParams, onUpdateBadge: () =>
            {
                scroll.RefreshItemView();
                AppManager.Instance.UIManager.Footer.CharacterButton
                    .SetNotificationBadge(BadgeUtility.IsCharacterBadge);
            },
            onChangeScrollFavorite: (index, isFavorite) =>
            {
                var scrollData = scroll.SupportEquipmentScrollDataList[index];
                scrollData.IsFavorite = isFavorite;
            });
            
            var modal = await SupportEquipmentDetailModal.OpenModalAsync(ModalType.SupportEquipmentDetail, modalParam);
            
            SupportEquipmentDetailModal.CloseUpdateType closeUpdateType = (SupportEquipmentDetailModal.CloseUpdateType)await modal.WaitCloseAsync();
            switch (closeUpdateType)
            {
                case SupportEquipmentDetailModal.CloseUpdateType.Sell:
                    scroll.OnSellSupportEquipment();
                    break;
                case SupportEquipmentDetailModal.CloseUpdateType.Redrawing:
                    // 再抽選後の能力反映のためにスクロールデータを更新する
                    scroll.RedrawSupportEquipment(selectingHashSet);
                    break;
            }

        }
        public void OnClickSellButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SupportEquipmentSell, true, null);
        }
        public async void OnClickAllSellButton()
        {
            var args = new SupportEquipmentAllSellFilterModal.SellData(SortFilterSheetType.Filter, SortFilterUtility.SortFilterType.AllSellSupportEquipment,ScrollerUpdate);
            CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.SupportEquipmentSellFilter, args, this.GetCancellationTokenOnDestroy());

        }

        public void OnClickFavoriteButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SupportEquipmentFavorite, true, null);
        }

        private void ScrollerUpdate()
        {
            scroll.InitializeScroll(selectingHashSet);
        }
        
        protected void SellButtonInteractive()
        {
            sellButton.interactable = UserDataManager.Instance.supportEquipment.data.Count > 0;
        }
        
        protected void AllSellButtonInteractive()
        {
            allSellButton.interactable = UserDataManager.Instance.supportEquipment.data.Count > 0;
        }
    }

}
