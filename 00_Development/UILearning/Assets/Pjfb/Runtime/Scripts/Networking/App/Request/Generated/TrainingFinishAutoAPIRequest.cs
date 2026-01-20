//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
自動トレーニングの終了

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingFinishAutoAPIPost : AppAPIPostBase {
		public long slotNumber = 0; // 枠番号
		public long id = 0; // 即時完了の場合はmTrainingAutoCostのidを指定

   }

   [Serializable]
   public class TrainingFinishAutoAPIResponse : AppAPIResponseBase {
		public TrainingPending pending = null; // 個人トレーニング途中情報
		public TrainingCharaVariable charaVariable = null; // 育成中キャラ情報
		public TrainingFriend friend = null; // フレンド情報
		public EnhanceLevelProgress[] levelProgressList = null; // 信頼度変動
		public FestivalEffectStatus[] festivalEffectStatusList = null; // トレーニングイベント特殊効果情報
		public FestivalPointProgress[] festivalPointProgressList = null; // トレーニングイベントポイント変動
		public TrainingAutoResultStatus result = null; // 自動トレーニング結果

   }
      
   public partial class TrainingFinishAutoAPIRequest : AppAPIRequestBase<TrainingFinishAutoAPIPost, TrainingFinishAutoAPIResponse> {
      public override string apiName{get{ return "training/finishAuto"; } }
   }
}