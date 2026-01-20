using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Storage;
using Pjfb.Utility;
using TMPro;
using UnityEngine.UI;

namespace Pjfb
{
    public class MyBadgeChangeModal : ModalWindow
    {
        public class ModalData
        {
            /// <summary>バッチリスト</summary>
            public long[] emblemIds;
        }
        
        private const SortFilterUtility.SortFilterType SortFilterType = SortFilterUtility.SortFilterType.MyBadge;
        
        [SerializeField]
        private MyBadgeImage[] myBadgeImages;
        
        [SerializeField]
        private GameObject[] myBadgeEmpties;
        [SerializeField]
        private ScrollGrid scrollGrid;
        
        [SerializeField]
        private Image sortImage;
        [SerializeField]
        private TextMeshProUGUI priorityText;
        [SerializeField]
        private TextMeshProUGUI sortText;
        [SerializeField]
        private TextMeshProUGUI isFilterText;
        [SerializeField]
        private UIButton applyButton;
        
        private ModalData modalData;
        private long[] selectedEmblemIds;
        private List<MyBadgeScrollItem.ScrollData> scrollData = new List<MyBadgeScrollItem.ScrollData>();
        
        private long[] masterIds;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (ModalData) args;
            
            // バッチの初期化
            selectedEmblemIds = new long[modalData.emblemIds.Length];
            
            // 現在設定中のバッチを設定
            for (int i = 0; i < modalData.emblemIds.Length; i++)
            {
                // バッチの設定
                selectedEmblemIds[i] = modalData.emblemIds[i];
                
                if (selectedEmblemIds[i] != 0)
                {
                    myBadgeEmpties[i].SetActive(false);
                    myBadgeImages[i].gameObject.SetActive(true);
                    myBadgeImages[i].SetTextureAsync(selectedEmblemIds[i]);
                }
                else
                {
                    myBadgeEmpties[i].SetActive(true);
                    myBadgeImages[i].gameObject.SetActive(false);
                }
            }
            
            // データ設定
            SetScrollData(true);
            
            UpdateFilterText();
            UpdateSortText();
            
            applyButton.interactable = false;

            return base.OnPreOpen(args, token);
        }

        protected override UniTask OnOpen(CancellationToken token)
        {
            scrollGrid.OnItemEvent += OnSelect;
            return base.OnOpen(token);
        }

        protected override UniTask OnPreClose(CancellationToken token)
        {
            scrollGrid.OnItemEvent -= OnSelect;
            
            // 閲覧データ更新
            LocalSaveManager.saveData.viewedMyBadgeIds = masterIds;
            LocalSaveManager.Instance.SaveData();
            
            return base.OnPreClose(token);
        }
        
        /// <summary>フィルタのテキスト更新</summary>
        private void UpdateFilterText()
        {
            isFilterText.text = StringValueAssetLoader.Instance
            [
                SortFilterUtility.GetIsFilterKey(SortFilterUtility.IsFilter(SortFilterType))
            ];
        }
        
        /// <summary>ソートのテキスト更新</summary>
        private void UpdateSortText()
        {
            SortDataBase sortData = SortFilterUtility.GetSortDataByType(SortFilterType);
            sortText.text = StringValueAssetLoader.Instance[SortFilterUtility.GetSortOrderKey(sortData.orderType)];
            priorityText.text = StringValueAssetLoader.Instance[SortFilterUtility.GetSortPriorityKey(sortData.priorityType)];
            sortImage.transform.localScale = new Vector3(1, sortData.orderType == OrderType.Descending ? 1 : -1, 1);
        }

        /// <summary>データを設定</summary>
        private void SetScrollData(bool isInitialize = false)
        {
            if (isInitialize)
            { 
                // マスターデータの取得
                masterIds = MasterManager.Instance.emblemMaster.values
                    .Where(master => master.IsHave).Select(master => master.id).ToArray();
            }
            
            long[] sortFilterMyBadgeIds = SortFilterUtility.GetFilterMyBadge(masterIds, modalData.emblemIds);
            sortFilterMyBadgeIds = SortFilterUtility.GetSortMyBadge(sortFilterMyBadgeIds, modalData.emblemIds);
            
            // データの初期化
            scrollData.Clear();

            for (int index = 0; index < sortFilterMyBadgeIds.Length; index++)
            {
                long masterId = sortFilterMyBadgeIds[index];
                scrollData.Add(new MyBadgeScrollItem.ScrollData(masterId, index, 
                    !LocalSaveManager.saveData.viewedMyBadgeIds.Contains(masterId), GetIndex(masterId)));
            }
            
            scrollGrid.SetItems(scrollData);
        }
        
