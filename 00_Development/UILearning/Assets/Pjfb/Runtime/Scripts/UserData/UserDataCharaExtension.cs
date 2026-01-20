using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Logger = CruFramework.Logger;

namespace Pjfb.UserData {
    
    public partial class UserDataChara
    {
        public CardType CardType => MChara.cardType;
        public long ParentMCharaId => MChara.parentMCharaId;

        public CharaMasterObject MChara => MasterManager.Instance.charaMaster.FindData(charaId);
        
        public bool IsMaxGrowthLevel => level >= GetMaxGrowthLevel(); 
        
        /// <summary>能力解放のみ</summary>
        public EnhanceLevelMasterObject[] MEnhanceLevelList => MasterManager.Instance.enhanceLevelMaster.values
            .Where(data => data.mEnhanceId == MChara.mEnhanceIdLiberation).ToArray();
        ///　<summary>強化のみ</summary>
        public EnhanceLevelPointMasterObject[] MEnhanceLevelPointList => MasterManager.Instance.enhanceLevelPointMaster.values
            .Where(data => data.mEnhanceId == MChara.mEnhanceIdGrowth).ToArray();

        public StatusAdditionLevelMasterObject[] MStatusAdditionLevelMasterObjectList_Growth => MasterManager.Instance
            .statusAdditionLevelMaster.values.Where(data =>
                data.mStatusAdditionId == MChara.mStatusAdditionIdGrowth).ToArray();

        public StatusAdditionLevelMasterObject[] MStatusAdditionLevelMasterObjectList_Liberation => MasterManager.Instance
            .statusAdditionLevelMaster.values.Where(data =>
                data.mStatusAdditionId == MChara.mStatusAdditionIdLiberation).ToArray();
        
        public TrainingCardCharaMasterObject[] MTrainingCardCharaList => MasterManager.Instance.trainingCardCharaMaster.values
            .Where(data => data.mCharaId == charaId).ToArray();
        
        public long GetMaxGrowthLevel(long liberationLv)
        {
            //1. m_chara.maxLevel を取得
            //2. u_chara.level と m_chara.mStatusAdditionIdGrowth をもとに m_status_addition_level.maxLevelGrowth を取得
            //3. u_chara.newLiberationLevel と m_chara.mStatusAdditionIdLiberation をもとに m_status_addition_level.maxLevelGrowth を取得
            //4. 1～3の合計値が強化の最大レベルになる

            var mStatusAdditionalLevelGrowth = MStatusAdditionLevelMasterObjectList_Growth.FirstOrDefault(x => x.level == level);
            var mStatusAdditionalLevelLiberation = MStatusAdditionLevelMasterObjectList_Liberation.FirstOrDefault(x => x.level == liberationLv);
            
            if (mStatusAdditionalLevelGrowth is null || mStatusAdditionalLevelLiberation is null)
            {
                Logger.LogError(
                    $"MStatusAdditionalLevelGrowth か　mStatusAdditionalLevelLiberationのデータがありません  MCharaId : {charaId}");
                return 0;  
            } 
            
            
            return MChara.maxLevel + mStatusAdditionalLevelGrowth.maxLevelGrowth + mStatusAdditionalLevelLiberation.maxLevelGrowth;
        }
        
        public long GetMaxGrowthLevel()
        {
            return GetMaxGrowthLevel(newLiberationLevel);
        }

        public bool IsPossibleLiberation()
        {
            var enhanceLevelCache = MEnhanceLevelList.ToDictionary(o => o.level);
            var currentLv = newLiberationLevel;
            var nextLv = newLiberationLevel + 1;
            var maxLevel = enhanceLevelCache.Count > 0 ? enhanceLevelCache.Keys.Max() : 0;
            var isMax = currentLv >= maxLevel;
            if(isMax) return false;;
                
            if (!enhanceLevelCache.ContainsKey(currentLv))
            {
                Debug.LogError($"Not found level {currentLv} EnhanceLevelMasterObject data");
                return false;
            }
            if (!enhanceLevelCache.ContainsKey(nextLv))
            {
                Debug.LogError($"Not found level {nextLv} EnhanceLevelMasterObject data");
                return false;;
            }
                
            EnhanceLevelMasterObject currentMEnhanceLevel = enhanceLevelCache[currentLv];
            EnhanceLevelMasterObject afterMEnhanceLevel = enhanceLevelCache[nextLv];
            var charaPiece = UserDataManager.Instance.charaPiece.data.FirstOrDefault(data => data.Key == charaId).Value;
            var possessionValue = charaPiece?.value ?? 0;
            var requiredValue = afterMEnhanceLevel.totalExp - currentMEnhanceLevel.totalExp;
            if (possessionValue < requiredValue) return false;
            return true;
        }
        
        public bool IsPossibleGrowth()
        {
            var enhanceLevelPointCache = MEnhanceLevelPointList.GroupBy(x => x.level).ToDictionary(g => g.Key, g => g.ToList());
            var currentLv = level;
            var nextLv = level + 1;
            var maxLevel = GetMaxGrowthLevel();
            var isMax = currentLv >= maxLevel;
            if(isMax) return false;
            
            if (!enhanceLevelPointCache.ContainsKey(nextLv))
            {
                Debug.LogError($"Not found level {nextLv} EnhanceLevelMasterObject data");
                return false;
            }
                
            List<EnhanceLevelPointMasterObject> afterMEnhanceLevelPointList = enhanceLevelPointCache[nextLv];
            foreach (var enhanceLevelPointMaster in afterMEnhanceLevelPointList)
            {
                long mPointId = enhanceLevelPointMaster.mPointId;
                long requiredValue = enhanceLevelPointMaster.value;
                long possessionValue = UserDataManager.Instance.point.Find(mPointId)?.value ?? 0;
                if (possessionValue < requiredValue) return false;
            }
            return true;
        }

        public bool IsPossiblePieceToChara()
        {
            var possessionCharaPieceValue = UserDataManager.Instance.charaPiece.data.Values.FirstOrDefault(data => data.charaId == charaId)?.value ?? 0;
            var requiredCharaPieceValue = MChara.priceFromPiece;
            return possessionCharaPieceValue >= requiredCharaPieceValue;
        }
    }
}