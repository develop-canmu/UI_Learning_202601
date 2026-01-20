//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
自動トレーニングの開始

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingStartAutoAPIPost : AppAPIPostBase {
		public long slotNumber = 0; // 枠番号
		public bool isRetry = false; // 同じ設定で再実行するか。する場合はデータを使いまわして実行
		public long statusType = 0; // 選択した重視ステータス。1=>スタミナ, 2=>スピード, 3=>フィジカル, 4=>テクニック, 5=>賢さ, 6=>キック, 7=>戦力重視
		public long trainingUCharaId = 0; // 育成を行いたい強化指定選手の uCharaId
		public long mTrainingScenarioId = 0; // プレイするシナリオの mTrainingScenarioId
		public long partyNumber = 0; // サポートとして使用するデッキの partyNumber
		public long trainerPartyNumber = 0; // トレーニング補助キャラデッキの partyNumber
		public long friendUCharaId = 0; // サポートとして使用するフレンドの uCharaId ※自身のuCharaIdでも可

   }

   [Serializable]
   public class TrainingStartAutoAPIResponse : AppAPIResponseBase {
		public TrainingAutoPendingStatus pendingStatus = null; // 自動トレーニング状況

   }
      
   public partial class TrainingStartAutoAPIRequest : AppAPIRequestBase<TrainingStartAutoAPIPost, TrainingStartAutoAPIResponse> {
      public override string apiName{get{ return "training/startAuto"; } }
   }
}