using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    /// <summary>
    /// フィルタータイプとトグルをまとめたクラス
    /// </summary>
    [Serializable]
    public class FilterTypeToggle
    {
        public SelectFilterType filterType;
        public Toggle toggle;
    }
    
    /// <summary>
    /// コンビネーションスキルキャラクタースクロールのデータ
    /// </summary>
    [Serializable]
    public class CombinationSkillCharacterScrollData
    {
        [SerializeField] private CardType cardType;
        [SerializeField] private CombinationSkillCharacterScroll combinationSkillCharacterScroll;
        private bool isInitialized;
        
        //カードタイプ
        public CardType CardType => cardType;
        
        //　キャラクタースクロール
        public CombinationSkillCharacterScroll Scroll => combinationSkillCharacterScroll;
        
        public bool IsInitialized
        {
            get => isInitialized;
            set => isInitialized = value;
        }
    }
    
    public abstract class BaseCombinationSkillSortFilterModal<T> : SortFilterBaseModal<CombinationSkillSortData, CombinationSkillFilterData>
        where T : ICombinationSortable
    {
        /// <summary>
        /// コンビネーションスキルソートフィルターモーダル用のデータ
        /// </summary>
        public new class Data : SortFilterBaseModal<CombinationSkillSortData, CombinationSkillFilterData>.Data
        {
            /// <summary> 表示するキャラクターIDリスト </summary>
            public IReadOnlyList<long> DisplayCharacterIdList { get; }

            public Data(
                SortFilterSheetType sheetType,
                SortFilterUtility.SortFilterType sortFilterType,
                List<long> displayCharacterIdList)
                : base(sheetType, sortFilterType) 
            {
                DisplayCharacterIdList = displayCharacterIdList;
            }
        }

        [Header("絞り込み - 検索条件")]
        [SerializeField] private List<FilterTypeToggle> filterTypeToggles = new ();
        
        [SerializeField] protected List<CombinationSkillCharacterScrollData> combinationSkillCharacterScrollData;
        
        [SerializeField] private CombinationIconTabSheetManager combinationIconTabSheetManager;
        
        [SerializeField] private UIButton sortButton;
        [SerializeField] private UIButton filterButton;
        
        private CombinationSkillSelectionController combinationSkillSelectionController;
        
        // 各カードタイプごとのキャラクターIDコレクション
        private Dictionary<CardType, HashSet<long>> cardTypeIds = new();
        
        // キャラクターアイコン選択の初期状態
        private List<long> initialSelectedCharacterIds = new();

        // 選択しているIdリスト
        private List<long> selectIdList;
        
        // 表示するキャラクターIDリスト（キャッシュ）
        private IReadOnlyList<long> displayCharacterIdList;
        
        /// <summary>
        /// モーダルのプレオープン時の初期化処理
        /// </summary>
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            combinationSkillSelectionController =  new CombinationSkillSelectionController();
            
            // argsから直接IDリストを取得してキャッシュ
            if (args is Data data)
            {
                displayCharacterIdList = data.DisplayCharacterIdList;
                // 表示するキャラクターがいない場合、ソート・フィルターボタンを有効・無効化
                bool hasCharaIdList = displayCharacterIdList.Count != 0;
                sortButton.interactable = hasCharaIdList;
                filterButton.interactable = hasCharaIdList;
            }
            else
            {
                CruFramework.Logger.LogError($"OnPreOpen: argsをDataにキャストできませんでした, argsType={args?.GetType().Name}, ModalType={GetType().Name}");
            }
            
            // SheetManagerのOnOpenSheetイベントにハンドラーを登録
            combinationIconTabSheetManager.OnOpenSheet -= SwitchCardType;
            combinationIconTabSheetManager.OnOpenSheet += SwitchCardType;
            await base.OnPreOpen(args, token); 
            await combinationIconTabSheetManager.OpenSheetAsync(combinationIconTabSheetManager.FirstSheet, null);
            
            // 「適用」ボタンのグレーアウト制御を有効の場合、初期状態を保存し、変更検知のコールバックを登録
            if (EnableApplyButtonGrayOut)
            {
                // キャラクター選択の初期状態を保存
                initialSelectedCharacterIds = new List<long>(combinationSkillSelectionController.SelectedCharacterIdList);
                
                // 選択変更時のコールバック登録
                // キャラアイコンがクリックされるたびに適用ボタンの状態を更新
                combinationSkillSelectionController.OnSelectionChanged = () => 
                {
                    ApplyButton.interactable = IsChangedFromInitialState();
                };
            }
        }
        
        # region SetFilterToggleFromData
        /// <summary>
        /// データから絞り込みのToggleの状態を設定する
        /// </summary>
        protected override void SetFilterToggleFromData(CombinationSkillFilterData filterData)
        {
            SetFilterToggleByType(filterData.filterType);
            selectIdList = filterData.selectedCharaIdList.ToList();
        }
        
        /// <summary>
        /// フィルタータイプに基づいてトグルを設定する
        /// </summary>
        private void SetFilterToggleByType(SelectFilterType filterType)
        {
            foreach (var filterTypeToggle in filterTypeToggles)
            {
                filterTypeToggle.toggle.isOn = filterTypeToggle.filterType == filterType;
            }
        }
        
        /// <summary>
        /// 現在選択されているフィルタータイプを取得する
        /// </summary>
        private SelectFilterType GetSelectedFilterType()
        {
            if (filterTypeToggles == null)
            {
                CruFramework.Logger.LogError("FilterTypeToggles is null");
                return SelectFilterType.Off;
            }

            foreach (FilterTypeToggle filterTypeToggle in filterTypeToggles)
            {
                if (filterTypeToggle.toggle.isOn)
                {
                    return filterTypeToggle.filterType;
                }
            }

            return SelectFilterType.Off; 
        }
        
        /// <summary>
        /// UIの設定状況から絞り込みデータを作成
        /// </summary>
        protected override CombinationSkillFilterData CreateFilterData()
        {
            return new CombinationSkillFilterData
            {
                filterType = GetSelectedFilterType(),
                selectedCharaIdList = combinationSkillSelectionController.SelectedCharacterIdList,
            };
        }
        # endregion
        
        # region CreateCharacterIconScrolls
        /// <summary>
        /// コンビネーションスキルに対応するキャラクターアイコンを表示する
        /// </summary>
        /// <param name="cardType">表示するカードタイプ</param>
        private void SetDisplayCombinationIcons(CardType cardType)
        {
            CombinationSkillCharacterScrollData targetScrollData = combinationSkillCharacterScrollData.Find(x => x.CardType == cardType);
            
            // 既に処理済みの場合はスキップ
            if (targetScrollData.IsInitialized)
            {
                // 初期化後はView更新させる(選択番号の反映のため)
                targetScrollData.Scroll.Scroll.RefreshItemView();
                return;
            }
            
            // キャラクターIDを収集
            CollectCharacterIds();
            
            UpdateScrollDisplay(targetScrollData, cardType);
        }
        
        
        /// <summary>
        /// キャラクターIDを収集してカードタイプ別のHashSetに追加
        /// </summary>
        private void CollectCharacterIds()
        {
            if (displayCharacterIdList == null)
            {
                CruFramework.Logger.LogError($"CollectCharacterIds: displayCharacterIdListがnullです, ModalType={GetType().Name}");
                return;
            }
            
            foreach (long mCharaId in displayCharacterIdList)
            {
                AddCharacterIdToCardTypeIds(mCharaId);
            }
        }
        
        /// <summary>
        /// キャラクターIDをカードタイプ別のHashSetに追加
        /// </summary>
        private void AddCharacterIdToCardTypeIds(long mCharaId)
        {
            CharaMasterObject characterMaster = MasterManager.Instance.charaMaster.FindData(mCharaId);
            
            // カードタイプに対応するHashSetを取得または作成
            if (!cardTypeIds.TryGetValue(characterMaster.cardType, out HashSet<long> idSet))
            {
                idSet = new HashSet<long>();
                cardTypeIds[characterMaster.cardType] = idSet;
            }
            
            idSet.Add(mCharaId);
        }
        
        /// <summary>
        /// スクロールの表示を更新
        /// </summary>
        protected virtual void UpdateScrollDisplay(CombinationSkillCharacterScrollData scrollData, CardType cardType)
        {
            if (!cardTypeIds.TryGetValue(cardType, out var characterIds))
            {
                CruFramework.Logger.Log($"CardType:{cardType} のカードは0枚です");
                // 空のHashSetを作成
                characterIds = new HashSet<long>();
            }
      
            // 選択管理クラスにスクロールを登録し、スクロールに選択管理クラスを登録
            combinationSkillSelectionController.RegisterScroll(cardType, scrollData.Scroll);
            scrollData.Scroll.SetSelectionController(combinationSkillSelectionController);
            // 選択済みIdをセット
            combinationSkillSelectionController.SetSelectionIdList(selectIdList);
            scrollData.Scroll.SetSkillIconList(characterIds);
            scrollData.IsInitialized = true;
        }
        
        private CardType MapSheetTypeToCardType(CombinationSkillSheetType cardType)
        {
            return cardType switch
            {
                CombinationSkillSheetType.CharacterScrollGrid => CardType.Character,
                CombinationSkillSheetType.AdviserScrollGrid => CardType.Adviser,
                CombinationSkillSheetType.SpecialSupportCardScrollGrid => CardType.SpecialSupportCharacter,
                                
                _ => throw new ArgumentOutOfRangeException(nameof(cardType), cardType, null)
            };
        }
        
        # endregion
        
        /// <summary>
        /// カードタイプを切り替えて表示を更新
        /// </summary>
        private void SwitchCardType(CombinationSkillSheetType cardType)
        {
            SetDisplayCombinationIcons(MapSheetTypeToCardType(cardType));
        }
        
        # region 適用ボタンのグレーアウト制御。
        
        /// <summary>
        /// トグル変更イベントを登録する
        /// </summary>
        protected override void RegisterToggleChangeEvents()
        {
            base.RegisterToggleChangeEvents();
            
            if (filterTypeToggles != null)
            {
                foreach (FilterTypeToggle filterTypeToggle in filterTypeToggles)
                {
                    filterTypeToggle.toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
                    filterTypeToggle.toggle.onValueChanged.AddListener(OnToggleValueChanged);
                }
            }
        }
        
        /// <summary>
        /// フィルター設定が変更されているかチェックする
        /// </summary>
        protected override bool IsFilterChanged()
        {
            if (initialFilterData == null) return false;
            
            // 現在選択されているフィルタータイプを取得
            SelectFilterType currentFilterType = GetSelectedFilterType();
            
            return initialFilterData.filterType != currentFilterType;
        }
        
        /// <summary>
        /// 選択されているキャラクターアイコンに変更があるかチェックする
        /// </summary>
        protected override bool HasAdditionalChanges()
        {
            if (initialSelectedCharacterIds == null) return false;
            
            // 現在選択されているキャラIDを取得
            List<long> currentSelectedCharacterIds = combinationSkillSelectionController.SelectedCharacterIdList;
            
            // 要素数が異なる場合
            if (initialSelectedCharacterIds.Count != currentSelectedCharacterIds.Count) return true;
            
            // 要素の内容が異なる場合
            for (int i = 0; i < initialSelectedCharacterIds.Count; i++)
            {
                if (initialSelectedCharacterIds[i] != currentSelectedCharacterIds[i]) return true;
            }
            
            return false;
        }
        
        # endregion
        
    }
}