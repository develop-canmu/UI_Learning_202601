using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.SystemUnlock;
using Pjfb.UserData;

namespace Pjfb
{
    // トレーニング編成強化バフ情報
    public class TrainingDeckEnhanceInfo
    {
        // m_deck_enhance_training_statusのId
        private long masterId;
        public long MasterId{get => masterId;}
        
        // どのデッキ種別に対して効果の対象になるか
        private DeckType deckType;
        public DeckType DeckType{get => deckType;}
        
        // デッキ番号
        private long deckSlotIndex;
        public long DeckSlotIndex{get => deckSlotIndex;}

        // バフの発生するレベル
        private long level;
        public long Level{get => level;}
        
        //trainingStatusTypeDetailのId
        private long trainingStatusTypeDetailId;
        public long TrainingStatusTypeDetailId{get => trainingStatusTypeDetailId;}

        //バフの表示カテゴリId
        private long buffCategoryId;
        public long BuffCategoryId {get => buffCategoryId;}

        // バフの表示アイコンId
        private long buffIconId;
        public long BuffIconId {get => buffIconId;}
        
        // 効果値(実数)
        private long realValue = 0;
        public long RealValue {get => realValue;}
        
        // 効果値実数で新たにバフを獲得しているか
        private bool isNewAcquisitionReal = false;
        public bool IsNewAcquisitionReal{get => isNewAcquisitionReal;}
        
        // 効果値(%)
        private long percentValue = 0;
        public long PercentValue{get => percentValue;}
        
        // 効果値%で新たにバフを獲得しているか
        private bool isNewAcquisitionPercent = false;
        public bool IsNewAcquisitionPercent{get => isNewAcquisitionPercent;}

        // 表示順
        private long priority;
        public long Priority{get => priority;}
        
        public TrainingDeckEnhanceInfo(long masterId, DeckType deckType, long deckSlotIndex, long targetType, long level, long trainingStatusTypeDetailId, long value)
        {
            this.masterId = masterId;
            this.deckType = deckType;
            this.deckSlotIndex = deckSlotIndex;
            this.level = level;
            TrainingStatusTypeDetailMasterObject detailMaster = MasterManager.Instance.trainingStatusTypeDetailMaster.FindType(trainingStatusTypeDetailId);
            this.trainingStatusTypeDetailId = detailMaster.id;
            TrainingStatusTypeDetailCategoryMasterObject detailCategoryMaster = MasterManager.Instance.trainingStatusTypeDetailCategoryMaster.FindCategory(detailMaster.mTrainingStatusTypeDetailCategoryId, targetType);
            buffCategoryId = detailCategoryMaster.id;
            buffIconId = detailMaster.buffIconId;
            priority = detailCategoryMaster.displayPriority;
            
            // 効果値タイプによって入れる変数を変える
            switch (detailMaster.valueType)
            {
                case PracticeSkillUtility.ValueTypeValue:
                    realValue = value;
                    break;
                case PracticeSkillUtility.ValueTypePercent:
                    percentValue = value;
                    break;
                default:
                    CruFramework.Logger.LogError("Not Find ValueType");
                    break;
            }
        }

        // Totalバフを出す際に新たにそのレベルのデータを作成する
        public TrainingDeckEnhanceInfo(TrainingDeckEnhanceInfo enhanceInfo, long level, bool isNewAcquisition)
        {
            this.masterId = enhanceInfo.MasterId;
            this.deckType = enhanceInfo.DeckType;
            this.deckSlotIndex = enhanceInfo.DeckSlotIndex;
            this.level = level;
            this.trainingStatusTypeDetailId = enhanceInfo.trainingStatusTypeDetailId;
            this.buffCategoryId = enhanceInfo.buffCategoryId;
            this.buffIconId = enhanceInfo.BuffIconId;
            this.realValue = enhanceInfo.RealValue;
            this.percentValue = enhanceInfo.percentValue;
            this.priority = enhanceInfo.priority;
            
            // 新規獲得の表示を行うなら
            if (isNewAcquisition)
            {
                if (realValue > 0) isNewAcquisitionReal = true;
                if (percentValue > 0) isNewAcquisitionPercent = true;
            }
        }
        
        // 効果値の加算
        public void AddValue(TrainingDeckEnhanceInfo enhanceInfo)
        {
            realValue += enhanceInfo.realValue;
            percentValue += enhanceInfo.percentValue;
            
            // 新しく追加したデータのフラグが立っているならフラグを立てる
            isNewAcquisitionReal = isNewAcquisitionReal || enhanceInfo.isNewAcquisitionReal;
            isNewAcquisitionPercent = isNewAcquisitionPercent || enhanceInfo.isNewAcquisitionPercent;
        }
        
