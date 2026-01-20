using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public class SupportEquipmentSellPage : SupportEquipmentListBasePage
    { 
        [SerializeField] private UIButton sellButton;
        
        //private Dictionary<long, SupportEquipmentScrollData> selectingDictionary;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            //selectingDictionary = new Dictionary<long, SupportEquipmentScrollData>();
            
            // スクロールの除外リストの更新
            UpdateExcludeSelectionIds();
            
            await base.OnPreOpen(args, token);
        }
        
        /// <summary>スクロールの除外リストの更新</summary>
        private void UpdateExcludeSelectionIds()
        {
            var favoriteList = UserDataManager.Instance.supportEquipment.data.Values.Where(x => x.isLocked).Select(x => (long)x.id).ToList();
            var formattingList = DeckUtility.GetSupportEquipmentIds();
            scroll.SetExcludeSelectionIds(favoriteList.Concat(formattingList));
        }

        protected override void OnScrollInitialized()
        {
            SetSellButtonInteractable();
        }

        protected override void OnSelectSupportEquipment(SupportEquipmentScrollData data)
        {
            if(data is null) return;
            SetSellButtonInteractable();
        }
        
        protected override void OnDeselectSupportEquipment(SupportEquipmentScrollData data)
        {
            if(data is null) return;
            SetSellButtonInteractable();
        }

        protected override void OnSellCompleted(){
            SetSellButtonInteractable();
        }

        private void SetSellButtonInteractable()
        {
            sellButton.interactable = scroll.SelectedSupportEquipmentIds.Count > 0;
        }
        
        public void OnClickSellButton()
        {
            SupportEquipmentSellConfirmModalWindow.Open(new SupportEquipmentSellConfirmModalWindow.WindowParams()
            {
                idList = scroll.SelectedSupportEquipmentIds.Select(x => x.Id).ToArray(),
                onConfirm = AppManager.Instance.UIManager.PageManager.PrevPage
            });
        }
                
        public void OnClickFavoriteButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SupportEquipmentFavorite, true, null);
        }
        
        [CruEventTarget]
        public void OnCloseDetailModal(SupportEquipmentDetailModal.CloseUpdateType type)
        {
            // スクロールの除外リストの更新
            UpdateExcludeSelectionIds();
            // スクロールの選択状態を更新
            scroll.RefreshSelectionId();
            // 売却ボタンの有効化チェック
            SetSellButtonInteractable();
        }
    }

}
