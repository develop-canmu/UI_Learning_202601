using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Encyclopedia;
using Pjfb.Master;
using Pjfb.Training;
using TMPro;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.Character
{ 
    public class SuccessCharaDetailModalParams : SwipeableDetailModalParams<CharacterVariableDetailData>
    {
        public readonly bool CanOpenCharacterEncyclopediaPage;
        
        public SuccessCharaDetailModalParams(SwipeableParams<CharacterVariableDetailData> swipeableParams, bool canOpenCharacterEncyclopediaPage, string titleStringKey = null) : base(swipeableParams, titleStringKey)
        {
            CanOpenCharacterEncyclopediaPage = canOpenCharacterEncyclopediaPage;
        }
    }
    public class SuccessCharaDetailModalWindow : SwipeableDetailModalWindow<CharacterVariableDetailData, SuccessCharaDetailModalParams>
    {
        [SerializeField] private CharacterSkillScrollGrid skillScroll;
        [SerializeField] private CharacterStatusValuesView statusValuesView;
        [SerializeField] private SuccessCharacterNameView nameView;
        
        [Header("Training Information")]
        [SerializeField] private GameObject encyclopediaButton;
        [SerializeField] private List<CharacterIcon> characterIconList;
        [SerializeField] private CharacterIcon friendIcon;
        [SerializeField] ScrollGrid specialSupportScrollGrid;
        [SerializeField] TextMeshProUGUI noSpecialSupportCardText;
        [SerializeField] ScrollGrid extraSupportCardScrollGrid;
        [SerializeField] TextMeshProUGUI noExtraSupportCardText;
        [SerializeField] SupportEquipmentIconItemParent SupportEquipmentIconItemParent;
        [SerializeField] TextMeshProUGUI noSupportEquipmentText;

        [SerializeField]
        private ScrollGrid adviserScrollGrid = null;
        [SerializeField]
        private TMP_Text noAdviserText = null;
        
        [Space(20)]
        [SerializeField] private SuccessCharacterDetailTabSheetManager tabSheetManager;
        [SerializeField] private TrainingScenarioNameView trainingScenarioNameView;
        [SerializeField] private CharacterLevelView characterLevelView;
        [SerializeField] private GameObject recommendCharaRoot;
        
        protected override string defaultTitleStringKey => "character.success_character_detail";
        
        private Action OnSetCharacter;

        protected override UniTask OnOpen(CancellationToken token)
        {
            skillScroll.Refresh();
            return base.OnOpen(token);
        }

        #region PrivateMethods
        protected override void Init()
        {
            recommendCharaRoot.SetActive(objectDetail.HasTrainingInfo());
            statusValuesView.SetCharacterVariable(objectDetail);
            skillScroll.SetItems(objectDetail.AbilityList);
            nameView.InitializeUI(objectDetail);
            encyclopediaButton.SetActive(modalParams.CanOpenCharacterEncyclopediaPage);

            var supportCharaList = objectDetail.SupportDetailJson?.Select(item => new CharacterDetailData(item.mCharaId, item.level, item.newLiberationLevel)).ToList();
            tabSheetManager.Initialize(objectDetail.OpenedCollections, supportCharaList, objectDetail.HasTrainingInfo());
            
            InitializeCharacterLevelView(objectDetail);
            InitializeTrainingInfo();
            InitializeTrainingScenarioView(objectDetail);
        }

        private void InitializeCharacterLevelView(CharacterVariableDetailData characterData)
        {
            characterLevelView.gameObject.SetActive(characterData.HasCharacterLevel());
            characterLevelView.SetValue(characterData.CharacterLevel);
        }
        
        private void InitializeTrainingInfo()
        {
            if (!objectDetail.HasTrainingInfo())
            {
                return;
            }
            
            int charaIndex = 0;
            int specialSupportCardIndex = 0;
            int extraSupportCardIndex = 0;
            int supportEquipmentIndex = 0;
            int detailIndex = 0;
                
            List<CharacterDetailData> characterDetailOrderList = new();
            List<CharacterDetailData> specialSupportCardDetailOrderList = new();
            List<SupportEquipmentDetailData> supportEquipmentDetailOrderList = new();
            List<CharacterDetailData> adviserDetailOrderList = new List<CharacterDetailData>();
            
            List<CharacterScrollData> specialSupportCharacterScrollDataList = new();
            List<CharacterScrollData> extraSupportCharacterScrollDataList = new();
            List<SupportEquipmentIconItemParent.SupportEquipmentIconItemData> equipmentDataList = new();
            List<CharacterScrollData> adviserScrollDataList = new List<CharacterScrollData>();    
            
            int supportIndex = 0;
                
            foreach (var detail in objectDetail.SupportDetailJson.OrderBy(chara => MasterManager.Instance.charaMaster.FindData(chara.mCharaId).isExtraSupport == false))
            {
                long id = detail.mCharaId;
                long level = detail.level;
                long newLiberationLevel = detail.newLiberationLevel;
                TrainingUtility.SupportCharacterType type = (TrainingUtility.SupportCharacterType)detail.supportType;
                bool isExtra = CharacterUtility.IsExtraCharacter(id);
                // サポートタイプ。0 => 育成キャラ自身、1 => 通常、2 => フレンド、3 => スペシャルサポート、4 => 追加サポートキャラ
                switch (type)
                {
                    case TrainingUtility.SupportCharacterType.TrainingChar:
                        break;
                    case TrainingUtility.SupportCharacterType.Normal:
                        characterIconList[charaIndex].SetIcon(id, level, newLiberationLevel);
                        characterIconList[charaIndex++].SwipeableParams = new SwipeableParams<CharacterDetailData>(characterDetailOrderList, supportIndex++);
                        characterDetailOrderList.Add(new CharacterDetailData(id, level, newLiberationLevel));
                        break;
                    case TrainingUtility.SupportCharacterType.Friend:
                        friendIcon.SetIcon(id, level, newLiberationLevel);
                        friendIcon.SwipeableParams = new SwipeableParams<CharacterDetailData>(characterDetailOrderList, supportIndex++);
                        characterDetailOrderList.Add(new CharacterDetailData(id, level, newLiberationLevel));
                        break;
                    case TrainingUtility.SupportCharacterType.Special:
                        switch ((CardType)detail.cardType)
                        {
                            case CardType.SpecialSupportCharacter:
                            {
                                CharacterScrollData iconData = new CharacterScrollData(id, level, newLiberationLevel, 0,
                                    new SwipeableParams<CharacterDetailData>(specialSupportCardDetailOrderList, detailIndex++));
                                if (isExtra)
                                {
                                    extraSupportCharacterScrollDataList.Add(iconData);
                                    extraSupportCardIndex++;
                                }
                                else
                                {
                                    specialSupportCharacterScrollDataList.Add(iconData);
                                    specialSupportCardIndex++;
                                }

                                specialSupportCardDetailOrderList.Add(new CharacterDetailData(id, level, newLiberationLevel));
                                break;
                            }
                            // アドバイザーはサポートタイプがスペシャルでくるので分ける
                            case CardType.Adviser:
                            {
                                SwipeableParams<CharacterDetailData> swipeParams = new SwipeableParams<CharacterDetailData>(adviserDetailOrderList, adviserDetailOrderList.Count);
                                CharacterScrollData adviserIconData = new CharacterScrollData(id, level, newLiberationLevel, 0, swipeParams, baseCharacterType: BaseCharacterType.Adviser);
                                adviserDetailOrderList.Add(new CharacterDetailData(id, level, newLiberationLevel));
                                adviserScrollDataList.Add(adviserIconData);
                                break;
                            }
                        }
                        break;
                    case TrainingUtility.SupportCharacterType.Equipment:
                        SupportEquipmentDetailData equipmentDetailData = new SupportEquipmentDetailData(id, level, detail.statusIdList);
                        supportEquipmentDetailOrderList.Add(equipmentDetailData);
                        SwipeableParams<SupportEquipmentDetailData> swipeableParams = new SwipeableParams<SupportEquipmentDetailData>(supportEquipmentDetailOrderList, supportEquipmentIndex);
                        SupportEquipmentIconItemParent.SupportEquipmentIconItemData scrollData = new SupportEquipmentIconItemParent.SupportEquipmentIconItemData(equipmentDetailData, swipeableParams);
                        equipmentDataList.Add(scrollData);
                        supportEquipmentIndex++;
                        break;
                    case TrainingUtility.SupportCharacterType.Add:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            // スペシャルサポカ
            specialSupportScrollGrid.SetItems(specialSupportCharacterScrollDataList);
            // エクストラサポカ
            extraSupportCardScrollGrid.SetItems(extraSupportCharacterScrollDataList);
            // サポート器具
            SupportEquipmentIconItemParent.CreateItem(equipmentDataList);
            // アドバイザー
            adviserScrollGrid.SetItems(adviserScrollDataList);  
            
            
            noExtraSupportCardText.gameObject.SetActive(extraSupportCardIndex == 0);
            noSpecialSupportCardText.gameObject.SetActive(specialSupportCardIndex == 0);
            noSupportEquipmentText.gameObject.SetActive(supportEquipmentIndex == 0);
            noAdviserText.gameObject.SetActive(adviserScrollDataList.Count == 0);
        }

        private void InitializeTrainingScenarioView(CharacterVariableDetailData characterData)
        {
            if (!characterData.HasTrainingScenario())
            {
                trainingScenarioNameView.SetEmpty();
                return;
            }
            
            var scenarioData = MasterManager.Instance.trainingScenarioMaster.FindData(characterData.TrainingScenarioId);
            trainingScenarioNameView.SetValue(scenarioData);
        }
        
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }

        public void OpenEncyclopedia()
        {
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(objectDetail.MCharaId);
            var charaParent = MasterManager.Instance.charaParentMaster.values.FirstOrDefault(x => x.parentMCharaId == mChara.parentMCharaId);
            if (charaParent is null)
            {
                Logger.LogError("MCharaParentのデータありません");
                return;
            }

            
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            
            Close(onCompleted:()=>
            {
                EncyclopediaPage.OpenPage(true,mChara.parentMCharaId);
            });
        }

        public void OnLongTapSupportCharacterIcon(CharacterIcon icon)
        {
            BaseCharacterDetailModal.Open(ModalType.BaseCharacterDetail,
                        new BaseCharaDetailModalParams(icon.SwipeableParams, false, false, titleStringKey: "character.detail_modal.support_character_info", objectDetail.TrainingScenarioId, icon.CanGrowth));
        }

        #endregion
    }
}
