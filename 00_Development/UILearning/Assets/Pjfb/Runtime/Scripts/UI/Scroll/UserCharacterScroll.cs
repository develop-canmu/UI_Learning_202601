using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb
{
    
    public class UserCharacterScroll : CharacterScroll
    {
        [Flags]
        public enum ViewFlags
        {
            None = 0,
            /// <summary>所持キャラの表示</summary>
            HasUserCharacter    = 1 << 0, 
            /// <summary>未所持キャラの表示</summary>
            NonHasUserCharacter = 1 << 1
        }
        
        
        [SerializeField]
        private CardType cardType = CardType.Character;
        public CardType CardType => cardType;
        
        [SerializeField] protected ViewFlags viewFlags = ViewFlags.HasUserCharacter;

        [SerializeField] private BaseCharacterType baseCharacterType = BaseCharacterType.Character;
        
        [SerializeField]
        private SortFilterModalOption filterOptions = SortFilterModalOption.None;
        /// <summary>フィルタオプション</summary>
        public SortFilterModalOption FilterOptions{get{return filterOptions;}set{filterOptions = value;}}
        
        // 所持キャラを未所持キャラより優先して表示するか
        [SerializeField] private bool prioritizeHasCharacters = true;
        
        [SerializeField] private bool canLiberation = false;
        [SerializeField] private bool canGrowth = false;
        // 結果
        protected List<CharacterScrollData> characterScrollDataList = new List<CharacterScrollData>();
        private List<CharacterScrollData> hasCharacterScrollDataList = new List<CharacterScrollData>();
        private List<CharacterScrollData> nonHasCharacterScrollDataList = new List<CharacterScrollData>();
        private List<UserDataChara> hasUserCharacterList = new List<UserDataChara>();
        private List<UserDataChara> nonHasUserCharacterList = new List<UserDataChara>();
        private HashSet<long> filterExcludeIdSet;
        public Func<long> GetTrainingScenarioId;

        public ReadOnlyCollection<CharacterDetailData> DetailOrderList => detailOrderList.AsReadOnly();
        private List<CharacterDetailData> detailOrderList = new();
        
        public List<UserDataChara> NonHasUserCharacterList => nonHasUserCharacterList;

        public void SetUserCharacterList()
        {
            hasUserCharacterList = UserDataManager.Instance.GetUserDataCharaListByType(cardType).ToList();
            nonHasUserCharacterList = CreateNonHasDefaultUserDataCharaList();
        }
        
        public void SetUserNonExtraCharacterList()
        {
            hasUserCharacterList = UserDataManager.Instance.GetUserDataNonExtraCharaList().ToList();
            nonHasUserCharacterList = CreateNonHasDefaultUserDataCharaList();
        }
        
        public void SetUserExtraCharacterList()
        {
            hasUserCharacterList = UserDataManager.Instance.GetUserDataExtraCharaList().ToList();
            nonHasUserCharacterList = CreateNonHasDefaultUserDataCharaList();
        }

        public void SetSortCharaList(List<UserDataChara> userDataCharaList)
        {
            characterScrollDataList.Clear();
            string message = string.Empty;
            string description = string.Empty;
            Color filterColor = Color.white;
            int scrollIndex = 0;
            foreach (UserDataChara c in userDataCharaList)
            {
                CharacterScrollDataOptions options = CharacterScrollDataOptions.None;
                // 選択中
                if (selectedCharacterIds.Contains(c.id)) options |= CharacterScrollDataOptions.Selected;
                // 強化/能力解放可能
                if (enableGrowthLiberationCharacterIds.Contains(c.id)) options |= CharacterScrollDataOptions.Badge;

                // リストに追加
                var scrollData = new CharacterScrollData(c.charaId, c.level, c.newLiberationLevel, c.id, message, description, filterColor, this, new SwipeableParams<CharacterDetailData>(detailOrderList, scrollIndex++, OnDetailModalIndexChange), options, baseCharacterType, () => GetTrainingScenarioId?.Invoke() ?? -1);
                characterScrollDataList.Add(scrollData);
            }
            Refresh();
        }

        /// <summary>
        /// 指定されたキャラクターIDリストから表示用データを作成する
        /// </summary>
        public void SetCharacterListByIds(IEnumerable<long> characterIds)
        {
            hasUserCharacterList.Clear();
            nonHasUserCharacterList.Clear();
            Dictionary<long, UserDataChara> userCharaDictionary = new Dictionary<long, UserDataChara>();
            foreach (UserDataChara uChara in UserDataManager.Instance.chara.data.Values)
            {
                // 各キャラクターデータ(uChara)を、そのcharaIdをキーとしてDictionaryに追加する
                userCharaDictionary[uChara.charaId] = uChara;
            }
            
            foreach (long charaId in characterIds)
            {
                if (userCharaDictionary.TryGetValue(charaId, out UserDataChara userChara))
                {
                    hasUserCharacterList.Add(userChara);
                }
                else
                {
                    UserDataChara dummyChara = new UserDataChara(new CharaV2Base
                    {
                        id = -1,
                        mCharaId = charaId,
                        level = 1,
                        newLiberationLevel = 0
                    });
                    nonHasUserCharacterList.Add(dummyChara);
                }
            }
        }

        public Action<CharacterScrollData> OnSwipeDetailModal;

        private void OnDetailModalIndexChange(int index)
        {
            OnSwipeDetailModal?.Invoke(ItemListSrc[index]);
        }
        public virtual void SetCharacterList()
        {
            characterScrollDataList.Clear();
            hasCharacterScrollDataList.Clear();
            nonHasCharacterScrollDataList.Clear();
            detailOrderList.Clear();
            int scrollIndex = 0;
            // ソート/フィルター適用後のUserDataCharaリスト
            List<UserDataChara> sortFilteredCharaDataList;
            
            // 所持キャラを優先して表示する場合、所持と未所持のそれぞれにソートフィルターを適用した後に結合
            if (prioritizeHasCharacters)
            {
                var sortedHasList = new List<UserDataChara>();
                var sortedNonHasList = new List<UserDataChara>();
                
                // 所持キャラデータをソート
                if ((viewFlags & ViewFlags.HasUserCharacter) != ViewFlags.None)
                {
                    sortedHasList = ApplySortFilter(hasUserCharacterList);
                }
                // 未所持キャラデータをソート
                if ((viewFlags & ViewFlags.NonHasUserCharacter) != ViewFlags.None)
                {
                    sortedNonHasList = ApplySortFilter(nonHasUserCharacterList);
                }
                
                // 所持キャラ -> 未所持キャラ の順で結合
                sortFilteredCharaDataList = sortedHasList;
                sortFilteredCharaDataList.AddRange(sortedNonHasList);
            }
            // 所持・未所持キャラを混在して表示する場合、所持と未所持を結合した後にソートフィルターを適用
            else
            {
                var combinedCharaDataList = new List<UserDataChara>();
                // 所持キャラデータと未所持キャラデータを結合
                if ((viewFlags & ViewFlags.HasUserCharacter) != ViewFlags.None)
                {
                    combinedCharaDataList.AddRange(hasUserCharacterList);
                }
                if ((viewFlags & ViewFlags.NonHasUserCharacter) != ViewFlags.None)
                {
                    combinedCharaDataList.AddRange(nonHasUserCharacterList);
                }
                
                // ソートフィルターを適用
                sortFilteredCharaDataList = ApplySortFilter(combinedCharaDataList);
            }

            // ソートフィルター済のキャラデータからスクロールデータを作成
            foreach (UserDataChara charaData in sortFilteredCharaDataList)
            {
                // 所持キャラか未所持キャラかを判定
                bool isHasCharacter = charaData.id >= 0;
                // スクロールデータの作成
                CharacterScrollData scrollData = CreateCharacterScrollData(charaData, scrollIndex, isHasCharacter);
                
                // メンバのリストに保存
                if (isHasCharacter)
                {
                    hasCharacterScrollDataList.Add(scrollData);
                }
                else
                {
                    nonHasCharacterScrollDataList.Add(scrollData);
                }
                
                characterScrollDataList.Add(scrollData);
                detailOrderList.Add(new CharacterDetailData(charaData));
                scrollIndex++;
            }
        }
        
        /// <summary> UserDataChara から CharacterScrollData を作成 </summary>
        protected virtual CharacterScrollData CreateCharacterScrollData(UserDataChara charaData, int scrollIndex, bool isHasCharacter)
        {
            // 所持キャラの場合、各種データやオプションを適用
            if (isHasCharacter)
            {
                string message = string.Empty;
                string description = string.Empty;
                Color filterColor = Color.white;
                CharacterScrollDataOptions options = CharacterScrollDataOptions.None;
                
                long parentId = CharacterUtility.UserCharIdToParentId(charaData.id);
                // 選択中
                if(selectedCharacterIds.Contains(charaData.id))options |= CharacterScrollDataOptions.Selected;
                // 選択不可
                if (disableCharacterParentIds.Contains(parentId)) options |= CharacterScrollDataOptions.Disable;
                // 強化/能力解放可能
                if (enableGrowthLiberationCharacterIds.Contains(charaData.id)) options |= CharacterScrollDataOptions.Badge;
                // 強化画面へ遷移可能
                if (canGrowth) options |= CharacterScrollDataOptions.CanGrowth;
                // フレンド設定中
                if (friendSettingsCharacterId == (charaData.id)) options |= CharacterScrollDataOptions.FriendSetting;
                // トレーニング育成キャラ
                if (TrainingCharacterId == parentId)
                {
                    message = StringValueAssetLoader.Instance["training.training_character"];
                    description = StringValueAssetLoader.Instance[$"training.select_page_message{(long)cardType}"];
                    filterColor = ColorValueAssetLoader.Instance["character.filter.default"];
                }
                // 編成制限対象だった場合制限をかける
                else if (limitedCharacterIds.Contains(charaData.charaId))
                {
                    // 選択不可に
                    options |= CharacterScrollDataOptions.Disable;
                    message = StringValueAssetLoader.Instance["training.limit.message"];
                    description = StringValueAssetLoader.Instance["training.deck_limit.message"];
                    filterColor = ColorValueAssetLoader.Instance["character.filter.limit"];
                }
                // 他のデッキスロットに編成されていて編成不可の場合
                else if (disableDeckSlotCharacterIds.Contains(charaData.id))
                {
                    // 選択不可に
                    options |= CharacterScrollDataOptions.Disable;
                    message = StringValueAssetLoader.Instance["deck.character.disable_deck_slot"];
                    filterColor = ColorValueAssetLoader.Instance["character.filter.default"];
                }
                
                // 特攻
                if (CharacterUtility.IsTrainingScenarioSpAttackCharacter(charaData.MChara.id, charaData.level, TrainingScenarioId))
                {
                    options |= CharacterScrollDataOptions.ScenarioSpecialAttack;
                }
                
                // 今の編成枠に設定されているキャラ
                if (currentSelectCharacterId == charaData.charaId)
                {
                    options |= CharacterScrollDataOptions.CurrentSelect;
                }

                // スクロールデータ生成
                return new CharacterScrollData(charaData.charaId, charaData.level, charaData.newLiberationLevel, charaData.id, message,
                    description, filterColor, this, new SwipeableParams<CharacterDetailData>(detailOrderList, scrollIndex,
                        OnDetailModalIndexChange), options, baseCharacterType, () => GetTrainingScenarioId?.Invoke() ?? -1);
            }
            // 未所持キャラの場合、初期値を適用
            else
            {
                CharacterScrollDataOptions options = CharacterScrollDataOptions.None;
                options |= CharacterScrollDataOptions.NonPossession;
                if (canLiberation) options |= CharacterScrollDataOptions.CanLiberation;
                // 解放可能
                if (enablePieceToCharacterIds.Contains(charaData.charaId)) options |= CharacterScrollDataOptions.Badge;
                
                // スクロールデータ生成
                return new CharacterScrollData(charaData.charaId, 1, 0, -1,
                    new SwipeableParams<CharacterDetailData>(detailOrderList, scrollIndex,
                        OnDetailModalIndexChange), options, baseCharacterType);
            }
        }
        
        public void SetFilterExcludeIdSet(IEnumerable<long> idList)
        {
            filterExcludeIdSet = idList.ToHashSet();
        }
        
        protected override List<CharacterScrollData> GetItemList()
        {
            return characterScrollDataList;
        }

        private List<UserDataChara> ApplySortFilter(List<UserDataChara> userDataCharaList)
        {
            var list = new List<UserDataChara>(userDataCharaList);
            
            // UserDataCharaリストのソートフィルター適用
            list = GetSortFilteredUserDataCharaList(list);
            
            // 固定キャラId
            for(int i=fixedCharacterIds.Count-1;i>=0;i--)
            {
                long id = fixedCharacterIds[i];
                if(id >= 0)
                {
                    list.Insert(0, UserDataManager.Instance.chara.Find(id) );
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
        
        /// <summary>
        /// カードタイプに応じたソートデータを作成
        /// </summary>
        protected virtual SortDataBase CreateSortDataByCardType()
        {
            switch(cardType)
            {
                case CardType.Character:
                    return new BaseCharacterSortData();
                case CardType.SpecialSupportCharacter:
                    return new SpecialSupportCardSortData();
                case CardType.Adviser:
                    return new AdviserSortData();
                default:
                    CruFramework.Logger.LogError($"CreateDefaultSortDataByCardType: 未対応のCardTypeです, cardType={cardType}, sortFilterType={sortFilterType}");
                    return null;
            }
        }
        
        /// <summary> ソートフィルターが掛かったUserDataCharaのリストを返す </summary>
        protected virtual List<UserDataChara> GetSortFilteredUserDataCharaList(List<UserDataChara> list)
        {
            switch (cardType)
            {
                case CardType.None:
                    break;
                case CardType.Character:
                {
                    list = list.GetFilterBaseCharacterList(sortFilterType, filterExcludeIdSet, fixedCharacterIds);
                    list = list.GetSortBaseCharacterList(sortFilterType);
                    break;
                }
                case CardType.SpecialSupportCharacter:
                {
                    list = list.GetFilterSpecialSupportCardList(sortFilterType, filterExcludeIdSet, fixedCharacterIds);
                    list = list.GetSortSpecialSupportCardList(sortFilterType);
                    break;
                }
                case CardType.Adviser:
                {
                    list = list.GetFilterAdviserList(sortFilterType, filterExcludeIdSet, fixedCharacterIds);
                    list = list.GetSortAdviserList(sortFilterType);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return list;
        }

        private List<UserDataChara> CreateNonHasDefaultUserDataCharaList()
        {
            var list = new List<UserDataChara>();
            
            foreach(CharaMasterObject c in MasterManager.Instance.charaMaster.values)
            {
                // ユーザーが所持している
                if(UserDataManager.Instance.chara.data.Values.Any(data => data.charaId == c.id))continue;
                // タイプをチェック
                if(c.cardType != cardType) continue;
                // chara.priceFromPiece = -1のキャラは解放一覧に表示しない
                if(c.priceFromPiece < 0) continue;
                var chara =  new UserDataChara(new CharaV2Base
                {
                    id = -1,
                    mCharaId = c.id,
                    level = 1,
                    newLiberationLevel = 0
                });
                // リストに追加
                list.Add(chara);
            }
            
            return list;
        }

        public int GetListMaxCount()
        {
            return hasUserCharacterList.Count + nonHasUserCharacterList.Count;
        }
        
       public void OnClickSortFilterButton()
        {
            OpenSortFilterModalAsync().Forget();
        }

        /// <summary>
        /// 決定したソート・フィルターモーダルを開き、適用後処理を実行する。
        /// </summary>
        private async UniTask OpenSortFilterModalAsync()
        {
            CruFramework.Page.ModalWindow modalWindow = await GetDetermineModalWindow();
            bool isFilterApplied = (bool)await modalWindow.WaitCloseAsync();
            if (isFilterApplied)
            {
                SetCharacterList();
                Refresh();
                OnSortFilter?.Invoke();
            }
        }
        
        /// <summary>
        /// どのソート・フィルターモーダルを開くかを決定する。
        /// </summary>
        protected virtual async UniTask<CruFramework.Page.ModalWindow> GetDetermineModalWindow()
        {
            ModalType modalType = new ModalType();
            object args = new object();
            
            switch (cardType)
            {
                case CardType.Character:
                {
                    modalType = ModalType.BaseCharacterSortFilter;
                    args = new SortFilterBaseModal<BaseCharacterSortData, BaseCharacterFilterData>.Data(SortFilterSheetType.Filter, sortFilterType);
                    break;
                }
                case CardType.SpecialSupportCharacter:
                {
                    modalType = ModalType.SpecialSupportCardSortFilter;
                    args = new SortFilterBaseModal<SpecialSupportCardSortData, SpecialSupportCardFilterData>.Data(SortFilterSheetType.Filter, sortFilterType, filterOptions);
                    break;
                }
                case CardType.Adviser:
                {
                    modalType = ModalType.AdviserSortFilter;
                    args = new SortFilterBaseModal<AdviserSortData, AdviserFilterData>.Data(SortFilterSheetType.Filter, sortFilterType);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(modalType, args, this.GetCancellationTokenOnDestroy());
        }
        
        public void OnClickReversalAscendingDescendingButton()
        {
            // 現在のソートデータ
            SortDataBase sortData = SortFilterUtility.GetSortDataByType(sortFilterType);
            
            // 新規ソートデータを作成
            SortDataBase newSortData = CreateSortDataByCardType();
            
            newSortData.orderType = SortFilterUtility.GetReversalOrderType(sortData.orderType);
            newSortData.priorityType = sortData.priorityType;
            // 新規ソートデータを保存
            SortFilterUtility.SaveSortData(sortFilterType, newSortData);
            
            SetCharacterList();
            Refresh();
            OnReverseCharacterOrder?.Invoke();
        }
        
        private async UniTask<bool> OpenConfirmModalAsync(ModalType modalType, SortFilterSheetType sheetType)
        {
            var args = new object();
            switch (cardType)
            {
                case CardType.None:
                    break;
                case CardType.Character:
                    args = new SortFilterBaseModal<BaseCharacterSortData, BaseCharacterFilterData>.Data(sheetType, sortFilterType);
                    break;
                case CardType.SpecialSupportCharacter:
                    args = new SortFilterBaseModal<SpecialSupportCardSortData, SpecialSupportCardFilterData>.Data(sheetType, sortFilterType, filterOptions);
                    break;
                case CardType.Adviser:
                {
                    args = new SortFilterBaseModal<AdviserSortData, AdviserFilterData>.Data(sheetType, sortFilterType);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(modalType, args, this.GetCancellationTokenOnDestroy());
            return (bool)await modalWindow.WaitCloseAsync();
        }
        
        public void SelectItemByUCharaId(long id, out int index)
        {
            index = ItemListSrc.ToList().FindIndex(x => x.UserCharacterId == id);
            if (index == -1)return;
            Scroll.SelectItem(index);
        }

        public void SelectItemByUCharaId(long id)
        {
            SelectItemByUCharaId(id, out _);
        }
        
        public void SelectItem(CharacterScrollData data)
        {
            int index = ItemListSrc.ToList().IndexOf(data);
            if (index == -1) return;
            Scroll.SelectItem(index);
        }
        
        /// <summary>
        /// mCharacterIdでアイテムを選択する
        /// </summary>
        public void SelectItemBymCharacterId(long characterId)
        {
            List<CharacterScrollData> itemList = GetItemList();
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].CharacterId == characterId)
                {
                    scrollGrid.SelectItem(i);
                    return;
                }
            }
        }
    }
}