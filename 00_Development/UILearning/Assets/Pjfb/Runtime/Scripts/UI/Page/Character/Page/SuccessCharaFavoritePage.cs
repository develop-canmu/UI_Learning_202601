using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;


namespace Pjfb.Character
{
    public class SuccessCharaFavoritePage : CharacterVariableListBasePage
    {
        [SerializeField] private UIButton favoriteButton;

        private HashSet<long> editingFavoriteIdHashSet;
        private HashSet<long> originFavoriteIdHashSet;

        private bool isOpening;
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            originFavoriteIdHashSet = Enumerable.ToHashSet(UserDataManager.Instance.charaVariable.data.Values.Where(x => x.isLocked).Select(x => (long)x.id));
            editingFavoriteIdHashSet = new HashSet<long>(originFavoriteIdHashSet);
            SetFavoriteButtonInteractable();
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
        
        private bool IsSettingChanged()
        {
            var displayingCharacterIdList = GetItems().Select(x => (long)x.id);
            var hidingCharacterFavoriteIdList = originFavoriteIdHashSet.Where(x => !displayingCharacterIdList.Contains(x));
            var favoriteIdList = GetItems().Where(x => x.IsFavorite).Select(x => (long)x.id).Concat(hidingCharacterFavoriteIdList).ToArray();
            return  (favoriteIdList.Length != originFavoriteIdHashSet.Count ||
                     favoriteIdList.Any(x => !originFavoriteIdHashSet.Contains(x)));
        }
        
        #region EventListeners
        protected override void OnSelectCharacter(object value)
        {
            CharacterVariableScrollData data = value as CharacterVariableScrollData;
            data.IsFavorite = !data.IsFavorite;
            if (data.IsFavorite)
                editingFavoriteIdHashSet.Add(data.id);
            else
                editingFavoriteIdHashSet.Remove(data.id);
            SetFavoriteButtonInteractable();
            RefreshItemView();
        }
        public async void OnClickFavoriteButton()
        {
            var displayingCharacterIdList = GetItems().Select(x => (long)x.id);
            var hidingCharacterFavoriteIdList = originFavoriteIdHashSet.Where(x => !displayingCharacterIdList.Contains(x));
            var favoriteIdList = GetItems().Where(x => x.IsFavorite).Select(x => (long)x.id).Concat(hidingCharacterFavoriteIdList).ToArray();


            
            CharaVariableLockBulkAPIRequest request = new CharaVariableLockBulkAPIRequest();
            CharaVariableLockBulkAPIPost post = new CharaVariableLockBulkAPIPost
            {
                idList = favoriteIdList
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            originFavoriteIdHashSet = favoriteIdList.ToHashSet();
            RefreshItemView();
            
            
            AppManager.Instance.UIManager.PageManager.PrevPage();
        }

        protected override void OnReverseCharacterOrder()
        {
            foreach (var scrollData in GetItems().Where(scrollData => editingFavoriteIdHashSet.Contains(scrollData.id)))
            {
                scrollData.IsFavorite = true;
            }
            RefreshItemView();
        }

        protected override void OnSortFilter()
        {
            foreach (var scrollData in GetItems().Where(scrollData => editingFavoriteIdHashSet.Contains(scrollData.id)))
            {
                scrollData.IsFavorite = true;
            }
            RefreshItemView();
            SetFavoriteButtonInteractable();
        }
        #endregion

        private void SetFavoriteButtonInteractable()
        {
            favoriteButton.interactable = IsSettingChanged();
        }
    }
}