        //// <summary> バフの効果値単位タイプを取得 </summary>
        public long GetValueType()
        {
            return MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(trainingStatusTypeDetailId).valueType;
        }
    }

    public static class TrainingDeckEnhanceUtility
    {
        // すべての枠に対して効果の対象になるIndex番号
        private const int AllEnhanceSlotIndex = -1;

        // バフの効果対象がスキルのバフアイコンId 
        public const int EnhanceTypeSkillBuffIconId = 0;

        public static readonly string NormalColorId = "default";
        public static readonly string PositiveColorId = "highlight.orange";
        
        // トレーニング編成強化のロックId
        public const int TrainingDeckEnhanceLockId = (int)SystemUnlockDataManager.SystemUnlockNumber.TrainingDeckEnhance;
        public static long playerEnhanceId => PlayerEnhanceManager.GetPlayerEnhance(PlayerEnhanceType.TrainingDeckEnhance).mPlayerEnhanceId;

        // 現在のレベル
        public static long CurrentLevel => PlayerEnhanceManager.GetPlayerEnhance(PlayerEnhanceType.TrainingDeckEnhance).level;
        
        // トレーニング編成強化レベルが最大レベルに達しているか
        public static bool IsMaxLevel => CurrentLevel >= GetMaxEnhanceLevel();

        // バッジが表示されるか
        public static bool IsTrainingDeckEnhanceBadge => IsUnLockTrainingEnhance() && CanDeckEnhanceLevelUp();
        
        //// <summary> トレーニング編成強化が解放されているか </summary>
        public static bool IsUnLockTrainingEnhance()
        { 
            return UserDataManager.Instance.IsUnlockSystem(TrainingDeckEnhanceLockId) || SystemUnlockDataManager.Instance.IsUnlockingSystem(TrainingDeckEnhanceLockId);
        }

        //// <summary> トレーニング編成強化レベルをあげることができるか (現在のレベルから1Levelあげれるか) </summary>
        public static bool CanDeckEnhanceLevelUp()
        {
            long currentLevel = CurrentLevel;
            return CanDeckEnhanceLevelUp(currentLevel, currentLevel + 1);
        }
        
        //// <summary> トレーニング編成強化レベルをあげることができるか </summary>
        public static bool CanDeckEnhanceLevelUp(long currentLv, long afterLv)
        {
            // レベルが上限に達しているならfalse
            if (IsMaxLevel)
            {
                return false;
            }
            
            var enhancePointData = GetDeckEnhancePointData();
         
            // 次のレベルに強化する素材があるか
            bool canNextLevelup = EnhancePointUtility.IsEnoughPoint(enhancePointData.GetTotalRequiredCost(currentLv, afterLv));
            
            //現在のレベルを１レベル強化するためのポイントがあるか
            return canNextLevelup;
        }

        //// <summary> トレーニング編成強化バフの最大強化レベルを取得 </summary>
        public static long GetMaxEnhanceLevel()
        {
            var mEnhanceId = MasterManager.Instance.playerEnhanceMaster.FindData(playerEnhanceId).mEnhanceId;
            EnhanceLevelMasterObject[] masterObjects = MasterManager.Instance.enhanceLevelMaster.FindByMEnhanceId(mEnhanceId);
            long maxLv = 0;
            foreach(EnhanceLevelMasterObject master in masterObjects)
            {
                maxLv = Math.Max( master.level, maxLv);
            }
            return maxLv;
        }

        //// <summary> 編成強化レベルを上げるために必要なポイントを取得 </summary>
        public static EnhancePointInfo GetDeckEnhancePointData()
        {
            var mPlayerEnhance = MasterManager.Instance.playerEnhanceMaster.FindData(playerEnhanceId);
            return new EnhancePointInfo(mPlayerEnhance.mEnhanceId);
        }

        //// <summary> 指定したデッキ種別のバフ効果情報の取得</summary>
        public static List<TrainingDeckEnhanceInfo> GetDeckEnhanceStatusDataList(DeckType deckType)
        {
            // 結果用
            List<TrainingDeckEnhanceInfo> result = new List<TrainingDeckEnhanceInfo>();
            
            foreach (var enhanceStatusMaster in MasterManager.Instance.deckEnhanceTrainingStatusMaster.values.Where(x => x.deckUseType == (long)deckType))
            {
                GetDeckEnhanceStatusData(enhanceStatusMaster.id, result);
            }

            return result;
        }

