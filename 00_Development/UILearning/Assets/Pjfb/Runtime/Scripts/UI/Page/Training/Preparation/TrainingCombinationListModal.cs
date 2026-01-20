
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.Training
{
    public class TrainingCombinationListModal : ModalWindow
    {
        
        public class Arguments
        {
            private TrainingCharacterData[] characterDatas = null;
            /// <summary>キャラデータ</summary>
            public  TrainingCharacterData[] CharacterDatas{get{return characterDatas;}}
            
            private DateTime startAt = DateTime.MaxValue;
            /// <summary>スキル参照の時間</summary>
            public DateTime StartAt{get{return startAt;}}
            
            private bool canGrowth = false;
            /// <summary>強化可能</summary>
            public bool CanGrowth{get{return canGrowth;}}
            
            private bool canOpenDetail = false;
            /// <summary>詳細開ける</summary>
            public bool CanOpenDetail{get{return canOpenDetail;}}

            private Action onCloseAction = null;
            public Action OnCloseAction{get{return onCloseAction;}}
            
            public Arguments(TrainingCharacterData[] characterDatas, bool canOpenDetail, bool canGrowth, string startAtString, Action onCloseAction = null)
            {
                this.canGrowth = canGrowth;
                this.canOpenDetail = canOpenDetail;
                this.characterDatas = characterDatas;
                // 時間
                if(string.IsNullOrEmpty( startAtString ) == false)
                {
                    startAt = AppTime.Parse(startAtString);
                }
                this.onCloseAction = onCloseAction;
            }
        }
    
        [SerializeField]
        private TrainingCombinationListModalSheetManager sheetManager = null;
    
        [SerializeField]
        private TrainingCombinationListModalCategorySheetManager categorySheetManager = null;
        
        [SerializeField]
        private CombinationTrainingScrollDynamic trainingScrollActivated = null;
        [SerializeField]
        private CombinationTrainingScrollDynamic trainingScrollAll = null;
        [SerializeField]
        private CombinationCollectionScrollDynamic collectionScrollActivated = null;
        [SerializeField]
        private CombinationCollectionScrollDynamic collectionScrollAll = null;
        
        [SerializeField]
        private ScrollGrid totalSkillScroll = null;
        [SerializeField]
        private GameObject noActivatingCombinationText_total = null;
        
        [SerializeField]
        private TMPro.TMP_Text  activeListText = null;
        [SerializeField]
        private TMPro.TMP_Text  trainingSkillText = null;
        [SerializeField]
        private TMPro.TMP_Text  collectionSkillText = null;
        
        [SerializeField]
        private UIButton totalListButton = null;
        [SerializeField]
        private UIButton groupListButton = null;
        
        [SerializeField]
        private GameObject buttonBackground = null;
        
        [SerializeField]
        private CharacterIcon[] characterIcons = null;
        
        private bool showTotalList = false;
        
        private Arguments arguments = null;

        private Dictionary<long, long> characterTupleDictionary = new();
        private Dictionary<long, UserDataChara> characterDictionary = new();
        private List<PracticeSkillInfo> trainingPracticeSkillDataList = null;
        private List<PracticeSkillInfo> collectionPracticeSkillDataList = null;
    
        // スキル数を取得
        private List<CombinationManager.CombinationTraining> activatingTrainingSkillList = null;

        private bool hasInitializedTrainingCombinaionActivated;
        private bool hasInitializedTrainingCombinaionAll;
        private bool hasInitializedCollectionCombinaionActivated;
        private bool hasInitializedCollectionCombinaionAll;
    
        private void HideSheets()
        {
            trainingScrollActivated.gameObject.SetActive(false);
            trainingScrollAll.gameObject.SetActive(false);
            collectionScrollActivated.gameObject.SetActive(false);
            collectionScrollAll.gameObject.SetActive(false);
            totalSkillScroll.gameObject.SetActive(false);
        }
    
        /// <summary>UGUI</summary>
        public void OnTotalListButton()
        {
            showTotalList = true;
            UpdateList();
        }
        
        /// <summary>UGUI</summary>
        public void OnGroupListButton()
        {
            showTotalList = false;
            UpdateList();
        }

        /// <summary>UGUI</summary>
        public void OnActiveListTab()
        {
            sheetManager.OpenSheet(TrainingCombinationListModalSheetType.Active, null);
            UpdateList();
        }
        
        /// <summary>UGUI</summary>
        public void OnAllListTab()
        {
            sheetManager.OpenSheet(TrainingCombinationListModalSheetType.All, null);
            UpdateList();
        }
        
        /// <summary>UGUI</summary>
        public void OnTrainingTab()
        {
            categorySheetManager.OpenSheet(TrainingCombinationListModalCategorySheetType.Training, null);
            UpdateList();
        }
        
        /// <summary>UGUI</summary>
        public void OnCollectionTab()
        {
            categorySheetManager.OpenSheet(TrainingCombinationListModalCategorySheetType.Collection, null);
            UpdateList();
        }

        public void OnClickClose()
        {
            Close();
            arguments.OnCloseAction?.Invoke();
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            arguments = (Arguments)args;

            hasInitializedTrainingCombinaionActivated = false;
            hasInitializedTrainingCombinaionAll = false;
            hasInitializedCollectionCombinaionActivated = false;
            hasInitializedCollectionCombinaionAll = false;
            
            characterTupleDictionary.Clear();
            foreach(TrainingCharacterData c in arguments.CharacterDatas)
            {
                characterTupleDictionary[c.MCharId] = c.Lv;
                characterDictionary[c.MCharId] = new UserDataChara(new CharaV2Base
                    { mCharaId = c.MCharId, level = c.Lv, newLiberationLevel = c.LiberationId });
            }
            
            // 一旦表示をきる
            HideSheets();
            
            // 強化ボタン
            foreach(CharacterIcon icon in characterIcons)
            {
                icon.CanGrowth = arguments.CanGrowth;
                icon.OpenDetailModal = arguments.CanOpenDetail;
            }

            groupListButton.gameObject.SetActive(false);
            totalListButton.gameObject.SetActive(true);
            // 発動スキル
            activatingTrainingSkillList = CombinationManager.GetActivatingCombinationTrainingListBefore(characterTupleDictionary, arguments.StartAt);
            CombinationStatusTrainingBase activeCollectionSkillList = await CombinationManager.GetCombinationCollectionTrainingBuffAPI();
            // 発動中のトレーニングスキルのみクライアントで計算するため、mCharaTrainingComboBuffStatusIdを取得する
            var activatingTrainingMaxLevelMComboBuffStatusIdList = new List<long>();
            foreach (var combinationTraining in activatingTrainingSkillList)
            {
                // 発動しているトレーニングスキルから発動しているMCharaTrainingComboBuffStatusIdListを取得する
                var activatingMCharaTrainingComboBuffStatusIdList =
                    combinationTraining.GetActivatingMCharaTrainingComboBuffStatusIdList(characterTupleDictionary);
                // 発動しているMCharaTrainingComboBuffStatusIdListから要求されているレベルが高い方を保持する
                long currentMaxLevelMCharaTrainingComboBuffStatusId = 0;
                long requireLevel = 0;
                foreach (var mCharaTrainingComboBuffStatusId in activatingMCharaTrainingComboBuffStatusIdList)
                {
                    var mCharaTrainingComboBuffStatus =
                        MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(
                            mCharaTrainingComboBuffStatusId);
                    if(mCharaTrainingComboBuffStatus == null || mCharaTrainingComboBuffStatus.requireLevel < requireLevel) continue;
                    currentMaxLevelMCharaTrainingComboBuffStatusId = mCharaTrainingComboBuffStatusId;
                    requireLevel = mCharaTrainingComboBuffStatus.requireLevel;
                }
                activatingTrainingMaxLevelMComboBuffStatusIdList.Add(currentMaxLevelMCharaTrainingComboBuffStatusId);
            }
            // 合計スキル
            trainingPracticeSkillDataList = PracticeSkillUtility.GetComboBuffTotalPracticeSkill(activatingTrainingMaxLevelMComboBuffStatusIdList.ToArray());
            collectionPracticeSkillDataList = PracticeSkillUtility.GetCombinationStatusTrainingPracticeSkill(activeCollectionSkillList);
            UpdateList();
            await base.OnPreOpen(args, token);
        }
        
        
        private void SetActiveListTabText(int activeSkillCount)
        {
            if(activeSkillCount < 0)
            {
                activeListText.text = StringValueAssetLoader.Instance["training.combination.modal.active_list2"];
            }
            else
            {
                activeListText.text = string.Format(StringValueAssetLoader.Instance["training.combination.modal.active_list1"], activeSkillCount);
            }
        }
        
        private void SetTrainingActiveListTabText(int activeSkillCount)
        {
            if(activeSkillCount < 0)
            {
                trainingSkillText.text = StringValueAssetLoader.Instance["training.combination.modal.training_skill2"];
            }
            else
            {
                trainingSkillText.text = string.Format(StringValueAssetLoader.Instance["training.combination.modal.training_skill1"], activeSkillCount);
            }
        }
        
        private void SetCollectionActiveListTabText(int activeSkillCount)
        {
            if(activeSkillCount < 0)
            {
                collectionSkillText.text = StringValueAssetLoader.Instance["training.combination.modal.collection_skill2"];
            }
            else
            {
                collectionSkillText.text = string.Format(StringValueAssetLoader.Instance["training.combination.modal.collection_skill1"], activeSkillCount);
            }
        }
        private void UpdateList(bool refreshScroller = false)
        {
            // タブのテキスト更新
            
            // 合計値表示
            if(showTotalList)
            {
                SetActiveListTabText(-1);
                SetTrainingActiveListTabText(-1);
                SetCollectionActiveListTabText(-1);
            }
            else
            {
                // テキストの更新
                SetActiveListTabText(activatingTrainingSkillList.Count);
                
                switch(sheetManager.CurrentSheetType)
                {
                    // すべて表示の場合は個数表示なし
                    case TrainingCombinationListModalSheetType.All:
                    {
                        SetTrainingActiveListTabText(-1);
                        SetCollectionActiveListTabText(-1);
                        break;
                    }
                    
                    // 発動数を表示
                    case TrainingCombinationListModalSheetType.Active:
                    {
                        SetTrainingActiveListTabText(activatingTrainingSkillList.Count);
                        SetCollectionActiveListTabText(-1);
                        break;
                    }
                }
            }

        
            // 選択しているシートで表示を切り替える
            switch(categorySheetManager.CurrentSheetType)
            {
                // トレーニングスキル
                case TrainingCombinationListModalCategorySheetType.Training:
                {
                    // 表示切り替え
                    HideSheets();
                        

                    // 表示するスキルの範囲
                    switch(sheetManager.CurrentSheetType)
                    {
                        // 発動中のスキル
                        case TrainingCombinationListModalSheetType.Active:
                        {
                            if(showTotalList)
                            {
                                // ボタン表示
                                groupListButton.gameObject.SetActive(true);
                                totalListButton.gameObject.SetActive(false);
                                // スクロールに表示
                                totalSkillScroll.gameObject.SetActive(true);
                                noActivatingCombinationText_total.SetActive(trainingPracticeSkillDataList.Count == 0);
                                totalSkillScroll.SetItems(trainingPracticeSkillDataList);
                            }
                            else
                            {
                                // ボタン表示
                                groupListButton.gameObject.SetActive(false);
                                totalListButton.gameObject.SetActive(true);
                                // スクロールに表示
                                trainingScrollActivated.gameObject.SetActive(true);
                                noActivatingCombinationText_total.SetActive(false);
                                if(refreshScroller || !hasInitializedTrainingCombinaionActivated)
                                {
                                    hasInitializedTrainingCombinaionActivated = true;
                                    trainingScrollActivated.Initialize(characterTupleDictionary, characterDictionary, arguments.StartAt);
                                }
                            }
                            
                            // ボタンの背景を表示
                            buttonBackground.SetActive(true);

                            break;
                        }
                        // すべてのスキル
                        case TrainingCombinationListModalSheetType.All:
                        {
                            // ボタン表示
                            groupListButton.gameObject.SetActive(false);
                            totalListButton.gameObject.SetActive(false);
                            // ボタンの背景を非表示
                            buttonBackground.SetActive(false);
                            // スクロールに表示
                            trainingScrollActivated.gameObject.SetActive(false);
                            trainingScrollAll.gameObject.SetActive(true);
                            noActivatingCombinationText_total.SetActive(false);
                            if(refreshScroller || !hasInitializedTrainingCombinaionAll)
                            {
                                hasInitializedTrainingCombinaionAll = true;
                                trainingScrollAll.Initialize();
                            }
                            
                            break;
                        }
                    }
                    
                    break;
                }
                
                // コレクションスキル
                case TrainingCombinationListModalCategorySheetType.Collection:
                {
                    // 表示切り替え
                    HideSheets();

                    // 表示するスキルの範囲
                    switch(sheetManager.CurrentSheetType)
                    {
                        // 発動中のスキル
                        case TrainingCombinationListModalSheetType.Active:
                        {
                            if(showTotalList)
                            {
                                // ボタン表示
                                totalListButton.gameObject.SetActive(false);
                                groupListButton.gameObject.SetActive(true);
                                // スクロールに表示
                                totalSkillScroll.gameObject.SetActive(true);
                                noActivatingCombinationText_total.SetActive(collectionPracticeSkillDataList.Count == 0);
                                totalSkillScroll.SetItems(collectionPracticeSkillDataList);
                            }
                            else
                            {
                                totalListButton.gameObject.SetActive(true);
                                groupListButton.gameObject.SetActive(false);
                                // スクロールに表示
                                collectionScrollActivated.gameObject.SetActive(true);
                                noActivatingCombinationText_total.SetActive(false);

                                if (refreshScroller || !hasInitializedCollectionCombinaionActivated)
                                {
                                    hasInitializedCollectionCombinaionActivated = true;
                                    collectionScrollActivated.InitializeActivating();    
                                }
                                
                            }
                            
                            // ボタンの背景を表示
                            buttonBackground.SetActive(true);
                            
                            break;
                        }
                        // すべてのスキル
                        case TrainingCombinationListModalSheetType.All:
                        {
                            // ボタン表示
                            groupListButton.gameObject.SetActive(false);
                            totalListButton.gameObject.SetActive(false);
                            // ボタンの背景を非表示
                            buttonBackground.SetActive(false);
                            
                            // スクロールに表示
                            collectionScrollAll.gameObject.SetActive(true);
                            noActivatingCombinationText_total.SetActive(false);
                            if (refreshScroller || !hasInitializedCollectionCombinaionAll)
                            {
                                hasInitializedCollectionCombinaionAll = true;
                                collectionScrollAll.InitializeAll((id)=>
                                {
                                    // スキル数を取得
                                    activatingTrainingSkillList = CombinationManager.GetActivatingCombinationTrainingListBefore(characterTupleDictionary, arguments.StartAt);
                                    // 発動スキルを更新
                                    collectionPracticeSkillDataList = PracticeSkillUtility.GetCombinationStatusTrainingPracticeSkill(CombinationManager.CombinationCollectionTrainingBuffCache);
                                    UpdateList(false);
                                });
                            }
                            break;
                        }
                    }
                    
                    break;
                }
            }
        }

    }
}