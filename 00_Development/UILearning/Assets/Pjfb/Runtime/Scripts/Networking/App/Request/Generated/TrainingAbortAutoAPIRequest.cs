//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
自動トレーニングの中止

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingAbortAutoAPIPost : AppAPIPostBase {
		public long slotNumber = 0; // 枠番号

   }

   [Serializable]
   public class TrainingAbortAutoAPIResponse : AppAPIResponseBase {

   }
      
   public partial class TrainingAbortAutoAPIRequest : AppAPIRequestBase<TrainingAbortAutoAPIPost, TrainingAbortAutoAPIResponse> {
      public override string apiName{get{ return "training/abortAuto"; } }
   }
}