        //// <summary> m_deck_enhance_training_statusの１レコードからデータを取得して追加する </summary>
        public static void GetDeckEnhanceStatusData(long masterId, List<TrainingDeckEnhanceInfo> result)
        {
            var mStatus = MasterManager.Instance.deckEnhanceTrainingStatusMaster.FindData(masterId);
            
            // 全ての枠に対して効果対象なら対象となるスロットごとにデータを作成する
            if (mStatus.deckSlotIndex == AllEnhanceSlotIndex)
            {
                // 対象のターゲットのスロット番号を取得
                IEnumerable<long> slotIndexList = DeckUtility.GetSlotMaster((DeckType)mStatus.deckUseType).Select(x => x.index);
                
                for (int i = 0; i < mStatus.typeList.Length; i++)
                {
                    foreach (var slotIndex in slotIndexList)
                    {
                        // スロット番号ごとにそれぞれデータを作成
                        TrainingDeckEnhanceInfo enhanceInfo = new TrainingDeckEnhanceInfo(mStatus.id, (DeckType)mStatus.deckUseType, slotIndex, mStatus.targetType, mStatus.level, mStatus.typeList[i], mStatus.valueList[i]);
                        // 効果のマージ
                        MergeEnhanceList(enhanceInfo, result);
                    }
                }
            }
            // 対象の枠が全てでない時
            else
            {
                for (int i = 0; i < mStatus.typeList.Length; i++)
                {
                    TrainingDeckEnhanceInfo enhanceInfo = new TrainingDeckEnhanceInfo(mStatus.id, (DeckType)mStatus.deckUseType, mStatus.deckSlotIndex, mStatus.targetType, mStatus.level, mStatus.typeList[i], mStatus.valueList[i]);
                    // 効果のマージ
                    MergeEnhanceList(enhanceInfo, result);
                }
            }
        }

        //// <summary> 同一効果のマージ </summary>
        public static void MergeEnhanceList(TrainingDeckEnhanceInfo enhanceInfo, List<TrainingDeckEnhanceInfo> result)
        {
            // 同一のバフを探す
            TrainingDeckEnhanceInfo sameBuffList = result.FirstOrDefault(x => IsSameBuff(enhanceInfo, x));

            // 同一のバフがないならリストに追加
            if (sameBuffList == null)
            {
                result.Add(enhanceInfo);
                return;
            }

            // 効果値を加算する
            sameBuffList.AddValue(enhanceInfo);
        }

        //// <summary> 同じバフか? </summary>
        public static bool IsSameBuff(TrainingDeckEnhanceInfo buff1, TrainingDeckEnhanceInfo buff2)
        {
            // 枠番号 レベル 、バフカテゴリ、バフのアイコンが一致しているか
            return buff1.DeckSlotIndex == buff2.DeckSlotIndex && buff1.Level == buff2.Level && buff1.BuffCategoryId == buff2.BuffCategoryId && buff1.BuffIconId == buff2.BuffIconId;
        }

        //// <summary> そのレベルで発生するすべてのバフデータ(マージ済み)を構築する(編成効果データ) </summary>
        public static List<BuffTargetData> GetBuffTargetTotalEnhanceData(DeckType deckType, long currentLv, long afterLv, List<TrainingDeckEnhanceInfo> deckTypeList)
        {
            // 結果用 (Level5では1~5のレベルでのバフがマージされて格納されるイメージ)
            List<TrainingDeckEnhanceInfo> totalEnhanceList = new List<TrainingDeckEnhanceInfo>();
            
            // 指定したレベル以下で発生するバフをマージ
            foreach (TrainingDeckEnhanceInfo currentLevelEnhanceInfo in deckTypeList.Where(x => currentLv >= x.Level ))
            {
                TrainingDeckEnhanceInfo copyEnhanceInfo = new TrainingDeckEnhanceInfo(currentLevelEnhanceInfo, afterLv, false);
                MergeEnhanceList(copyEnhanceInfo, totalEnhanceList);
            }
            
            // 現在のレベルより高いレベルかつ強化後のレベル以下で発生するバフをマージ(新たに取得したバフを強調表示するため)
            foreach (TrainingDeckEnhanceInfo currentLevelEnhanceInfo in deckTypeList.Where(x => currentLv < x.Level && afterLv >= x.Level ))
            {
                // 新たに取得したバフは強調表示する
                TrainingDeckEnhanceInfo copyEnhanceInfo = new TrainingDeckEnhanceInfo(currentLevelEnhanceInfo, afterLv, true);
                MergeEnhanceList(copyEnhanceInfo, totalEnhanceList);
            }
            
            return GetBuffTargetData(deckType, afterLv, totalEnhanceList);
        }
        
