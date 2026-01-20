using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using Pjfb.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class UserIconChangeModalWindow : ModalWindow
    {
        #region Params
        
        public class WindowParams
        {
            public long DefaultSelectId = 0;
        }
        
        /// <summary>ソートフィルタのタイプ</summary>
        private const SortFilterUtility.SortFilterType SortFilterType = SortFilterUtility.SortFilterType.UserIcon;
        
        [SerializeField] private ScrollGrid scroller;
        [SerializeField] private UIButton applyButton;
        [SerializeField] private TextMeshProUGUI filterText;
        [SerializeField] private TextMeshProUGUI sortText;
        [SerializeField] private TextMeshProUGUI priorityText;
        [SerializeField] private Image sortIcon;
        
        private WindowParams _windowParams;
        private List<IconScrollItem.Info> itemInfoList = new List<IconScrollItem.Info>();
        private long currentSelectId;

        private List<long> userIconIdList = null;

        #endregion

        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.UserIconChange, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            
            // 最初のId設定
            currentSelectId = _windowParams.DefaultSelectId;
            
            // ユーザーアイコンのリストをセット
            SetUserIcons(true);
            
            // フィルタのテキスト更新
            UpdateFilterText();
            // ソートのテキスト更新
            UpdateSortText();
            
            // 初期は設定しないのでfalse
            UpdateApplyButton();
            
            return base.OnPreOpen(args, token);
        }
        
        /// <summary>アイコンの設定(開いたときだけデータ取得してくる)</summary>
        private void SetUserIcons(bool isInitialize = false)
        {
            itemInfoList.Clear();
            if (isInitialize)
            {
                // ユーザーアイコンのリストを取得
                userIconIdList = UserDataManager.Instance.GetUserIcons();
            }
            
            // フィルタ
            List<long> sortFilterUserIconIdList = SortFilterUtility.GetFilterUserIcon(userIconIdList, _windowParams.DefaultSelectId);
            // ソート
            sortFilterUserIconIdList = SortFilterUtility.GetSortUserIcon(sortFilterUserIconIdList, _windowParams.DefaultSelectId);

            // 選択中のIDがリストにない場合はデフォルトIdに戻す
            if (!sortFilterUserIconIdList.Contains(currentSelectId))
            {
                currentSelectId = _windowParams.DefaultSelectId;
            }
            
            foreach (long iconId in sortFilterUserIconIdList)
            {
                IconScrollItem.Info itemInfo = new IconScrollItem.Info
                {
                    Id = iconId,
                    Selected = currentSelectId == iconId,
                    Setted = _windowParams.DefaultSelectId == iconId,
                    OnSelect = OnSelect
                };
                itemInfoList.Add(itemInfo);
            }
            
            scroller.SetItems(itemInfoList);
        }
        
        /// <summary>フィルタしてるかのテキスト更新</summary>
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
        
        /// <summary>ボタンの状態更新</summary>
        private void UpdateApplyButton()
        {
            applyButton.interactable = currentSelectId != _windowParams.DefaultSelectId;
        }

        /// <summary>選択中のID更新</summary>
        private void OnSelect(long id)
        {
            currentSelectId = id;
            UpdateApplyButton();
            
            foreach (IconScrollItem.Info info in itemInfoList)
            {
                info.Selected = currentSelectId == info.Id;
            }

            scroller.RefreshItemView();
        }
        
        /// <summary>UGUI</summary>
        public void OnClickFilter()
        {
            OnClickFilterAsync().Forget();
        }

        private async UniTask OnClickFilterAsync() 
        {
            SortFilterBaseModal<UserIconSortData, UserIconFilterData>.Data data = new SortFilterBaseModal<UserIconSortData, UserIconFilterData>.Data
            (
                SortFilterSheetType.Filter,
                SortFilterType
            );
            
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.UserIconSortFilter, data, destroyCancellationToken);
            bool sortFilter = (bool)await modal.WaitCloseAsync();

            if (sortFilter)
            {
                // フィルタする
                SetUserIcons();
                // フィルタのテキスト更新
                UpdateFilterText();
                // ソートのテキスト更新
                UpdateSortText();
                // ボタンの有効無効
                UpdateApplyButton();
            }
        }
        
        /// <summary>UGUI</summary>
        public void OnClickReversalAscendingDescending()
        {
            SortDataBase beforeData = SortFilterUtility.GetSortDataByType(SortFilterType);
            // 逆順にする 
            UserIconSortData sortData = new UserIconSortData
            {
                orderType = SortFilterUtility.GetReversalOrderType(beforeData.orderType)
            };
            SortFilterUtility.SaveSortData(SortFilterType, sortData);
            // 並び替えする
            SetUserIcons();
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
            // Iconを更新する
            SetCloseParameter(await TrainerCardUtility.UpdateTrainerCardIcon(currentSelectId));
            UserDataManager.Instance.user.UpdateUserIconId(currentSelectId);
            await CloseAsync();
        }
    }
}