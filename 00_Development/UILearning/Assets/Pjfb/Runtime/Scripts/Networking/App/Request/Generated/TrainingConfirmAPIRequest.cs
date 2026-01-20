//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
個人トレーニング状況の確認

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingConfirmAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TrainingConfirmAPIResponse : AppAPIResponseBase {
		public long code = 0; // 0:中断データあり。trainingEvent, pending, charaVariable がレスポンスに含まれる、99:中断データなし
		public TrainingTrainingEvent trainingEvent = null; // イベント情報
		public TrainingPending pending = null; // 個人トレーニング途中情報
		public TrainingCharaVariable charaVariable = null; // 育成中キャラ情報
		public TrainingBattlePending battlePending = null; // 個人トレーニング中バトル情報
		public TrainingPointStatus pointStatus = null; // トレーニング専用ポイント関連情報
		public TrainingFriend friend = null; // フレンド情報
		public long maxAddTurnValue = 0; // 最大加算ターン数

   }
      
   public partial class TrainingConfirmAPIRequest : AppAPIRequestBase<TrainingConfirmAPIPost, TrainingConfirmAPIResponse> {
      public override string apiName{get{ return "training/confirm"; } }
   }
}