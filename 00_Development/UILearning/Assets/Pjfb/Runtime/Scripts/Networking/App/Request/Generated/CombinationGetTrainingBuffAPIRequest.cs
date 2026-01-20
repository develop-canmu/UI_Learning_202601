//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザーに付加されているトレーニング補正値を得る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CombinationGetTrainingBuffAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CombinationGetTrainingBuffAPIResponse : AppAPIResponseBase {
		public CombinationStatusTrainingBase statusTrainingBase = null; // トレーニング補正値

   }
      
   public partial class CombinationGetTrainingBuffAPIRequest : AppAPIRequestBase<CombinationGetTrainingBuffAPIPost, CombinationGetTrainingBuffAPIResponse> {
      public override string apiName{get{ return "combination/getTrainingBuff"; } }
   }
}