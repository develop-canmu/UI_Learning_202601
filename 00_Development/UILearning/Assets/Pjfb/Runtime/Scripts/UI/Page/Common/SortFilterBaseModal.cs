using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    
    public enum SortFilterModalOption
    {
        None = 0,
        HideExtraFilter = 1 << 0
    }
    
    public abstract class SortFilterBaseModal<TSort, TFilter> : ModalWindow where TSort : SortDataBase, new() where TFilter : FilterDataBase, new()
    {
        [Serializable]
        public class Data
        {
            public SortFilterSheetType sheetType;
            public SortFilterUtility.SortFilterType sortFilterType;
            public SortFilterModalOption options = SortFilterModalOption.None;

            public Data(SortFilterSheetType _sheetType, SortFilterUtility.SortFilterType _sortFilterType, SortFilterModalOption _options = SortFilterModalOption.None)
            {
                sheetType = _sheetType;
                sortFilterType = _sortFilterType;
                options = _options;
            }
        }
        
        [Serializable]
        public class OrderToggleInfo
        {
            public OrderType Type;
            public Toggle ToggleObject;
        }
        
        [Serializable]
        public class PriorityToggleInfo
        {
            public PriorityType Type;
            public Toggle ToggleObject;
        }
        
        
        [Header("並び替え")]
        [SerializeField] private List<OrderToggleInfo> orderToggleList;
        [SerializeField] private List<PriorityToggleInfo> priorityToggleList;
        [SerializeField] private SortFilterTabSheetManager sheetManager = null;
        
        /// <summary>並び替えの順序トグルリスト</summary>
        protected List<OrderToggleInfo> OrderToggleList => orderToggleList;
        
        /// <summary>並び替えの優先順位トグルリスト</summary>
        protected List<PriorityToggleInfo> PriorityToggleList => priorityToggleList;
        
        [Header("適用ボタンのグレーアウト機能")]
        [SerializeField] private bool enableApplyButtonGrayOut = false;
        [SerializeField] private Button applyButton;

        private Data modalData;
        protected Data ModalData
        {
            get { return modalData; }
        }
        
        /// <summary>適用ボタンのグレーアウト機能が有効かどうか</summary>
        protected bool EnableApplyButtonGrayOut => enableApplyButtonGrayOut;
        
        /// <summary>適用ボタン</summary>
        protected Button ApplyButton => applyButton;
        
        // 初期状態を保存
        protected TSort initialSortData = null;
        protected TFilter initialFilterData = null;

        /// <summary>マスタ参照してトグルを生成する</summary>
        protected virtual void CreateToggleListFromMasterData(){}
        
        /// <summary>
        /// UIの設定状況から絞り込みデータの作成
        /// </summary>
        protected abstract TFilter CreateFilterData();


        /// <summary>
        /// データから絞り込みのToggleの状態を設定する
        /// </summary>
        /// <param name="filterData"></param>
        protected abstract void SetFilterToggleFromData(TFilter filterData);
        
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data)args;
            
            // 初期値として、キャンセル扱い（false）を設定
            SetCloseParameter(false);
            
            //データからソートのUiを設定する
            SetSortToggleFromData(SortFilterUtility.GetSortDataByType(modalData.sortFilterType));
            
            //マスタを参照してトグルを生成
            CreateToggleListFromMasterData();
            
            //データからフィルターのUiを設定する
            SetFilterToggleFromData(SortFilterUtility.GetFilterDataByType(modalData.sortFilterType) as TFilter);
            sheetManager?.OpenSheet(modalData.sheetType, true);
            
            // グレーアウト機能が有効な場合のみ初期化
            if (enableApplyButtonGrayOut)
            {
                SaveInitialState();
                RegisterToggleChangeEvents();
                applyButton.interactable = IsChangedFromInitialState();
            }
            
            return base.OnPreOpen(args, token);
        }

        /// <summary>
        /// データから並び替えのToggleの状態を設定する
        /// </summary>
        /// <param name="sortData"></param>
        protected virtual void SetSortToggleFromData(SortDataBase sortData)
        {
            if (sortData == null) return;
            
            foreach (var orderToggleInfo in orderToggleList)
            {
                orderToggleInfo.ToggleObject.SetIsOnWithoutNotify(orderToggleInfo.Type == sortData.orderType);
            }
            
            foreach (var priorityToggleInfo in priorityToggleList)
            {
                priorityToggleInfo.ToggleObject.SetIsOnWithoutNotify(priorityToggleInfo.Type == sortData.priorityType);
            }
        }
        
     
        
        public void OnClickApplyButton()
        {
            var sortData = CreateSortData();
            var filterData = CreateFilterData();
            SortFilterUtility.SaveSortData(modalData.sortFilterType, sortData);
            SortFilterUtility.SaveFilterData(modalData.sortFilterType, filterData);
            LocalSaveManager.Instance.SaveData();
            SetCloseParameter(true);
            Close();
        }

        public void OnClickCancel()
        {
            SetCloseParameter(false);
            Close();
        }

        public void OnClickSortResetButton()
        {
            SetSortToggleFromData(new TSort());
            if (EnableApplyButtonGrayOut)
            {
                ApplyButton.interactable = IsSortChanged();
            }
        }
        
        public void OnClickFilterResetButton()
        {
            SetFilterToggleFromData(new TFilter());
            if (EnableApplyButtonGrayOut)
            {
                ApplyButton.interactable = IsFilterChanged();
            }
        }

        /// <summary>
        /// UIの設定状況から並び替えデータの作成
        /// </summary>
        /// <returns></returns>
        protected virtual TSort CreateSortData()
        {
            TSort sortData = new TSort();

            sortData.orderType = orderToggleList.FirstOrDefault(toggle => toggle.ToggleObject.isOn)?.Type ?? sortData.orderType;
            sortData.priorityType = priorityToggleList.FirstOrDefault(toggle => toggle.ToggleObject.isOn)?.Type ?? sortData.priorityType;
            
            return sortData;
        }
        
        # region 適用ボタンのグレーアウト制御
        
        /// <summary>
        /// 初期状態を保存する
        /// </summary>
        private void SaveInitialState()
        {
            initialSortData = CreateSortData();
            initialFilterData = CreateFilterData();
        }
        
        /// <summary>
        /// トグル変更イベントを登録する
        /// </summary>
        protected virtual void RegisterToggleChangeEvents()
        {
            if (orderToggleList != null)
            {
                foreach (OrderToggleInfo orderToggleInfo in orderToggleList)
                {
                    orderToggleInfo.ToggleObject.onValueChanged.RemoveListener(OnToggleValueChanged);
                    orderToggleInfo.ToggleObject.onValueChanged.AddListener(OnToggleValueChanged);
                }
            }
            
            if (priorityToggleList != null)
            {
                foreach (PriorityToggleInfo priorityToggleInfo in priorityToggleList)
                {
                    priorityToggleInfo.ToggleObject.onValueChanged.RemoveListener(OnToggleValueChanged);
                    priorityToggleInfo.ToggleObject.onValueChanged.AddListener(OnToggleValueChanged);
                }
            }
        }
        
        /// <summary>
        /// トグル値変更時の処理
        /// </summary>
        protected void OnToggleValueChanged(bool value)
        {
            applyButton.interactable = IsChangedFromInitialState();
        }
        
        /// <summary>
        /// モーダルを開いた時点の初期状態から変更があるかチェックする
        /// </summary>
        protected bool IsChangedFromInitialState()
        {
            if (initialSortData == null || initialFilterData == null) return false;
            
            bool sortChanged = IsSortChanged();
            bool filterChanged = IsFilterChanged();
            bool additionalChanged = HasAdditionalChanges();
            
            return sortChanged || filterChanged || additionalChanged;
        }
        
        /// <summary>
        /// ソート設定が変更されているかチェックする
        /// </summary>
        private bool IsSortChanged()
        {
            // 現在選択されているOrderTypeを取得
            OrderType? currentOrderType = orderToggleList.FirstOrDefault(toggle => toggle.ToggleObject.isOn)?.Type;
            
            // 現在選択されているPriorityTypeを取得
            PriorityType? currentPriorityType = priorityToggleList.FirstOrDefault(toggle => toggle.ToggleObject.isOn)?.Type;
            if (currentOrderType == null || currentPriorityType == null)
            {
                CruFramework.Logger.LogError("currentOrderType or currentPriorityType is null");
                return false;
            }
            
            return initialSortData.orderType != currentOrderType || initialSortData.priorityType != currentPriorityType;
        }
        
        /// <summary>
        /// フィルター設定が変更されているかチェックする
        /// デフォルトではfalseを返す（フィルター機能がない場合）
        /// </summary>
        protected virtual bool IsFilterChanged()
        {
            return false;
        }
        
        /// <summary>
        /// 追加の変更チェック
        /// デフォルトではfalseを返す（追加のチェックが必要ない場合）
        /// </summary>
        protected virtual bool HasAdditionalChanges()
        {
            return false;
        }
        
        # endregion

    }
}


