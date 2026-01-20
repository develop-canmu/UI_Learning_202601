using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb
{
    
    public class FriendCharacterScroll : CharacterScroll
    {
        
        public const int RELATION_TYPE_FOLLOW  = 1;
        public const int RELATION_TYPE_FOLLOWED  = 2;
        public const int RELATION_TYPE_MUTUAL_FOLLOW  = 3;
        
        public Action<CharacterScrollData> OnSwipeDetailModal;

        public Func<long> GetTrainingScenarioId;
        // 結果
        List<CharacterScrollData> friendList = new List<CharacterScrollData>();

        private CharaV2FriendLend[] friendDatas;

        private void OnDetailModalIndexChange(int index)
        {
            OnSwipeDetailModal?.Invoke(ItemListSrc[index]);
        }
        
        public void SetFriendList(CharaV2FriendLend[] friends)
        {
            friendDatas = friends;
            friendList.Clear();
            List<CharacterDetailData> detailOrderList = new();
            int scrollIndex = 0;
            foreach(CharaV2FriendLend friend in ApplySortFilter(friendDatas.ToList()))
            {
                string message = string.Empty;
                string description = string.Empty;
                Color filterColor = Color.white;
                CharacterScrollDataOptions options = CharacterScrollDataOptions.None;
                
                long parentId = CharacterUtility.CharIdToParentId(friend.mCharaId);
                // 選択中
                if(selectedCharacterIds.Contains(friend.id))options |= CharacterScrollDataOptions.Selected;
                // 選択不可
                if(disableCharacterParentIds.Contains( parentId))options |= CharacterScrollDataOptions.Disable;
                // 特攻
                if( CharacterUtility.IsTrainingScenarioSpAttackCharacter(friend.mCharaId, friend.level, TrainingScenarioId))
                {
                    options |= CharacterScrollDataOptions.ScenarioSpecialAttack;
                }
                // フォロー
                switch(friend.relationType)
                {
                    case RELATION_TYPE_FOLLOWED:
                        options |= CharacterScrollDataOptions.Follow;
                        break;
                    case RELATION_TYPE_MUTUAL_FOLLOW:
                        options |= CharacterScrollDataOptions.MutualFoolow;
                        break;
                }
                
                // 育成対象
                if(TrainingCharacterId == parentId)
                {
                    message = StringValueAssetLoader.Instance["training.training_character"];
                    description = StringValueAssetLoader.Instance["training.select_page_message1"];
                    filterColor = ColorValueAssetLoader.Instance["character.filter.default"];
                }
                // 編成制限対象だった場合制限をかける
                else if (limitedCharacterIds.Contains(friend.mCharaId))
                {
                    // 選択不可に
                    options |= CharacterScrollDataOptions.Disable;
                    message = StringValueAssetLoader.Instance["training.limit.message"];
                    description = StringValueAssetLoader.Instance["training.deck_limit.message"];
                    filterColor = ColorValueAssetLoader.Instance["character.filter.limit"];
                }
                
                // 自分の選手の場合、強化ボタンを表示
                if(UserDataManager.Instance.chara.data.ContainsKey(friend.id) == true)
                {
                    options |= CharacterScrollDataOptions.CanGrowth;
                }
                
                // リストの追加
                friendList.Add(new CharacterScrollData(friend.mCharaId, friend.level,
                    friend.newLiberationLevel, friend.id, message, description, filterColor, this,
                    new SwipeableParams<CharacterDetailData>(detailOrderList, scrollIndex++, OnDetailModalIndexChange),
                    options, BaseCharacterType.SupportCharacter, () => GetTrainingScenarioId?.Invoke() ?? -1));
                detailOrderList.Add(new CharacterDetailData(friend.id, friend.mCharaId, friend.level, friend.newLiberationLevel));
            }
        }
        
        protected override List<CharacterScrollData> GetItemList()
        {
            return friendList;
        }
        
        private List<CharaV2FriendLend> ApplySortFilter(List<CharaV2FriendLend> friendCharaList)
        {
            var list = new List<CharaV2FriendLend>(friendCharaList);
            
            list =　list.GetFilterBaseCharacterList(sortFilterType, selectedCharacterIds, fixedCharacterIds);
            list =　list.GetSortBaseCharacterList(sortFilterType);
            
            // 固定キャラId
            for(int i=fixedCharacterIds.Count-1;i>=0;i--)
            {
                long id = fixedCharacterIds[i];
                foreach(CharaV2FriendLend friend in friendCharaList)
                {
                    if(friend.id == id)
                    {
                        list.Insert(0, friend );
                    }
                }                
            }

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
        
        public async void OnClickSortFilterButton()
        {
            var isApplySortFilter = await OpenConfirmModalAsync(ModalType.BaseCharacterSortFilter, SortFilterSheetType.Filter);
            if (!isApplySortFilter) return;
            SetFriendList(friendDatas);
            Refresh();
            OnSortFilter?.Invoke();
        }
        
        public void OnClickReversalAscendingDescendingButton()
        {
            var baseCharacterSortData = SortFilterUtility.GetSortDataByType(sortFilterType);
            var newSortData = new BaseCharacterSortData
            {
                orderType = SortFilterUtility.GetReversalOrderType(baseCharacterSortData.orderType),
                priorityType = baseCharacterSortData.priorityType
            };
            SortFilterUtility.SaveSortData(sortFilterType, newSortData);
            SetFriendList(friendDatas);
            Refresh();
            OnReverseCharacterOrder?.Invoke();
        }
        
        private async UniTask<bool> OpenConfirmModalAsync(ModalType modalType, SortFilterSheetType sheetType)
        {
            var args = new SortFilterBaseModal<BaseCharacterSortData, BaseCharacterFilterData>.Data(sheetType, sortFilterType);
            CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(modalType, args, this.GetCancellationTokenOnDestroy());
            return (bool)await modalWindow.WaitCloseAsync();
        }
    }
}