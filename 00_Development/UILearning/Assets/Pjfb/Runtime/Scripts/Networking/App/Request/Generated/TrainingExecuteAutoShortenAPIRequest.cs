//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
自動トレーニング時間短縮実行

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingExecuteAutoShortenAPIPost : AppAPIPostBase {
		public long number = 0; // 自動トレーニングの枠番号
		public long id = 0; // mTrainingAutoCostのid

   }

   [Serializable]
   public class TrainingExecuteAutoShortenAPIResponse : AppAPIResponseBase {
		public TrainingAutoPendingStatus pendingStatus = null; // 自動トレーニング状況
		public TrainingAutoUserStatus userStatus = null; // ユーザーの自動トレーニング情報

   }
      
   public partial class TrainingExecuteAutoShortenAPIRequest : AppAPIRequestBase<TrainingExecuteAutoShortenAPIPost, TrainingExecuteAutoShortenAPIResponse> {
      public override string apiName{get{ return "training/executeAutoShorten"; } }
   }
}