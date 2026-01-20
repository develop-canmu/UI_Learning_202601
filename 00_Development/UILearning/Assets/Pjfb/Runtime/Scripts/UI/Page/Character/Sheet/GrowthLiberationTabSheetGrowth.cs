using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CruFramework.Page;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.Character
{
    public class GrowthCostData
    {
        // 強化に必要なアイテムの累積一覧（Key:PointId,Value:累積数）
        public Dictionary<long, long> SumCost;
    }
    
    public class GrowthLiberationTabSheetGrowth : GrowthLiberationTabSheetBase
    {
        [SerializeField] private ScrollGrid itemScrollGrid;
        
        private Dictionary<long, List<EnhanceLevelPointMasterObject>> enhanceLevelPointCache;
        private Dictionary<long, StatusAdditionLevelMasterObject> growthStatusAdditionLevelCache;
        private Dictionary<long, StatusAdditionLevelMasterObject> liberationStatusAdditionLevelCache;
        private Dictionary<long, GrowthCostData> growthCostDictionary;
        protected override long currentLv => uChara.level;
        
        public override void Initialize(long id)
        {
            base.Initialize(id);
            mElementId = MChara.mEnhanceIdGrowth;
        }
        
        
        protected override void SetDictionary()
        {
            enhanceLevelPointCache = uChara.MEnhanceLevelPointList.GroupBy(x => x.level).ToDictionary(g => g.Key, g => g.ToList());
            
            growthStatusAdditionLevelCache = uChara.MStatusAdditionLevelMasterObjectList_Growth.ToDictionary(o => o.level);
            liberationStatusAdditionLevelCache = uChara.MStatusAdditionLevelMasterObjectList_Liberation.ToDictionary(o => o.level);
            SetCostDictionary();
        }

        private void SetCostDictionary()
        {
            growthCostDictionary = new Dictionary<long, GrowthCostData>();
            var sumCost = new Dictionary<long, long>();
            foreach (var enhanceLevelPointDic in enhanceLevelPointCache.Where(data => data.Key > currentLv))
            {
                foreach (var enhanceLevelPoint in enhanceLevelPointDic.Value)
                {
                    if (!sumCost.ContainsKey(enhanceLevelPoint.mPointId))
                    {
                        sumCost.Add(enhanceLevelPoint.mPointId, enhanceLevelPoint.value);
                    }
                    else
                    {
                        sumCost[enhanceLevelPoint.mPointId] +=  enhanceLevelPoint.value;
                    }
                }
                var costData = new GrowthCostData { SumCost = new Dictionary<long, long>(sumCost) };
                growthCostDictionary[enhanceLevelPointDic.Key] = costData;
            }
        }

        protected override void SetMaxLevel()
        {
            //キャラとサポートカードで同じマスターを使用するようなので共通処理にする
            //1. m_chara.maxLevel を取得
            //2. u_chara.level と m_chara.mStatusAdditionIdGrowth をもとに m_status_addition_level.maxLevelGrowth を取得
            //3. u_chara.newLiberationLevel と m_chara.mStatusAdditionIdLiberation をもとに m_status_addition_level.maxLevelGrowth を取得
            //4. 1～3の合計値が強化の最大レベルになる


            var mStatusAdditionalLevelGrowth = growthStatusAdditionLevelCache.GetValueOrDefault(uChara.level);
            var mStatusAdditionalLevelLiberation = liberationStatusAdditionLevelCache.GetValueOrDefault(uChara.newLiberationLevel);

            if (mStatusAdditionalLevelGrowth is null || mStatusAdditionalLevelLiberation is null)
            {
                Logger.LogError($"mStatusAdditionalLevelGrowth Lv.{uChara.level} or mStatusAdditionalLevelLiberation Lv.{uChara.newLiberationLevel} is missing");
                return;
            }    
                

            maxLevel = MChara.maxLevel + mStatusAdditionalLevelGrowth.maxLevelGrowth + mStatusAdditionalLevelLiberation.maxLevelGrowth;
        }
        
        protected override void SetRequiredItem()
        {
            if (IsMaxLevel)
            {
                itemScrollGrid.SetItems(new List<ItemIconGridItem.Data>());
                canLevelUp = CanLevelUp(afterLv);
                return;
            }
            
            if (!growthCostDictionary.ContainsKey(afterLv))
            {
                Debug.LogError($"Not found level {afterLv} growthCostDictionary data");
                return;
            }

            var growthCostData = growthCostDictionary[afterLv];
            var itemIconGridItemDataList = new List<ItemIconGridItem.Data>();
            foreach (var costData in growthCostData.SumCost)
            {
                long mPointId = costData.Key;
                long requiredValue = costData.Value;
                long possessionValue = UserDataManager.Instance.point.Find(mPointId)?.value ?? 0;
                itemIconGridItemDataList.Add(new ItemIconGridItem.Data(mPointId, requiredValue, possessionValue, true));
            }

            itemScrollGrid.SetItems(itemIconGridItemDataList);
            canLevelUp = CanLevelUp(afterLv);
        }
        
        /// <summary>
        /// 必要アイテムと所持アイテムを比較して強化できるかを確認する
        /// </summary>
        protected override bool CanLevelUp(long lv)
        {
            if (IsMaxLevel) return false;
            if (!enhanceLevelPointCache.ContainsKey(lv))
            {
                Debug.LogError($"Not found level {lv} EnhanceLevelPointMasterObject data");
                return false;
            }
            if (!growthCostDictionary.ContainsKey(lv)) return false;

            var growthCostData = growthCostDictionary[lv];
            foreach (var costData in growthCostData.SumCost)
            {
                long mPointId = costData.Key;
                long requiredValue = costData.Value;
                long possessionValue = UserDataManager.Instance.point.Find(mPointId)?.value ?? 0;
                if (possessionValue < requiredValue) return false;
            }
            
            return true;
        }

        
        /// <summary> 強化確認モーダルタイプの取得 </summary>
        private ModalType GetConfirmModalType()
        {
            switch (MChara.cardType)
            {
                // キャラとスペシャルサポートカートは共通の確認モーダル
                case CardType.Character:
                case CardType.SpecialSupportCharacter:
                    return ModalType.CharacterGrowthConfirm;
                
                // アドバイザースキルの表示が必要なのでモーダルを切り分け
                case CardType.Adviser :
                    return ModalType.AdviserGrowthConfirm;
                // それ以外は強化対象外
                default: throw new Exception("強化対象外のCardTypeです");
            }
        }
        
        public override async void OnClickConfirmButton()
        {
            var unlockCombination = CombinationManager.IsUnLockCombination();
            var prevCombinationCollection = unlockCombination
                ? CombinationManager.GetCanActiveProgressDataListByCharaId(MChara.id)
                : null;
            var confirmWindow = await CharacterGrowthConfirmModal.OpenAsync(
                GetConfirmModalType(),
                new CharacterGrowthConfirmModal.GrowthData(
                    userCharacterId: userCharacterId,
                    currentLv: currentLv,
                    afterLv: afterLv,
                    growthCostData: growthCostDictionary[afterLv],
                    practiceSkillDataList: CreateAcquisitionPracticeSkillDataList()),
                this.GetCancellationTokenOnDestroy());

            bool? response = (bool?)await confirmWindow.WaitCloseAsync();
            
            if (response ?? false)
            {
                OnPlayEffect?.Invoke(GrowthLiberationTabSheetType.Growth, MChara.id, currentLv, afterLv, null, prevCombinationCollection);
                OnLevelUp?.Invoke();
            }
        }

        public override void InitializeView()
        {
            SetCostDictionary();
            base.InitializeView();
        }

        private List<PracticeSkillInfo> CreateAcquisitionPracticeSkillDataList()
        {
            var currentPracticeSkillDataList =
                PracticeSkillUtility.GetCharacterPracticeSkill(uChara.charaId, currentLv);
            var allSkillDataList = PracticeSkillUtility.GetCharacterPracticeSkill(uChara.charaId);
            
            // スキルの最低解放レベル(ソートに使う)
            Dictionary<long, long> minSkillUnlockLevel = new Dictionary<long, long>();
            
            IOrderedEnumerable<PracticeSkillInfo> orderedList = allSkillDataList.OrderBy(v=>
            {
                switch (v.MasterType)
                {
                    case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                        return MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(v.MasterId).level;
                    default:
                        return MasterManager.Instance.charaTrainingStatusMaster.FindData(v.MasterId).level;
                }
                
            });

            foreach (var nextSkillInfo in orderedList)
            {
                switch (nextSkillInfo.MasterType)
                {
                    case PracticeSkillMasterType.TrainingPointStatusEffectChara:
                        var effectCharaMaster = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(nextSkillInfo.MasterId);
                        // キーがないなら追加する
                        if (minSkillUnlockLevel.ContainsKey(nextSkillInfo.TrainingStatusTypeDetailId) == false)
                        {
                            // スキルIdごとの最低解放レベルを登録する
                            minSkillUnlockLevel.Add( nextSkillInfo.TrainingStatusTypeDetailId, effectCharaMaster.level);
                        }
                        break;
                    default:
                        var trainingStatus = MasterManager.Instance.charaTrainingStatusMaster.FindData(nextSkillInfo.MasterId);
                
                        // キーがないなら追加する
                        if (minSkillUnlockLevel.ContainsKey(nextSkillInfo.TrainingStatusTypeDetailId) == false)
                        {
                            // スキルIdごとの最低解放レベルを登録する
                            minSkillUnlockLevel.Add(nextSkillInfo.TrainingStatusTypeDetailId, trainingStatus.level);
                        }
                        break;
                }

            }

            // 強化後のレベルで修得するスキルを取得
            var afterLvPracticeSkillDataList = PracticeSkillUtility.GetCharacterPracticeSkill(uChara.charaId, afterLv);
            // 現在取得しているスキルのIdリストを取得
            var currentPracticeSkillDetailIdList = currentPracticeSkillDataList.Select(x => x.TrainingStatusTypeDetailId);
            
            // 現在のスキルと強化後のスキルを比べて持ってないスキルIdのスキルを取得する
            return afterLvPracticeSkillDataList.
                Where(skill => !currentPracticeSkillDetailIdList.Contains(skill.TrainingStatusTypeDetailId)).
                // スキルの最低解放レベル順にソートする
                OrderBy(v=>minSkillUnlockLevel[v.TrainingStatusTypeDetailId]).
                ThenBy(v => v.MasterId).ToList();
        }
    }
}


