using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using Pjfb.UserData;
using UnityEngine.UI;


namespace Pjfb.Character
{
    public class BaseCharaDetailModalParams : SwipeableDetailModalParams<CharacterDetailData>
    {
        public readonly bool CanLiberation;
        public readonly bool CanGrowth;
        public readonly long TrainingScenarioId;
        public readonly bool CanOpenCharacterEncyclopediaPage;
        
        public BaseCharaDetailModalParams(SwipeableParams<CharacterDetailData> swipeableParams, bool canOpenCharacterEncyclopediaPage = true, bool canLiberation = false, string titleStringKey = null, long trainingScenarioId = -1, bool canGrowth = false) : base(swipeableParams, titleStringKey)
        {
            CanGrowth = canGrowth;
            CanLiberation = canLiberation;
            TrainingScenarioId = trainingScenarioId;
            CanOpenCharacterEncyclopediaPage = canOpenCharacterEncyclopediaPage;
        }
    }

    /// <summary> CharacterDetailTabSheetType用の詳細モーダル(SheetTypeごとにクラス分け) </summary>
    public abstract class CharacterDetailModal : CharacterDetailModalBase
    {
        [SerializeField] private CharacterDetailTabSheetManager tabSheetManager;

        // それぞれのシート毎に初期化されたかのフラグ
        private Dictionary<CharacterDetailTabSheetType, bool> isInitializeListOnSheet = new Dictionary<CharacterDetailTabSheetType, bool>();
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // リストのクリア
            isInitializeListOnSheet.Clear();
            // 初期化状況のセット
            foreach (CharacterDetailTabSheetType sheetType in Enum.GetValues(typeof(CharacterDetailTabSheetType)))
            {
                isInitializeListOnSheet.Add(sheetType, false);
            }
            
            tabSheetManager.OnOpenSheet -= InitializeScroller;
            tabSheetManager.OnOpenSheet += InitializeScroller;
            return base.OnPreOpen(args, token);
        }
        
        protected override void OnOpened()
        {
            base.OnOpened();
            InitializeScroller(tabSheetManager.CurrentSheetType);
        }
        
        public override void NextDetail()
        {
            base.NextDetail();
            ResetInitializeOnSheet();
            InitializeScroller(tabSheetManager.CurrentSheetType);
        }

        public override void PrevDetail()
        {
            base.PrevDetail();
            ResetInitializeOnSheet();
            InitializeScroller(tabSheetManager.CurrentSheetType);
        }
        
         public void InitializeScroller(CharacterDetailTabSheetType type)
        {
            // すでに初期化済みならViewのセット処理は行わない
            if (isInitializeListOnSheet[type])
            {
                return;
            }
            
            // 自動レイアウトを使用しているのでContentのサイズを再計算してからアイテムをセットする
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            
            switch (type)
            {
                case CharacterDetailTabSheetType.CharaTrainingStatus:
                    SetPracticeAbilityList();
                    break;
                case CharacterDetailTabSheetType.TrainingCard:
                    SetTrainingPracticeCardList();
                    break;
                case CharacterDetailTabSheetType.TrainingEvent:
                    SetTrainingEventList();
                    break;
                // サポートイベントはサポカにはタブのボタンはない
                case CharacterDetailTabSheetType.SupportEvent:
                    SetSupportEventList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // 初期化済みにセット
            isInitializeListOnSheet[type] = true;
        }
         
        /// <summary> シート毎の初期化状況を一括でリセット </summary>
        private void ResetInitializeOnSheet()
        {
            foreach (CharacterDetailTabSheetType sheetType in Enum.GetValues(typeof(CharacterDetailTabSheetType)))
            {
                isInitializeListOnSheet[sheetType] = false;
            }
        }
    }
    
    
    public abstract class CharacterDetailModalBase: SwipeableDetailModalWindow<CharacterDetailData, BaseCharaDetailModalParams>
    {
        [SerializeField] protected RectTransform content;
        [SerializeField] private ScrollGrid practiceAbilityScroll;
        [SerializeField] private ScrollGrid practiceMenuCardScroll;
        [SerializeField] private CharacterEventSkillScrollView trainingEventScroll;
        [SerializeField] private CharacterEventSkillScrollView supportEventScroll;
        [SerializeField] private UIButton descriptionButton;
        
        protected CharaMasterObject MChara => objectDetail.MChara;

        /// <summary> カードタイプ毎のモーダルを開く </summary>
        public static void OpenCharacterDetailModal(CardType cardType, BaseCharaDetailModalParams modalParams)
        {
            // デフォルトはベースキャラモーダルにしとく
            ModalType modalType = ModalType.BaseCharacterDetail;
                
            switch (cardType)
            {
                case CardType.Character:
                {
                    modalType = ModalType.BaseCharacterDetail;
                    break;
                }
                case CardType.SpecialSupportCharacter:
                {
                    modalType = ModalType.SpecialSupportCardDetail;
                    break;
                }
                case CardType.Adviser:
                {
                    modalType = ModalType.AdviserDetail;
                    break;
                }
                // 未定義カードタイプならエラー出しとく
                default:
                {
                    CruFramework.Logger.LogError($"Not Find CharacterDetail ModalType in CardType : {cardType}");
                    break;
                }
            }

            Open(modalType, modalParams);
        }
        
