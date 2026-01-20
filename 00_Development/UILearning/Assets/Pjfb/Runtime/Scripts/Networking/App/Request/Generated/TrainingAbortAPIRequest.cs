//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
個人トレーニングの中止

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingAbortAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TrainingAbortAPIResponse : AppAPIResponseBase {

   }
      
   public partial class TrainingAbortAPIRequest : AppAPIRequestBase<TrainingAbortAPIPost, TrainingAbortAPIResponse> {
      public override string apiName{get{ return "training/abort"; } }
   }
}