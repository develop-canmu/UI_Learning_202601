using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class UserTitleChangeModalWindow : ModalWindow
    {
        public class WindowParams
        {
            public long DefaultSelectId = 0;
        }
        
        private const SortFilterUtility.SortFilterType SortFilterType = SortFilterUtility.SortFilterType.UserTitle;
        
        [SerializeField] private UserTitleImage userTitleImagePreview;
        [SerializeField] private ScrollGrid scroller;
        [SerializeField] private UIButton applyButton;
        [SerializeField] private TextMeshProUGUI filterText;
        [SerializeField] private TextMeshProUGUI sortText;
        [SerializeField] private TextMeshProUGUI priorityText;
        [SerializeField] private Image sortIcon;
        
        private WindowParams _windowParams;
        private List<TitleScrollItem.Info> itemInfoList = new List<TitleScrollItem.Info>();
        private long currentSelectId;
        
        private List<long> idList = new List<long>();

        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.UserTitleChange, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            
            // 最初のId設定
            currentSelectId = _windowParams.DefaultSelectId;
            // プレビューの更新
            UpdatePreview(currentSelectId);
            
            // フィルタのテキスト更新
            UpdateFilterText();
            // ソートのテキスト更新
            UpdateSortText();
            
            // タイトルのリストをセット
            SetUserTitles(true);
            
            // 初期は設定しないのでfalse
            UpdateApplyButton();
            
            return base.OnPreOpen(args, token);
        }
        
        /// <summary>称号設定</summary>
        private void SetUserTitles(bool isInitialize = false)
        {
            if (isInitialize)
            {
                idList = UserDataManager.Instance.title.OrderBy(t=>t).ToList();
            }
            itemInfoList.Clear();
            
            List<long> userTitles = SortFilterUtility.GetFilterUserTitle(idList, _windowParams.DefaultSelectId);
            userTitles = SortFilterUtility.GetSortUserTitle(userTitles, _windowParams.DefaultSelectId);

            // 選択中のIDがリストにない場合はデフォルトIdに戻す
            if (!userTitles.Contains(currentSelectId))
            {
                currentSelectId = _windowParams.DefaultSelectId;
            }
            
            userTitles.ForEach(id =>
            {
                TitleScrollItem.Info itemInfo = new TitleScrollItem.Info
                {
                    Id = id,
                    Selected = currentSelectId == id,
                    Setted = _windowParams.DefaultSelectId == id,
                    OnSelect = OnSelect
                };
                itemInfoList.Add(itemInfo);
            });
            
            scroller.SetItems(itemInfoList);
        }
        
        /// <summary>選択中のIdを更新</summary>
        private void OnSelect(long selectedId)
        {
            currentSelectId = selectedId;
            UpdateApplyButton();
            
            foreach (TitleScrollItem.Info info in itemInfoList)
            {
                info.Selected = selectedId == info.Id;
            }
            UpdatePreview(currentSelectId);
            scroller.RefreshItemView();
        }
        
        /// <summary>API</summary>
        private async UniTask UserUpdateTitleAPI(long mTitleId)
        {
            UserUpdateTitleAPIRequest request = new UserUpdateTitleAPIRequest();
            UserUpdateTitleAPIPost post = new UserUpdateTitleAPIPost{mTitleId = mTitleId};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
        }
        
        /// <summary>ボタンの状態更新</summary>
        private void UpdateApplyButton()
        {
            applyButton.interactable = currentSelectId != _windowParams.DefaultSelectId;
        }
        
        /// <summary>フィルタしているかのテキスト更新</summary>
        private void UpdateFilterText()
        {
            filterText.text = StringValueAssetLoader.Instance
            [
                SortFilterUtility.GetIsFilterKey(SortFilterUtility.IsFilter(SortFilterType))
            ];
        }
        
        /// <summary>ソート順のテキスト更新</summary>
        private void UpdateSortText()
        {
            SortDataBase sortData = SortFilterUtility.GetSortDataByType(SortFilterType);
            sortText.text = StringValueAssetLoader.Instance[SortFilterUtility.GetSortOrderKey(sortData.orderType)];
            priorityText.text = StringValueAssetLoader.Instance[SortFilterUtility.GetSortPriorityKey(sortData.priorityType)];
            sortIcon.transform.localScale = new Vector3(1, sortData.orderType == OrderType.Descending ? 1 : -1, 1);
        }

        /// <summary>プレビューの更新</summary>
        private void UpdatePreview(long id)
        {
            userTitleImagePreview.SetTexture(id);
        }

        /// <summary>UGUI</summary>
        public void OnClickFilter()
        {
            OnClickFilterAsync().Forget();
        }
        
        private async UniTask OnClickFilterAsync()
        {
            SortFilterBaseModal<UserTitleSortData, UserTitleFilterData>.Data data = new SortFilterBaseModal<UserTitleSortData, UserTitleFilterData>.Data
            (
                SortFilterSheetType.Filter,
                SortFilterType
            );
            
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.UserTitleSortFilter, data, destroyCancellationToken);
            bool sortFilter = (bool)await modal.WaitCloseAsync(destroyCancellationToken);

            if (sortFilter)
            {
                // フィルタする
                SetUserTitles();
                // テキスト更新
                UpdateFilterText();
                // テキスト更新
                UpdateSortText();
                // ボタンの状態更新
                UpdateApplyButton();
            }
        }
        
        /// <summary>UGUI</summary>
        public void OnClickReversalAscendingDescending()
        {
            SortDataBase beforeData = SortFilterUtility.GetSortDataByType(SortFilterType);
            // 逆順にする 
            UserTitleSortData sortData = new UserTitleSortData
            {
                orderType = SortFilterUtility.GetReversalOrderType(beforeData.orderType)
            };
            SortFilterUtility.SaveSortData(SortFilterType, sortData);
            // 並び替えする
            SetUserTitles();
            // テキスト更新
            UpdateSortText();
        }
        
        /// <summary>UGUI</summary>
        public void OnClickApply()
        {
            OnClickApplyAsync().Forget();
        }
        
        private async UniTask OnClickApplyAsync()
        {
            await UserUpdateTitleAPI(currentSelectId);
            OnClickClose();
        }
        
        /// <summary>UGUI</summary>
        public void OnClickClose()
        {
            LocalSaveManager.saveData.viewedTitles = new List<long>(UserDataManager.Instance.title);
            LocalSaveManager.Instance.SaveData();
            AppManager.Instance.UIManager.Header.UpdateMenuBadge();
            Close();
        }
    }
}
