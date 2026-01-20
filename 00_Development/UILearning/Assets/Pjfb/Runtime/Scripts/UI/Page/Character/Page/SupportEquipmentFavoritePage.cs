using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public class SupportEquipmentFavoritePage : SupportEquipmentListBasePage
    {
        [SerializeField] private UIButton favoriteButton;
     
        private HashSet<long> editingFavoriteIdHashSet;
        private HashSet<long> originFavoriteIdHashSet;
        private bool isOpening;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            originFavoriteIdHashSet = UserDataManager.Instance.supportEquipment.data.Values.Where(x => x.isLocked).Select(x => (long)x.id).ToHashSet();
            editingFavoriteIdHashSet = new HashSet<long>(originFavoriteIdHashSet);
            selectingHashSet = originFavoriteIdHashSet;
            favoriteButton.interactable = false;
            isOpening = true;
            return base.OnPreOpen(args, token);
        }

        protected override async UniTask<bool> OnPreLeave(CancellationToken token)
        {
            if (!isOpening) return await base.OnPreLeave(token);
            bool result = true;
            if (IsSettingChanged())
            {
                result = await OnLeaveFavoritePage(this.GetCancellationTokenOnDestroy());

                isOpening = !result;
            }
           
            return result;
        }


        private async UniTask<bool> OnLeaveFavoritePage(CancellationToken cancellationToken)
        {
            var cancelButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], (window =>
            {
                window.SetCloseParameter(false);
                window.Close();
            }));
            
            var okButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (window) =>
            {
                window.SetCloseParameter(true);
                window.Close();
            });
            
            // タッチガードより手前に表示する
            var confirmWindow = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                StringValueAssetLoader.Instance["character.leave_favorite_page_content"], "", 
                okButtonParams,cancelButtonParams), cancellationToken);
            
            return (bool)await confirmWindow.WaitCloseAsync();
        }

        protected override void OnScrollInitialized()
        {
            SetFavoriteButtonInteractable();
        }

        protected override void OnSelectSupportEquipment(SupportEquipmentScrollData data)
        {
            if(data is null) return;
            data.IsFavorite = true;

            editingFavoriteIdHashSet.Add(data.Id);
                
            SetFavoriteButtonInteractable();
        }
        
        protected override void OnDeselectSupportEquipment(SupportEquipmentScrollData data)
        {
            if(data is null) return;
            data.IsFavorite = false;

            editingFavoriteIdHashSet.Remove(data.Id);
                
            SetFavoriteButtonInteractable();
        }

        public void OnClickFavoriteButton()
        {
            SetSupportEquipmentFavorite().Forget();
        }

        private async UniTask SetSupportEquipmentFavorite()
        {
            var displayingSupportEquipmentIdList = GetItems().Select(x => (long)x.Id);
            var hidingSupportEquipmentFavoriteIdList = originFavoriteIdHashSet.Where(x => !displayingSupportEquipmentIdList.Contains(x));
            var favoriteIdList = GetItems().Where(x => x.IsFavorite).Select(x => (long)x.Id).Concat(hidingSupportEquipmentFavoriteIdList).ToArray();

            
            CharaVariableTrainerLockBulkAPIRequest request = new CharaVariableTrainerLockBulkAPIRequest();
            CharaVariableTrainerLockBulkAPIPost post = new CharaVariableTrainerLockBulkAPIPost()
            {
                idList = favoriteIdList
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            originFavoriteIdHashSet = favoriteIdList.ToHashSet();
            RefreshItemView();
            
            
            AppManager.Instance.UIManager.PageManager.PrevPage();
        }
        
        
        
        private void SetFavoriteButtonInteractable()
        {
            favoriteButton.interactable = IsSettingChanged();
        }
        
        private bool IsSettingChanged()
        {
            var displayingSupportEquipmentIdList = GetItems().Select(x => (long)x.Id);
            var hidingSupportEquipmentFavoriteIdList = originFavoriteIdHashSet.Where(x => !displayingSupportEquipmentIdList.Contains(x));
            var favoriteIdList = GetItems().Where(x => x.IsFavorite).Select(x => (long)x.Id).Concat(hidingSupportEquipmentFavoriteIdList).ToArray();
            return  (favoriteIdList.Length != originFavoriteIdHashSet.Count ||
                     favoriteIdList.Any(x => !originFavoriteIdHashSet.Contains(x)));
        }
    }
}