        /// <summary> 練習能力リストのセット処理 </summary>
        protected void SetPracticeAbilityList()
        {
            // 自動レイアウトを使用しているのでContentのサイズを再計算してからアイテムをセットする
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            practiceAbilityScroll.SetItems(CreatePracticeSkillDataList(objectDetail.Lv, objectDetail.LiberationLevel));
        }

        /// <summary> 練習カードリストのセット処理 </summary>
        protected void SetTrainingPracticeCardList()
        {
            // 自動レイアウトを使用しているのでContentのサイズを再計算してからアイテムをセットする
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            List<PracticeCardScrollItem.Arguments> cardArgsList = new();
            // トレーニング中の場合は、サーバー側で保存されたカードIDを使用
            if (AppManager.Instance.UIManager.PageManager.CurrentPageObject is TrainingMain training)
            {
                // TrainingMainArgumentsから取得
                TrainingMainArguments mainArguments = training.CurrentTrainingData;
                
                // 対象キャラのTrainingSupportを取得
                TrainingSupport targetSupport = null;
                foreach (TrainingSupport support in mainArguments.Pending.supportDetailList)
                {
                    if (support.mCharaId == MChara.id)
                    {
                        targetSupport = support;
                        break;
                    }
                }

                if (targetSupport == null)
                {
                    CruFramework.Logger.LogError($"TrainingScenarioId : {modalParams.TrainingScenarioId}, MCharaId : {MChara.id}のTrainingSupportが見つかりません。");
                    return;
                }

                // サーバー側で保存されたサポートキャラの練習カードIDを使用
                foreach (long cardCharaId in targetSupport.mTrainingCardCharaIdList)
                {
                    TrainingCardCharaMasterObject cardCharaMaster = MasterManager.Instance.trainingCardCharaMaster.FindData(cardCharaId);
                    cardArgsList.Add(
                        new PracticeCardScrollItem.Arguments(
                            cardCharaMaster.id,
                            MChara.id,
                            cardCharaMaster.mTrainingCardId,
                            objectDetail.Lv,
                            PracticeCardView.DisplayEnhanceUIFlags.Label | PracticeCardView.DisplayEnhanceUIFlags.DetailLabel
                            ));
                }
            }
            else
            {
                // トレーニング中でない場合は、現在のレベルから取得
                List<TrainingCardCharaMasterObject> cardCharaMasters = MasterManager.Instance.trainingCardCharaMaster.GetDisplayCardListForCharaLevel(MChara.id, objectDetail.Lv);
                foreach (TrainingCardCharaMasterObject targetMaster in cardCharaMasters)
                {
                    cardArgsList.Add(
                        new PracticeCardScrollItem.Arguments(
                            targetMaster.id,
                            MChara.id,
                            targetMaster.mTrainingCardId,
                            objectDetail.Lv,
                            PracticeCardView.DisplayEnhanceUIFlags.Label | PracticeCardView.DisplayEnhanceUIFlags.NextLevel |
                            PracticeCardView.DisplayEnhanceUIFlags.DetailLabel | PracticeCardView.DisplayEnhanceUIFlags.DetailEnhanceUI));
                }
            }

            practiceMenuCardScroll.SetItems(cardArgsList.ToArray());
        }

        /// <summary> トレーニングイベントリストのセット処理 </summary>
        protected void SetTrainingEventList()
        {
            // 自動レイアウトを使用しているのでContentのサイズを再計算してからアイテムをセットする
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            trainingEventScroll.SetCharacter(MChara.id, objectDetail.Lv, CharacterEventSkillScrollView.CharacterType.All);
        }

        /// <summary> サポートイベント処理のセット </summary>
        protected void SetSupportEventList()
        {
            // 自動レイアウトを使用しているのでContentのサイズを再計算してからアイテムをセットする
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            supportEventScroll.SetCharacter(objectDetail.MCharaId, objectDetail.Lv, modalParams.TrainingScenarioId, CharacterEventSkillScrollView.CharacterType.Support);
        }
        
        private List<PracticeSkillViewMiniGridItem.Info> CreatePracticeSkillDataList(long currentLv, long liberationLv)
        {
            var practiceSkillGridDataList = new List<PracticeSkillViewMiniGridItem.Info>();
            var acquireAndUnAcquireSkill = PracticeSkillUtility.GetCharacterPracticeSkillAcquireAndUnAcquire(MChara.id, currentLv);
            
            foreach (var skill in acquireAndUnAcquireSkill)
            {
                bool isLock = currentLv < skill.GetLevel();
                practiceSkillGridDataList.Add(new PracticeSkillViewMiniGridItem.Info(skill, false, isLock));
            }
            
            return practiceSkillGridDataList;
        }
        
        protected override void Init()
        {
            // 特殊能力がない場合はボタンを表示しない
            descriptionButton.gameObject.SetActive(!string.IsNullOrEmpty(MChara.description) && descriptionButton != null);
        }

        public void OnClickDescriptionButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CharacterCardDescription,objectDetail.MChara);
        }
    }
}