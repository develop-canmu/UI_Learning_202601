using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.UserData;

namespace Pjfb
{
    public class UserCharacterVariableScroll : CharacterVariableScroll
    {
        [SerializeField] private bool showDeckFormattingBadge = false;
        [SerializeField] private AnimationClip selectAnimationClip;

        public bool ShowDeckFormattingBadge => showDeckFormattingBadge;
        private DeckListData deckListData;
        private DeckListData clubMatchDeckListData;
        private DeckListData leagueMatchDeckListData;
        private DeckListData clubRoyalDeckListData;
        private HashSet<long> filterExcludeIdSet;
        public ReadOnlyCollection<CharacterVariableDetailData> DetailOrderList => detailOrderList.AsReadOnly();
        private List<CharacterVariableDetailData> detailOrderList = new();
        
        // 結果
        private List<CharacterVariableScrollData> characterVariableScrollDataList = new List<CharacterVariableScrollData>();
        private List<UserDataCharaVariable> hasUserCharacterVariableList = new List<UserDataCharaVariable>();
        
        private float SelectEffectAnimationNormalizedTime => animationTime / selectAnimationClip.length;
        private float animationTime;
        
        private void Update()
        {
            animationTime += Time.deltaTime;
            if (animationTime >= selectAnimationClip.length) animationTime -= selectAnimationClip.length;
        }

        public void SetDeckListData(DeckListData deck, DeckListData clubMatchDeck, DeckListData leagueMatchDeck, DeckListData clubRoyalDeck)
        {
            deckListData = deck;
            clubMatchDeckListData = clubMatchDeck; 
            leagueMatchDeckListData = leagueMatchDeck;
            clubRoyalDeckListData = clubRoyalDeck;
        }

        public void SetFilterExcludeIdSet(IEnumerable<long> idList)
        {
            filterExcludeIdSet = idList.ToHashSet();
        }
        
        public void SetUserCharacterVariableList()
        {
            hasUserCharacterVariableList = UserDataManager.Instance.charaVariable.data.Values.ToList();
        }

        public void SetCharacterVariableList()
        {
            characterVariableScrollDataList.Clear();
            detailOrderList.Clear();
            int scrollIndex = 0;
            foreach(UserDataCharaVariable chara in ApplySortFilter(hasUserCharacterVariableList))
            {
                characterVariableScrollDataList.Add( CreateCharacterVariableScrollData(chara, scrollIndex++));
                detailOrderList.Add(new CharacterVariableDetailData(chara));
            }
        }
        
        protected override List<CharacterVariableScrollData> GetItemList()
        {
            animationTime = 0;
            return characterVariableScrollDataList;
        }
        
        private List<UserDataCharaVariable> ApplySortFilter(List<UserDataCharaVariable> userDataCharaVariableList)
        {
            var list = new List<UserDataCharaVariable>(userDataCharaVariableList);

            list =　list.GetFilterSuccessCharacterList(sortFilterType, filterExcludeIdSet);
            list =　list.GetSortSuccessCharacterList(sortFilterType);
            
            var sortData = SortFilterUtility.GetSortDataByType(sortFilterType);
            var sortOrderKey = SortFilterUtility.GetSortOrderKey(sortData.orderType);
            var sortPriorityKey = SortFilterUtility.GetSortPriorityKey(sortData.priorityType);
            var isFilterKey = SortFilterUtility.GetIsFilterKey(SortFilterUtility.IsFilter(sortFilterType));
            sortPriorityText.text = StringValueAssetLoader.Instance[sortPriorityKey];
            Vector3 scale = sortOrderImage.transform.localScale;
            scale.y = sortData.orderType == OrderType.Descending ? 1 : -1;
            sortOrderImage.transform.localScale = scale;
            sortOrderText.text = StringValueAssetLoader.Instance[sortOrderKey];
            isFilterText.text = StringValueAssetLoader.Instance[isFilterKey];

            return list;
        }
        public Action<CharacterVariableScrollData> OnSwipeDetailModal;
        private void OnDetailModalIndexChange(int index)
        {
            OnSwipeDetailModal?.Invoke(ItemListSrc[index]);
        }
        
        private CharacterVariableScrollData CreateCharacterVariableScrollData(UserDataCharaVariable chara, int detailOrderIndex)
        {
            DeckBadgeType deckBadgeType = showDeckFormattingBadge && 
                (deckListData != null && deckListData.Contains(chara.id) || clubMatchDeckListData != null && clubMatchDeckListData.Contains(chara.id) || leagueMatchDeckListData != null && leagueMatchDeckListData.Contains(chara.id) || clubRoyalDeckListData != null && clubRoyalDeckListData.Contains(chara.id))
                    ? DeckBadgeType.Formatting
                    : DeckBadgeType.None;
            return new CharacterVariableScrollData(chara, () => SelectEffectAnimationNormalizedTime, new SwipeableParams<CharacterVariableDetailData>(detailOrderList, detailOrderIndex,
                OnDetailModalIndexChange), deckBadgeType);
        }

        public async void OnClickSortFilterButton()
        {
            var isApplySortFilter = await OpenConfirmModalAsync(ModalType.SuccessCharacterSortFilter, SortFilterSheetType.Filter);
            if (!isApplySortFilter) return;
            SetCharacterVariableList();
            Refresh();
            OnSortFilter?.Invoke();
        }
        
        public void OnClickReversalAscendingDescendingButton()
        {
            // Todo : Optimize
            // GetItems() and reverse order?
            var sortData = SortFilterUtility.GetSortDataByType(sortFilterType);
            var newSortData = new SuccessCharacterSortData
            {
                orderType = SortFilterUtility.GetReversalOrderType(sortData.orderType),
                priorityType = sortData.priorityType
            };
            SortFilterUtility.SaveSortData(sortFilterType, newSortData);
            SetCharacterVariableList();
            Refresh();
            OnReverseCharacterOrder?.Invoke();
        }
        
        private async UniTask<bool> OpenConfirmModalAsync(ModalType modalType, SortFilterSheetType sheetType)
        {
            var args = new SortFilterBaseModal<SuccessCharacterSortData, SuccessCharacterFilterData>.Data(sheetType, sortFilterType);
            CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(modalType, args, this.GetCancellationTokenOnDestroy());
            // モーダルが閉じるまで待機
            return (bool)await modalWindow.WaitCloseAsync();
        }
        
    }
}

