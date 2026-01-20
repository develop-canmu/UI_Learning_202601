//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class CombinationStatusTrainingBase {
		public long[] typeList = null; // このレコードが持つ練習能力タイプ（m_training_status_type_detail.type）を詰めたjson配列。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		public long[] valueList = null; // このレコードが持つ練習能力の効果量を詰めたjson配列。値の順番はtypeListに対応する。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		public CombinationStatusCommon firstParamAddMap = null;
		public long battleParamEnhanceRate = 0;
		public long rarePracticeEnhanceRate = 0;
		public CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnType = null;
		public CharaVariableTrainerStatusCommon practiceParamAddBonusMap = null;
		public CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnType = null;
		public CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnType = null;
		public CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnType = null;
		public long[] firstMTrainingEventRewardIdList = null;
		public long conditionEffectGradeUpRate = 0;
		public CharaVariableTrainerStatusCommon practiceParamRateMap = null;
		public long conditionDiscountRate = 0;
		public CharaVariableTrainerCoachEnhanceRateOnType[] coachEnhanceRateMapOnType = null;
		public long entireCoachRewardEnhanceRate = 0;

   }
   
}