        //// <summary> 指定したバフの対象を取得(LevelUp効果データ) </summary>
        public static List<BuffTargetData> GetBuffTargetLevelUpData(DeckType deckType, long level, List<TrainingDeckEnhanceInfo> deckTypeList)
        {
            return GetBuffTargetData(deckType, level, deckTypeList.Where(x => x.Level == level).ToList());
        }

        //// <summary> 新たに獲得するバフデータを取得 </summary>
        public static List<BuffTargetData> GetBuffLevelUpAcquireData(DeckType deckType, long currentLv, long afterLv, List<TrainingDeckEnhanceInfo> deckTypeList)
        {
            // 結果用
            List<TrainingDeckEnhanceInfo> totalEnhanceList = new List<TrainingDeckEnhanceInfo>();
            
            // 現在のレベルより高く、強化後で発生するバフまでを取得してマージ
            foreach (TrainingDeckEnhanceInfo currentLevelEnhanceInfo in deckTypeList.Where(x => currentLv < x.Level && afterLv >= x.Level))
            {
                TrainingDeckEnhanceInfo copyEnhanceInfo = new TrainingDeckEnhanceInfo(currentLevelEnhanceInfo, afterLv, false);
                MergeEnhanceList(copyEnhanceInfo, totalEnhanceList);
            }
            
            return GetBuffTargetData(deckType, afterLv, totalEnhanceList);
        }
        
        //// <summary> 指定されたDeckTypeの枠番号ごとにグループデータを作成 </summary>
        public static List<BuffTargetData> GetBuffTargetData(DeckType deckType,long level, List<TrainingDeckEnhanceInfo> enhanceInfoList)
        {
            List<BuffTargetData> buffTargetDataList = new List<BuffTargetData>();
            // 枠番号でグループ化
            IEnumerable<IGrouping<long,TrainingDeckEnhanceInfo>> groupList = enhanceInfoList.GroupBy(x => x.DeckSlotIndex);

            foreach (IGrouping<long, TrainingDeckEnhanceInfo> group in groupList)
            {
                // 表示順でソート(昇順)
                List<TrainingDeckEnhanceInfo> sortEnhanceList = group.ToList().OrderBy(x => x.Priority).ThenBy(x => x.BuffCategoryId).ToList();
                TrainingDeckSlotData slotData = TrainingDeckUtility.GetEnhanceTargetType(deckType, group.Key);
                buffTargetDataList.Add(new BuffTargetData(slotData.SlotType ,slotData.Index, level, sortEnhanceList));
            }
            return buffTargetDataList;
        }

        //// <summary> 強化バフの対象のリストデータ </summary>
        public static List<TrainingDeckSlotData> GetEnhanceTargetList()
        {
            List<TrainingDeckSlotData> enhanceList = new List<TrainingDeckSlotData>();

            // 育成対象選手
            enhanceList.Add(new TrainingDeckSlotData(TrainingDeckSlotType.GrowthTarget, 0));
            // サポート選手(フレンド枠もサポート選手とみなすので+1)
            for (int i = 1; i <= DeckUtility.GetCharacterIndex(DeckType.Training, DeckSlotCardType.Support).Length + 1; i++)
            {
                enhanceList.Add(new TrainingDeckSlotData(TrainingDeckSlotType.SupportCharacter, i));
            }
            // スペシャルサポートカード
            for (int i = 1; i <= DeckUtility.GetSpecialSupportCharacterIndex(DeckType.Training, true).Length; i++)
            {
                enhanceList.Add(new TrainingDeckSlotData(TrainingDeckSlotType.SupportCard, i));
            }
            // Exサポートカード
            for (int i = 1; i <= DeckUtility.GetExtraSupportCharacterIndex(DeckType.Training, true).Length; i++)
            {
                enhanceList.Add(new TrainingDeckSlotData(TrainingDeckSlotType.ExSupportCard, i));
            }
            // サポート器具
            for (int i = 1; i <= DeckUtility.GetCharacterIndex(DeckType.SupportEquipment, DeckSlotCardType.SupportEquipment).Length; i++)
            {
                enhanceList.Add(new TrainingDeckSlotData(TrainingDeckSlotType.SupportEquipment, i));
            }
            return enhanceList;
        }
    }
}