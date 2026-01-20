using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Character;
using Pjfb.UserData;
using UnityEngine.UI;

namespace Pjfb
{
    
    public enum CharacterScrollFilterType
    {
    }
    
    public enum CharacterScrollSortType
    {
    }
    
    [Flags]
    public enum CharacterScrollDataOptions
    {
        None = 0,
        Selected = 1 << 0,
        Disable = 1 << 1,
        NonPossession = 1 << 2,
        Badge = 1 << 3,
        FriendSetting = 1 << 4,
        StealReward = 1 << 5,
        CanLiberation = 1<<6,
        
        Follow = 1 << 7,
        MutualFoolow = 1 << 8,
        
        CanGrowth = 1 << 9,
        
        ScenarioSpecialAttack = 1 << 10,
        
        CurrentSelect = 1 << 11,
        DisableDetailModal  = 1 << 12,
    }
    
    public class CharacterScrollData
    {
        private long characterId = 0;
        /// <summary>キャラId</summary>
        public long CharacterId{get{return characterId;}}
        
        private long characterLv = 0;
        /// <summary>Lv</summary>
        public long CharacterLv{get{return characterLv;}}
        
        private long liberationLv = 0;
        /// <summary>Lv</summary>
        public long LiberationLv{get{return liberationLv;}}
        
        private long userCharacterId = 0;
        /// <summary>Lv</summary>
        public long UserCharacterId{get{return userCharacterId;}}
        
        private string message = string.Empty;
        /// <summary>Message表示</summary>
        public string Message{get{return message;}}
        
        private string description = string.Empty;
        /// <summary>説明表示</summary>
        public string Description{get{return description;}}

        private Color filterColor = Color.white;
        /// <summary>フィルターの色</summary>
        public Color FilterColor{get{return filterColor;}}
        
        private CharacterScroll scroll = null;
        /// <summary>Scroll</summary>
        public CharacterScroll Scroll{get{return scroll;}}

        private CharacterScrollDataOptions options = CharacterScrollDataOptions.None;
        /// <summary>選択中</summary>
        public CharacterScrollDataOptions Options{get{return options;}}

        public SwipeableParams<CharacterDetailData> SwipeableParams;
        public BaseCharacterType BaseCharacterType;
        public Func<long> GetTrainingScenarioId = null;
        
        public bool HasOption(CharacterScrollDataOptions option)
        {
            return (this.options & option) != CharacterScrollDataOptions.None;
        }
        
        public bool IsSelecting = false;
        // 選択番号
        public int SelectionNumber = 0;
        
        /// <summary>
        /// CharacterScrollDataOptionsを追加
        /// </summary>
        public void AddOption(CharacterScrollDataOptions characterScrollDataOptions)
        {
            options |= characterScrollDataOptions;
        }
        
        private void Initialize(long characterId, long characterLv, long liberationLv, long userCharacterId, string message,
            string description, Color filterColor, CharacterScroll scroll, SwipeableParams<CharacterDetailData> swipeableParams,
            CharacterScrollDataOptions options = CharacterScrollDataOptions.None, BaseCharacterType baseCharacterType = BaseCharacterType.Character, Func<long> getTrainingScenarioId = null)
        {
            this.characterId = characterId;
            this.characterLv = characterLv;
            this.liberationLv = liberationLv;
            this.userCharacterId = userCharacterId;
            this.message = message;
            this.description = description;
            this.filterColor = filterColor;
            this.scroll = scroll;
            this.options = options;
            SwipeableParams = swipeableParams;
            BaseCharacterType = baseCharacterType;
            GetTrainingScenarioId = getTrainingScenarioId;
        }

        public CharacterScrollData(long characterId, long characterLv, long liberationLv, long userCharacterId,  
            SwipeableParams<CharacterDetailData> swipeableParams = null, CharacterScrollDataOptions options = CharacterScrollDataOptions.None, BaseCharacterType baseCharacterType = BaseCharacterType.Character, Func<long> getTrainingScenarioId = null)
        {
            Initialize(characterId, characterLv, liberationLv, userCharacterId, string.Empty, string.Empty, Color.white, null, swipeableParams, options, baseCharacterType, getTrainingScenarioId);
        }

        public CharacterScrollData(long characterId, long characterLv, long liberationLv, long userCharacterId, string message, string description, Color filterColor, CharacterScroll scroll,
            SwipeableParams<CharacterDetailData> swipeableParams= null, CharacterScrollDataOptions options = CharacterScrollDataOptions.None, BaseCharacterType baseCharacterType = BaseCharacterType.Character, Func<long> getTrainingScenarioId = null)
        {
            Initialize(characterId, characterLv, liberationLv, userCharacterId, message, description, filterColor, scroll, swipeableParams,
                options, baseCharacterType, getTrainingScenarioId);
        }
    }

    public abstract class CharacterScrollBase<T> : ItemIconScroll<T>
    {

        [SerializeField] protected TMPro.TMP_Text sortPriorityText = null;
        [SerializeField] protected Image sortOrderImage;
        [SerializeField] protected TMPro.TMP_Text sortOrderText = null;
        [SerializeField] protected TMPro.TMP_Text isFilterText = null;
        [SerializeField] protected SortFilterUtility.SortFilterType sortFilterType = SortFilterUtility.SortFilterType.None;
        [SerializeField] private GameObject noCharacterText;

