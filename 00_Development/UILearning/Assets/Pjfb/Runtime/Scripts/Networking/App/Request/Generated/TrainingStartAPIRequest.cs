//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
個人トレーニングの開始

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingStartAPIPost : AppAPIPostBase {
		public long trainingUCharaId = 0; // 育成を行いたい強化指定選手の uCharaId
		public long mTrainingScenarioId = 0; // プレイするシナリオの mTrainingScenarioId
		public long partyNumber = 0; // サポートとして使用するデッキの partyNumber
		public long trainerPartyNumber = 0; // トレーニング補助キャラデッキの partyNumber
		public long friendUCharaId = 0; // サポートとして使用するフレンドの uCharaId ※自身のuCharaIdでも可

   }

   [Serializable]
   public class TrainingStartAPIResponse : AppAPIResponseBase {
		public TrainingTrainingEvent trainingEvent = null; // イベント情報
		public TrainingPending pending = null; // 個人トレーニング途中情報
		public TrainingCharaVariable charaVariable = null; // 育成中キャラ情報
		public TrainingBattlePending battlePending = null; // 個人トレーニング中バトル情報
		public TrainingPointStatus pointStatus = null; // トレーニング専用ポイント関連情報
		public TrainingFriend friend = null; // フレンド情報
		public long maxAddTurnValue = 0; // 最大加算ターン数

   }
      
   public partial class TrainingStartAPIRequest : AppAPIRequestBase<TrainingStartAPIPost, TrainingStartAPIResponse> {
      public override string apiName{get{ return "training/start"; } }
   }
}