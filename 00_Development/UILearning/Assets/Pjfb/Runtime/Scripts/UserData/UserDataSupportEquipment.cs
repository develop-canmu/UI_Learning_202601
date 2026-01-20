using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Extensions;
using UnityEngine;
using Pjfb.Networking.App.Request;

namespace Pjfb.UserData {
    
    public partial class UserDataSupportEquipment {
        public long id{get;private set;}
		public long masterId{get;private set;}
		public long charaId{get;private set;}
		public long level{get;private set;}
        public CharaVariableTrainerStatusCommon firstParamAddMap;
        public long battleParamEnhanceRate { get; private set; }
        public long rarePracticeEnhanceRate { get; private set; }
        public CharaVariableTrainerStatusCommon battleParamEnhanceMap{ get; private set; }
        public CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnType{ get; private set; }
        public CharaVariableTrainerStatusCommon practiceParamAddBonusMap{ get; private set; }
        public CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnType{ get; private set; }
        public CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnType{ get; private set; }
        public CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnType{ get; private set; }
        public long[] firstRewardIdList{ get; private set; }
        public CharaVariableTrainerLotteryProcess lotteryProcessJson{ get; private set; }
        public bool isLocked{ get; private set; }
        public bool isReLotteryLocked{ get; private set; }
        public long rankNumber{ get; private set; }
        public long lotteryRarityValue{ get; private set; }
        public DateTime createAt{ get; private set; }

        public UserDataSupportEquipment( CharaVariableTrainerBase supportEquipment ){
            this.id = supportEquipment.id;
            this.masterId = supportEquipment.uMasterId;
            this.charaId = supportEquipment.mCharaId;
            this.level = supportEquipment.level;
            this.firstParamAddMap = supportEquipment.firstParamAddMap;
            this.battleParamEnhanceRate = supportEquipment.battleParamEnhanceRate;
            this.rarePracticeEnhanceRate = supportEquipment.rarePracticeEnhanceRate;
            this.battleParamEnhanceMap = supportEquipment.battleParamEnhanceMap;
            this.conditionEffectGradeUpMapOnType = supportEquipment.conditionEffectGradeUpMapOnType;
            this.practiceParamAddBonusMap = supportEquipment.practiceParamAddBonusMap;
            this.practiceParamEnhanceMapOnType = supportEquipment.practiceParamEnhanceMapOnType;
            this.rarePracticeEnhanceRateMapOnType = supportEquipment.rarePracticeEnhanceRateMapOnType;
            this.popRateEnhanceMapOnType = supportEquipment.popRateEnhanceMapOnType;
            this.firstRewardIdList = supportEquipment.firstMTrainingEventRewardIdList;
            this.lotteryProcessJson = supportEquipment.lotteryProcessJson;
            this.isLocked = supportEquipment.isLocked;
            this.isReLotteryLocked = supportEquipment.isReLotteryLocked;
            this.rankNumber = supportEquipment.rankNumber;
            this.lotteryRarityValue = supportEquipment.lotteryRarityValue;
            this.createAt = supportEquipment.createdAt.TryConvertToDateTime();
            SubPracticeSkillDataList = PracticeSkillUtility.GetCharaTrainerLotteryStatusPracticeSkill(this.lotteryProcessJson.statusList);
        }
    }


    public class UserDataSupportEquipmentContainer : UserDataContainer<long, UserDataSupportEquipment> {
        
        
        /// <summary>
        /// データ更新
        /// </summary>
        public void Update( CharaVariableTrainerBase[] supportEquipments ){
            if( supportEquipments == null ) {
               return; 
            } 
            
            foreach ( var supportEquipment in supportEquipments) {
                var userSupportEquipment = new UserDataSupportEquipment(supportEquipment);
                Update(userSupportEquipment.id, userSupportEquipment);
            }

            ForeachHandler((handler)=>{ handler.OnUpdatedData(); });
        }   

        /// <summary>
        /// データ削除
        /// </summary>
        public void Remove(long[] ids) {
            foreach ( var id in ids) {
                Remove(id);
            }
            
            // 未確認のものが削除された際にバッジを消せるように
            ForeachHandler((handler)=>{ handler.OnUpdatedData(); });
        }

        public bool HaveCharacterWithMasterCharaId( long masterCharaId ){
            return data.Any(itr => itr.Value.charaId == masterCharaId );
        }

    }
}