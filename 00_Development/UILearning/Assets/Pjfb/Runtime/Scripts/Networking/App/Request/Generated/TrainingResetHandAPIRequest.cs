//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
練習手札リストをリセット

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingResetHandAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TrainingResetHandAPIResponse : AppAPIResponseBase {
		public TrainingPending pending = null; // 個人トレーニング途中情報
		public TrainingPointStatus pointStatus = null; // トレーニング専用ポイント関連情報

   }
      
   public partial class TrainingResetHandAPIRequest : AppAPIRequestBase<TrainingResetHandAPIPost, TrainingResetHandAPIResponse> {
      public override string apiName{get{ return "training/resetHand"; } }
   }
}