        public Action OnSortFilter;
        public Action OnReverseCharacterOrder;
        
        // 選択中のキャラクタ
        protected List<long> selectedCharacterIds = new List<long>();
        // 選択できないキャラ
        protected List<long> disableCharacterParentIds = new List<long>();
        // レアリティ制限キャラリスト
        protected List<long> limitedCharacterIds = new List<long>();
        // 強化/能力解放ができるキャラ
        protected List<long> enableGrowthLiberationCharacterIds = new List<long>();
        // 解放ができるキャラ
        protected List<long> enablePieceToCharacterIds = new List<long>();
        // 選択できないキャラ
        protected List<long> fixedCharacterIds = new List<long>();
        // フレンド貸出設定中
        protected long friendSettingsCharacterId = -1;
        // 前回の編成枠キャラ
        protected long currentSelectCharacterId = -1;
        
        private long trainingCharacterId = -1;
        /// <summary>トレーニング育成キャラ</summary>
        public long TrainingCharacterId{get{return trainingCharacterId;}set{trainingCharacterId = value;}}

        private long trainingScenarioId = -1;
        /// <summary>トレーニングシナリオId</summary>
        public long TrainingScenarioId{get{return trainingScenarioId;}set{trainingScenarioId = value;}}
        
        /// <summary>デッキ編成不可</summary>
        protected List<long> disableDeckSlotCharacterIds = new List<long>();

        public void Sort(CharacterScrollSortType sort)
        {
            OnSort(sort);
            scrollGrid.SetItems(ItemList);
        }
        
        public void Filter(CharacterScrollFilterType filter)
        {
            OnFilter(filter);
            scrollGrid.SetItems(ItemList);
        }

       
        

        
        /// <summary>選択中キャラ</summary>
        public void SetSelectedCharacterIds(IList<long> list)
        {
            selectedCharacterIds.Clear();
            selectedCharacterIds.AddRange(list);
        }
        
        /// <summary>選択中キャラ</summary>
        public void SetSelectedCharacterId(long id)
        {
            selectedCharacterIds.Clear();
            selectedCharacterIds.Add(id);
        }
        
        /// <summary>固定キャラ</summary>
        public void SetFixedCharacterIds(IList<long> list)
        {
            fixedCharacterIds.Clear();
            fixedCharacterIds.AddRange(list);
        }
        
        public void ClearFixedCharacterIds()
        {
            fixedCharacterIds.Clear();
        }
        
        public void ClearSelectedCharacterIds()
        {
            selectedCharacterIds.Clear();
        }
        
        /// <summary>選択できないキャラ</summary>
        public void SetDisableCharacterParentIds(IList<long> list)
        {
            disableCharacterParentIds.Clear();
            disableCharacterParentIds.AddRange(list);
        }
        
        public void ClearDisableCharacterParentIds()
        {
            disableCharacterParentIds.Clear();
        }
        
        /// <summary>選択できないキャラ</summary>
        public void SetDisableCharacterParentId(int id)
        {
            disableCharacterParentIds.Clear();
            disableCharacterParentIds.Add(id);
        }
        
        /// <summary>レアリティ制限キャラ</summary>
        public void SetLimitedCharacterIds(IList<long> list)
        {
            limitedCharacterIds.Clear();
            limitedCharacterIds.AddRange(list);
        }
        
        /// <summary>強化/能力解放できるキャラ</summary>
        public void SetEnableGrowthLiberationCharacterIds(IList<long> list)
        {
            enableGrowthLiberationCharacterIds.Clear();
            enableGrowthLiberationCharacterIds.AddRange(list);
        }
        
        /// <summary>解放できるキャラ</summary>
        public void SetEnablePieceToCharacterIds(IList<long> list)
        {
            enablePieceToCharacterIds.Clear();
            enablePieceToCharacterIds.AddRange(list);
        }
        
        /// <summary>編成できないキャラ</summary>
        public void SetDisableDeckSlotCharacterIds(IList<long> list)
        {
            disableDeckSlotCharacterIds.Clear();
            disableDeckSlotCharacterIds.AddRange(list);
        }
        
        public void SetFriendSettingId(long id)
        {
            friendSettingsCharacterId = id;
        }
        
        public void SetCurrentSelectCharacterId(long id)
        {
            currentSelectCharacterId = id;
        }

        
        public void ClearEnableLiberationParentIds()
        {
            enableGrowthLiberationCharacterIds.Clear();
        }
        
        
        
      
        
        public new void Refresh()
        {
            base.Refresh();
            // テキスト更新
            UpdateNoCharaText();
        }

        /// <summary> キャラカウントが0の時に表示するテキストの更新処理 </summary>
        protected virtual void UpdateNoCharaText()
        {
            if (noCharacterText != null) noCharacterText.SetActive(GetItemList().Count == 0);
        }
    }
    
    public abstract class CharacterScroll : CharacterScrollBase<CharacterScrollData>
    {
        protected void OnEnable()
        {
        }

        protected void OnDestroy()
        {
        }

        protected override void OnAwake()
        {
            base.OnAwake();
        }
    }
}