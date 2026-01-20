//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
既読 mTrainingEventId リストの取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingGetEventStatusAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TrainingGetEventStatusAPIResponse : AppAPIResponseBase {
		public long[] eventIdList = null; // 既読 mTrainingEventId リスト

   }
      
   public partial class TrainingGetEventStatusAPIRequest : AppAPIRequestBase<TrainingGetEventStatusAPIPost, TrainingGetEventStatusAPIResponse> {
      public override string apiName{get{ return "training/getEventStatus"; } }
   }
}