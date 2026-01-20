using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UserData;

using System.Linq;

namespace Pjfb
{
    public static class CharacterUtility
    {
        /// <summary>育成選手本人がサポートカード扱いでトレーニングに参加できる？（育成選手Lv100ボーナスを持っている？）</summary>
        public static bool HasJoinTrainingHimselfBonus(long level)
        {
            const long bonusLevel = 100;
            return level >= bonusLevel;
        }

        /// <summary>最大レベル？</summary>
        public  static bool IsMaxGrowthLevel(long mCharId, long level, long liberationLevel)
        {
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(mCharId);

            long growthMax = 0;
            foreach(StatusAdditionLevelMasterObject mStatus in MasterManager.Instance.statusAdditionLevelMaster.values)
            {
                if(mStatus.mStatusAdditionId != mChara.mStatusAdditionIdGrowth)continue;
                growthMax = System.Math.Max(mStatus.maxLevelGrowth, growthMax);
            }
            
            long liberationMax = 0;
            foreach(StatusAdditionLevelMasterObject mStatus in MasterManager.Instance.statusAdditionLevelMaster.values)
            {
                if(mStatus.mStatusAdditionId != mChara.mStatusAdditionIdLiberation)continue;
                liberationMax = System.Math.Max(mStatus.maxLevelGrowth, liberationMax);
            }

            return level >= mChara.maxLevel + growthMax + liberationMax;
        }
        
        /// <summary>同一人物？</summary>
        public static bool IsSameCharacter(long mCharId1, long mCharId2)
        {
            return CharIdToParentId(mCharId1) == CharIdToParentId(mCharId2);
        }
        
        /// <summary>同一人物？</summary>
        public static bool IsSameUserCharacter(long uCharId1, long uCharId2)
        {
            return UserCharIdToParentId(uCharId1) == UserCharIdToParentId(uCharId2);
        }
        
        /// <summary>ParentId</summary>
        public static long CharIdToParentId(long mCharId)
        {
            return MasterManager.Instance.charaMaster.FindData(mCharId).parentMCharaId;
        }
        
        /// <summary>ParentId</summary>
        public static long UserCharIdToParentId(long uCharId)
        {
            return UserDataManager.Instance.chara.Find(uCharId).MChara.parentMCharaId;
        }
        
        /// <summary>ParentId</summary>
        public static long UserEquipmentIdToParentId(long uEquipmentId)
        {
            return CharIdToParentId(UserDataManager.Instance.supportEquipment.Find(uEquipmentId).charaId);
        }
        
        /// <summary>mCharId</summary>
        public static long UserCharIdToMCharId(long uCharId)
        {
            return UserDataManager.Instance.chara.Find(uCharId).MChara.id;
        }
        
        /// <summary>mCharId</summary>
        public static long UserEquipmentIdToMCharId(long uEquipmentId)
        {
            return UserDataManager.Instance.supportEquipment.Find(uEquipmentId).charaId;
        }
        
        /// <summary>立ち絵のId</summary>
        public static string CharIdToStandingImageId(long mCharId)
        {
            return MasterManager.Instance.charaMaster.FindData(mCharId).standingImageMCharaId;
        }
        
        /// <summary>ParentId</summary>
        public static long UserVariableCharIdToParentId(long uCharId)
        {
            return UserDataManager.Instance.charaVariable.Find(uCharId).ParentMCharaId;
        }
        
        public static long GetMaxGrowthLevel(long mCharId, long lv, long liberationLv)
        {
            
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(mCharId);
            
            StatusAdditionLevelMasterObject[] MStatusAdditionLevelMasterObjectList_Growth = MasterManager.Instance.statusAdditionLevelMaster.values.Where(data =>data.mStatusAdditionId == mChar.mStatusAdditionIdGrowth).ToArray();
            StatusAdditionLevelMasterObject[] MStatusAdditionLevelMasterObjectList_Liberation = MasterManager.Instance.statusAdditionLevelMaster.values.Where(data =>data.mStatusAdditionId == mChar.mStatusAdditionIdLiberation).ToArray();
            
            var mStatusAdditionalLevelGrowth = MStatusAdditionLevelMasterObjectList_Growth.FirstOrDefault(x => x.level == lv);
            var mStatusAdditionalLevelLiberation = MStatusAdditionLevelMasterObjectList_Liberation.FirstOrDefault(x => x.level == liberationLv);
            
            if (mStatusAdditionalLevelGrowth is null || mStatusAdditionalLevelLiberation is null)
            {
                return mChar.maxLevel;  
            } 
            
            
            return mChar.maxLevel + mStatusAdditionalLevelGrowth.maxLevelGrowth + mStatusAdditionalLevelLiberation.maxLevelGrowth;
        }
        
        /// <summary>特攻キャラチェック</summary>
        public static bool IsTrainingScenarioSpAttackCharacter(long mCharId, long lv, long trainingScenarioId)
        {
            if(trainingScenarioId > 0)
            {   
                if(MasterManager.Instance.charaTrainingStatusMaster.HasStatus(mCharId, lv, trainingScenarioId))
                {
                    return true;
                }
            }
            return false;
        }
        
        
        public  static bool IsExtraCharacter(long mCharId)
        {
            return MasterManager.Instance.charaMaster.FindData(mCharId).isExtraSupport;
        }
        
        public static bool IsCharacterAvailable(long parentMCharaId, long mCharId)
        {
            CharaParentMasterObject mCharaParent = MasterManager.Instance.charaParentMaster.FindDataByParentMCharaId(parentMCharaId);
            if (mCharaParent == null) return false;
            for (int i = 0; i < mCharaParent.mCharaIdList.Length; i++)
            {
                if(mCharaParent.mCharaIdList[i] == mCharId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