        /// <summary>選択済みのバッチのindexを取得</summary>
        private int GetIndex(long id)
        {
            for (int i = 0; i < selectedEmblemIds.Length; i++)
            {
                if (selectedEmblemIds[i] == id)
                {
                    return i;
                }
            }

            return -1;
        }
        
        /// <summary>設定可能なindexの取得</summary>
        private bool FindSelectableIndex(out int index)
        {
            index = -1;
            for (int i = 0; i < selectedEmblemIds.Length; i++)
            {
                // 選択されていないものがあればそのインデックスを返す
                if (selectedEmblemIds[i] == 0)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>更新したか</summary>
        private bool IsUpdateBadge()
        {
            for (int i = 0; i < modalData.emblemIds.Length; i++)
            {
                if (selectedEmblemIds[i] != modalData.emblemIds[i])
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>バッチの更新</summary>
        private async UniTask DisplayBadge(int index, long emblemId)
        {
            selectedEmblemIds[index] = emblemId;
            myBadgeImages[index].gameObject.SetActive(true);
            myBadgeEmpties[index].SetActive(false);
            await myBadgeImages[index].SetTextureAsync(emblemId);
        }
        
        /// <summary>バッチの更新</summary>
        private void RemoveBadge(int index)
        {
            selectedEmblemIds[index] = 0;
            myBadgeImages[index].gameObject.SetActive(false);
            myBadgeEmpties[index].SetActive(true);
        }
        
        /// <summary>バッチの設定</summary>
        private async UniTask OnClickApplyAsync()
        {
            // バッチリストの更新
            SetCloseParameter(await TrainerCardUtility.UpdateProfileEmblem(selectedEmblemIds));
            await CloseAsync();
        }
        
        /// <summary>フィルタ</summary>
        private async UniTask OnClickFilterAsync()
        {
            var data = new SortFilterBaseModal<MyBadgeSortData, MyBadgeFilterData>.Data
            (
                SortFilterSheetType.Filter,
                SortFilterType
            );
            
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.MyBadgeSortFilter, data, destroyCancellationToken);
            bool sortFilter = (bool)await modal.WaitCloseAsync(destroyCancellationToken);

            if (sortFilter)
            {
                SetScrollData();
                UpdateFilterText();
                UpdateSortText();
            }
        }

        /// <summary>コールバック</summary>
        private void OnSelect(ScrollGridItem item, object dataIndex)
        {
            int i = (int)dataIndex;
            if (scrollData[i].SelectedIndex > -1)
            {
                // バッチの削除
                RemoveBadge(scrollData[i].SelectedIndex);
                scrollData[i].SelectedIndex = -1;
            }
            else
            {
                // バッチの設定が可能か
                if (FindSelectableIndex(out int index))
                {
                    // バッチの更新
                    DisplayBadge(index, scrollData[i].Id).Forget();
                    scrollData[i].SelectedIndex = index;
                }
            }
            
            // ボタンの有効化
            applyButton.interactable = IsUpdateBadge();
        }
        
        /// <summary>UGUI</summary>
        public void OnClickReset()
        {
            // プレビューの削除
            for (int i = 0; i < selectedEmblemIds.Length; i++)
            {
                if (selectedEmblemIds[i] != 0)
                {
                    RemoveBadge(i);
                }
            }
            
            // 選択状態の解除
            foreach (MyBadgeScrollItem.ScrollData data in scrollData)
            {
                data.SelectedIndex = -1;
            }
            
            // ビューの更新
            scrollGrid.RefreshItemView();
            
            // ボタンの更新
            applyButton.interactable = IsUpdateBadge();
        }
        
        /// <summary>UGUI</summary>
        public void OnClickReversalAscendingDescending()
        {
            SortDataBase beforeData = SortFilterUtility.GetSortDataByType(SortFilterType);
            // 逆順にする
            MyBadgeSortData sortData = new MyBadgeSortData
            {
                orderType = SortFilterUtility.GetReversalOrderType(beforeData.orderType)
            };
            SortFilterUtility.SaveSortData(SortFilterType, sortData);
            // 並び替えする
            SetScrollData();
            // テキスト更新
            UpdateSortText();
        }
        
        /// <summary>UGUI</summary>
        public void OnClickFilter()
        {
            OnClickFilterAsync().Forget();
        }
        
        /// <summary>UGUI</summary>
        public void OnClickApply()
        {
            OnClickApplyAsync().Forget();
        }
    }
}