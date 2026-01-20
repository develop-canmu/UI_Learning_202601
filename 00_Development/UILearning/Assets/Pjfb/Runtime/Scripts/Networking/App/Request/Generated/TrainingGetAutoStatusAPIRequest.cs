//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
自動トレーニングデータ取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingGetAutoStatusAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TrainingGetAutoStatusAPIResponse : AppAPIResponseBase {
		public TrainingAutoPendingStatus[] pendingStatusList = null; // 自動トレーニング状況リスト
		public TrainingAutoUserStatus userStatus = null; // ユーザーの自動トレーニング情報
		public long trainingAutoRequireMinute = 0; // 自動トレーニングの１回あたりの時間（分）

   }
      
   public partial class TrainingGetAutoStatusAPIRequest : AppAPIRequestBase<TrainingGetAutoStatusAPIPost, TrainingGetAutoStatusAPIResponse> {
      public override string apiName{get{ return "training/getAutoStatus"; } }
   }
}