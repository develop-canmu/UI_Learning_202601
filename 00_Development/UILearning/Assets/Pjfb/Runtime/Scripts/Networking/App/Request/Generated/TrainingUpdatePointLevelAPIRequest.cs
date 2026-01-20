//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
専用ポイントのステータスレベルを上げる

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingUpdatePointLevelAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TrainingUpdatePointLevelAPIResponse : AppAPIResponseBase {
		public TrainingPending pending = null; // 個人トレーニング途中情報
		public long[] mTrainingPointStatusEffectIdList = null; // 発動したmTrainingPointStatusEffectのidリスト
		public long[] additionMTrainingPointStatusEffectIdList = null; // 追加で発動したmTrainingPointStatusEffectのidリスト
		public long[] charaMTrainingPointStatusEffectIdList = null; // 発動したキャラのmTrainingPointStatusEffectのidリスト
		public long mTrainingPointStatusEffectCharaId = 0; // 発動したmTrainingPointStatusEffectCharaのid
		public TrainingPointStatus pointStatus = null; // トレーニング専用ポイント関連情報

   }
      
   public partial class TrainingUpdatePointLevelAPIRequest : AppAPIRequestBase<TrainingUpdatePointLevelAPIPost, TrainingUpdatePointLevelAPIResponse> {
      public override string apiName{get{ return "training/updatePointLevel"; } }
   }
}