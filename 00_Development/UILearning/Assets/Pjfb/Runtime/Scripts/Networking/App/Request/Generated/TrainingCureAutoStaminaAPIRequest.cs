//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
自動トレーニングスタミナ回復実行

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingCureAutoStaminaAPIPost : AppAPIPostBase {
		public long id = 0; // mTrainingAutoCostのid

   }

   [Serializable]
   public class TrainingCureAutoStaminaAPIResponse : AppAPIResponseBase {
		public TrainingAutoUserStatus userStatus = null; // ユーザーの自動トレーニング情報

   }
      
   public partial class TrainingCureAutoStaminaAPIRequest : AppAPIRequestBase<TrainingCureAutoStaminaAPIPost, TrainingCureAutoStaminaAPIResponse> {
      public override string apiName{get{ return "training/cureAutoStamina"; } }
   }
}