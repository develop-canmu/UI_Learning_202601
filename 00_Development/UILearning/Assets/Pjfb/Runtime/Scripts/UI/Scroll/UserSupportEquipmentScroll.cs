using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{

    public class UserSupportEquipmentScroll : ItemIconScroll<SupportEquipmentScrollData>
    {
        private enum SelectionMode
        {
            None,
            Single,
            Multiple,
        }
        
        private const string PossessionCountStringValueKey = "character.list.possession_count";

        [SerializeField] private SelectionMode selectionMode;
        [SerializeField] private bool showSelectionEffect;
        [SerializeField] private bool showFavoriteBlinkEffect;
        [SerializeField] private AnimationClip selectAnimationClip;
        [SerializeField] private AnimationClip favoriteAnimationClip;
        [SerializeField] private TextMeshProUGUI sortPriorityText;
        [SerializeField] private TextMeshProUGUI sortOrderText;
        [SerializeField] private TextMeshProUGUI isFilterText;
        [SerializeField] private TextMeshProUGUI noSupportEquipmentText;
        [SerializeField] private TextMeshProUGUI possessionCountText;
        [SerializeField] private SortFilterUtility.SortFilterType sortFilterType = SortFilterUtility.SortFilterType.ListSupportEquipment;
        [SerializeField] private Image sortOrderImage;
        public Action<SupportEquipmentScrollData> OnSelectSupportEquipment;
        public Action<SupportEquipmentScrollData> OnDeselectSupportEquipment;
        

        public HashSet<SupportEquipmentScrollData> SelectedSupportEquipmentIds => selectedSupportEquipmentScrollDataHashSet;
        private HashSet<SupportEquipmentScrollData> selectedSupportEquipmentScrollDataHashSet = new HashSet<SupportEquipmentScrollData>();
        private HashSet<long> excludeSelectionIdSet = new ();
        private HashSet<long> filterExcludeIdSet;
        //最初のお気に入りに設定しているIDのリスト
        public HashSet<long> originFavoriteHashSet = null;
        
        public Action OnSortFilter;
        public Action OnReverseSupportEquipmentOrder;
        public Action OnScrollInitialized;
        public Action<SupportEquipmentScrollData> OnSwipeDetailModal;
        public Action OnSellCompleted = null;
        
        
        private float selectAnimationTime;
        private float favoriteAnimationTime;
        private float SelectEffectAnimationNormalizedTime => selectAnimationTime / selectAnimationClip.length;
        private float FavoriteEffectAnimationNormalizedTime => favoriteAnimationTime / favoriteAnimationClip.length;
        
        private List<SupportEquipmentScrollData> supportEquipmentScrollDataList = new List<SupportEquipmentScrollData>();
        public List<SupportEquipmentScrollData> SupportEquipmentScrollDataList
        {
            get{return  supportEquipmentScrollDataList;}
        }
        
        private List<UserDataSupportEquipment> userSupportEquipmentList;
        private bool isInitialized;
        
        private HashSet<long> formattingIdHashSet = new HashSet<long>();
       
        public List<SupportEquipmentDetailData> DetailOrderList => detailOrderList.ToList();
        private List<SupportEquipmentDetailData> detailOrderList = new();
        
        /// <summary>デッキ制限比較用</summary>
        private DeckFormatConditionMasterObject supportEquipmentFormatCondition = null;

        public void SetSupportEquipmentFormatCondition(DeckFormatConditionMasterObject supportEquipmentFormatCondition)
        {
            this.supportEquipmentFormatCondition = supportEquipmentFormatCondition;
        }
        
        public void SetFormattingIds(long[] ids)
        {
            formattingIdHashSet = ids.ToHashSet();
        }
        
        public void SetFormattingIds(HashSet<long> ids)
        {
            formattingIdHashSet = ids;
        }
        
        private void Update()
        {
            if (showSelectionEffect)
            {
                selectAnimationTime += Time.deltaTime;
                if (selectAnimationTime >= selectAnimationClip.length) selectAnimationTime -= selectAnimationClip.length;
            }
            
            if (showFavoriteBlinkEffect)
            {
                favoriteAnimationTime += Time.deltaTime;
                if (favoriteAnimationTime >= favoriteAnimationClip.length) favoriteAnimationTime -= favoriteAnimationClip.length;
            }
        }

        public void InitializeScroll(HashSet<long> selectingIds = null)
        {
            if (selectionMode != SelectionMode.None)
            {
                OnSelectedItem -= SelectSupportEquipment;
                OnSelectedItem += SelectSupportEquipment;
            }
            
            selectedSupportEquipmentScrollDataHashSet.Clear();
            userSupportEquipmentList = UserDataManager.Instance.supportEquipment.data.Values.ToList();
            supportEquipmentScrollDataList.Clear();
            detailOrderList.Clear();
            int scrollIndex = 0;
            
            Func<float> getSelectEffectNormalizedTime = (showSelectionEffect) ? () => SelectEffectAnimationNormalizedTime : null;
            Func<float> getFavoriteEffectNormalizedTime = (showFavoriteBlinkEffect) ? () => FavoriteEffectAnimationNormalizedTime : null;
            
            // ユーザーが所持しているリスト
            List<UserDataSupportEquipment> equipmentList = new List<UserDataSupportEquipment>(UserDataManager.Instance.supportEquipment.data.Values);
            
            // 編成中のリスト
            List<UserDataSupportEquipment> formattingEquipmentList = new List<UserDataSupportEquipment>();
            foreach(long id in formattingIdHashSet)
            {
                UserDataSupportEquipment uEquipment = UserDataManager.Instance.supportEquipment.Find(id);
                formattingEquipmentList.Add(uEquipment);
                equipmentList.Remove(uEquipment);
            }
            
            // ソート
            equipmentList = ApplySortFilter(equipmentList);
            // 編成中のリストを頭に追加
            equipmentList.InsertRange(0, formattingEquipmentList);
            
            foreach (UserDataSupportEquipment supportEquipment in equipmentList)
            {
                SupportEquipmentScrollDataOptions options = SupportEquipmentScrollDataOptions.None;
                // 編成中
                if(formattingIdHashSet.Contains(supportEquipment.id))options |= SupportEquipmentScrollDataOptions.Formatting;
                
                // デッキ制限
                bool isLimit = false;
                // nullでないかつ1個以上条件がある場合はデッキ制限を表示する
                if (supportEquipmentFormatCondition != null)
                {
                    // デッキ制限表示
                    isLimit = !DeckUtility.IsInSupportEquipmentSlotLimit(supportEquipmentFormatCondition, supportEquipment.MChara.deckConditionGroup);
                }

                detailOrderList.Add(new SupportEquipmentDetailData(supportEquipment));
                var scrollData = new SupportEquipmentScrollData(supportEquipment, new SwipeableParams<SupportEquipmentDetailData>(detailOrderList, scrollIndex++, OnDetailModalIndexChange), getSelectEffectNormalizedTime, getFavoriteEffectNormalizedTime, options, OnUpdateBadge,OnChangeFavorite,OnSellSupportEquipment, OnRedrawSupportEquipment, isLimit);

                if (selectingIds != null && selectingIds.Contains(supportEquipment.id))
                {
                    selectedSupportEquipmentScrollDataHashSet.Add(scrollData);
                }
                supportEquipmentScrollDataList.Add(scrollData);    
            }
            
            possessionCountText.text = StringValueAssetLoader.Instance[PossessionCountStringValueKey].Format(UserDataManager.Instance.supportEquipment.data.Values.Count, ConfigManager.Instance.uCharaVariableTrainerCountMax);
            isInitialized = true;
            Refresh();
            OnScrollInitialized?.Invoke();
        }
        
        public new void Refresh()
        {
            if (!isInitialized)
            {
                InitializeScroll();
                return;
               
            }
        
            base.Refresh();
            SetPossessionText();
        }
        
        // 初期化時以外の更新
        private void UpdateRedraw(bool isResetScrollPosition = true)
        {
            scrollGrid.Refresh(isResetScrollPosition);
            SetPossessionText();
        }

        public void OnRedrawSupportEquipment()
        {
            RedrawSupportEquipment(null);
        }
        
        public void RedrawSupportEquipment(HashSet<long> selectingIds = null)
        {
            Vector2 normalizedPosition = scrollGrid.normalizedPosition;
            InitializeScroll(selectingIds);
            scrollGrid.normalizedPosition = normalizedPosition;
            UpdateRedraw(false);
        }
        
        public void OnSellSupportEquipment()
        {
            var currentIdList = UserDataManager.Instance.supportEquipment.data.Values.Select(x => x.id).ToHashSet();
            supportEquipmentScrollDataList.RemoveAll(x => !currentIdList.Contains((int)x.Id));
            detailOrderList.RemoveAll(x => !currentIdList.Contains(x.USupportEquipmentId));
            selectedSupportEquipmentScrollDataHashSet.RemoveWhere( x => !currentIdList.Contains((int)x.Id) );
            for (int i = 0; i < supportEquipmentScrollDataList.Count; i++)
            {
                supportEquipmentScrollDataList[i].SwipeableParams.StartIndex = i;
            }
            UpdateRedraw(false);
            
            possessionCountText.text = StringValueAssetLoader.Instance[PossessionCountStringValueKey].Format(UserDataManager.Instance.supportEquipment.data.Values.Count, ConfigManager.Instance.uCharaVariableTrainerCountMax);
            OnSellCompleted?.Invoke();
        }

        private void OnUpdateBadge()
        {
            RefreshItemView();
            AppManager.Instance.UIManager.Footer.CharacterButton.SetNotificationBadge(BadgeUtility.IsCharacterBadge);
        }

        public void UpdateScrollFavorite()
        {
            foreach (var data in supportEquipmentScrollDataList)
            {
                var scrollData = UserDataManager.Instance.supportEquipment.Find(data.Id);
                if (scrollData.isLocked != data.IsFavorite)
                {
                    data.IsFavorite = scrollData.isLocked;
                }
            }
            RefreshItemView();

        }

        private void OnChangeFavorite(int index,bool isFavorite)
        {
            var scrollData = ItemListSrc[index];
            scrollData.IsFavorite = isFavorite;
        }
        
        private void SetPossessionText()
        {
            if (GetItemList().Count == 0)
            {
                noSupportEquipmentText.gameObject.SetActive(true);
                noSupportEquipmentText.text = StringValueAssetLoader.Instance[(userSupportEquipmentList.Count == 0) ? "character.no_support_equipment.notification_01" : "character.no_support_equipment.notification_02"];    
            }
            else
            {
                noSupportEquipmentText.gameObject.SetActive(false);
            }
        }

        public void RefreshItemView()
        {
            scrollGrid.RefreshItemView();
        }

        public void SetExcludeSelectionIds(HashSet<long> hashSet)
        {
            excludeSelectionIdSet = hashSet;
        }

        public void SetExcludeSelectionIds(IEnumerable<long> idList)
        {
            SetExcludeSelectionIds(idList.ToHashSet());
        }
        
        public void SetExcludeSelectionIds(List<long> idList)
        {
            SetExcludeSelectionIds(idList.ToHashSet());
        }
        
        protected override List<SupportEquipmentScrollData> GetItemList()
        {
            return supportEquipmentScrollDataList;
        }

        private List<UserDataSupportEquipment> ApplySortFilter(List<UserDataSupportEquipment> userDataSupportEquipmentList)
        {
            var list = new List<UserDataSupportEquipment>(userDataSupportEquipmentList);
            
            list =　list.GetFilterSupportEquipmentList(sortFilterType, filterExcludeIdSet);
            list =　list.GetSortSupportEquipmentList(sortFilterType);
            
            var sortData = SortFilterUtility.GetSortDataByType(sortFilterType);
            var sortPriorityKey = SortFilterUtility.GetSortPriorityKey(sortData.priorityType);
            var isFilterKey = SortFilterUtility.GetIsFilterKey(SortFilterUtility.IsFilter(sortFilterType));
            sortPriorityText.text = StringValueAssetLoader.Instance[sortPriorityKey];
            
            SetSortOrderUI(sortData.orderType);
            
            isFilterText.text = StringValueAssetLoader.Instance[isFilterKey];

            return list;
        }
        
        public void SetFilterExcludeIdSet(IEnumerable<long> idList)
        {
            filterExcludeIdSet = idList.ToHashSet();
        }

        public void OnClickSortFilterButton()
        {
            OpenSupportEquipmentSortFilterModalAsync().Forget();
        }
        
        private async UniTask OpenSupportEquipmentSortFilterModalAsync()
        {
            SortFilterBaseModal<SupportEquipmentSortData,SupportEquipmentFilterData>.Data args = new SortFilterBaseModal<SupportEquipmentSortData, SupportEquipmentFilterData>.Data(SortFilterSheetType.Filter, sortFilterType);
            CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.SupportEquipmentSortFilter, args, this.GetCancellationTokenOnDestroy());


            bool isApplySortFilter = (bool)await modalWindow.WaitCloseAsync();
            if (!isApplySortFilter) return;
            
            InitializeScroll(originFavoriteHashSet);
            OnSortFilter?.Invoke();
        }

        public void OnClickReversalAscendingDescendingButton()
        {
            SortDataBase sortData = SortFilterUtility.GetSortDataByType(sortFilterType);
            SupportEquipmentSortData newSortData = new SupportEquipmentSortData()
            {
                orderType = SortFilterUtility.GetReversalOrderType(sortData.orderType),
                priorityType = sortData.priorityType
            };
            
            SortFilterUtility.SaveSortData(sortFilterType, newSortData);
            List<SupportEquipmentScrollData> editingList = new();
            List<SupportEquipmentScrollData> reverseItemList = new();
            
            //編成中のリスト順序を保存
            foreach (SupportEquipmentScrollData supportEquipment in ItemList)
            {
                if (formattingIdHashSet.Contains(supportEquipment.Id))
                {
                    editingList.Add(supportEquipment);
                }
                else
                {
                    reverseItemList.Add(supportEquipment);
                }
            }

            //ソート&ソート後のリストを取得
            List<SupportEquipmentScrollData> supportEquipmentScrollDatas = reverseItemList.SortSupportEquipmentListSortOrder(newSortData, editingList);
            
            SetSortOrderUI(newSortData.orderType);

            int scrollIndex = 0;
            detailOrderList.Clear();
            
            // 矢印データ更新
            foreach (SupportEquipmentScrollData scrollData in supportEquipmentScrollDatas)
            {
                detailOrderList.Add(new SupportEquipmentDetailData(scrollData.USupportEquipment));
                scrollData.SwipeableParams = new SwipeableParams<SupportEquipmentDetailData>(detailOrderList, scrollIndex++);
            }
            
            supportEquipmentScrollDataList = supportEquipmentScrollDatas;
            Scroll.SetItems(supportEquipmentScrollDatas);

            OnReverseSupportEquipmentOrder?.Invoke();
        }
        
        /// <summary>選択中のIdが除外リストに入っている場合は調整する</summary>
        public void RefreshSelectionId()
        {
            bool isUpdate = false;
                
            foreach(SupportEquipmentScrollData data in selectedSupportEquipmentScrollDataHashSet.ToList())
            {
                // 除外リストにあるかチェック
                if(excludeSelectionIdSet.Contains(data.Id))
                {
                    data.IsSelecting = false;
                    selectedSupportEquipmentScrollDataHashSet.Remove(data);
                    OnDeselectSupportEquipment?.Invoke(data);
                    isUpdate = true;
                }
            }
            
            // 更新があった場合はビューの更新
            if(isUpdate)
            {
                RefreshItemView();
            }
        }

        public void SelectSupportEquipment(SupportEquipmentScrollData scrollData)
        {
            if (excludeSelectionIdSet.Contains(scrollData.Id))
            {
                return;
            }
            
            switch (selectionMode)
            {
                case SelectionMode.None:
                    return;
                case SelectionMode.Single:
                    if (selectedSupportEquipmentScrollDataHashSet.Contains(scrollData))
                    {
                        OnSelectSupportEquipment?.Invoke(scrollData);
                        break;    
                    }  
                    
                    foreach (var data in selectedSupportEquipmentScrollDataHashSet.ToList())
                    {
                        data.IsSelecting = false;
                        selectedSupportEquipmentScrollDataHashSet.Remove(data);
                        OnDeselectSupportEquipment?.Invoke(data);
                    }
                    
                    
                    if (showSelectionEffect)
                    {
                        scrollData.IsSelecting = true;
                    }
                    
                    selectedSupportEquipmentScrollDataHashSet.Add(scrollData);
                    OnSelectSupportEquipment?.Invoke(scrollData);
                    break;
                case SelectionMode.Multiple:
                    if (selectedSupportEquipmentScrollDataHashSet.Contains(scrollData))
                    {
                        if (showSelectionEffect)
                        {
                            scrollData.IsSelecting = false;
                        }
                            
                        selectedSupportEquipmentScrollDataHashSet.Remove(scrollData);
                        OnDeselectSupportEquipment?.Invoke(scrollData);
                    }
                    else
                    {
                        if (showSelectionEffect)
                        {
                            scrollData.IsSelecting = true;
                        }
                            
                        selectedSupportEquipmentScrollDataHashSet.Add(scrollData); 
                        OnSelectSupportEquipment?.Invoke(scrollData);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RefreshItemView();
        }
        
        public void OnDetailModalIndexChange(int index)
        {
            var scrollData = ItemListSrc[index];
            if (selectionMode == SelectionMode.Single)
            {
                if (selectedSupportEquipmentScrollDataHashSet.Contains(scrollData))
                {
                    return;
                }  
                
                foreach (var data in selectedSupportEquipmentScrollDataHashSet.ToList())
                {
                    data.IsSelecting = false;
                    selectedSupportEquipmentScrollDataHashSet.Remove(data);
                }
                if (showSelectionEffect)
                {
                    scrollData.IsSelecting = true;
                }
                
                selectedSupportEquipmentScrollDataHashSet.Add(scrollData);
            }
            
            OnSwipeDetailModal?.Invoke(scrollData);
            RefreshItemView();
        }

        private void SetSortOrderUI(OrderType type)
        {
            Vector3 scale = sortOrderImage.transform.localScale;
            scale.y = type == OrderType.Descending ? 1 : -1;
            sortOrderImage.transform.localScale = scale;
            sortOrderText.text = StringValueAssetLoader.Instance[SortFilterUtility.GetSortOrderKey(type)];
        }
    }
}