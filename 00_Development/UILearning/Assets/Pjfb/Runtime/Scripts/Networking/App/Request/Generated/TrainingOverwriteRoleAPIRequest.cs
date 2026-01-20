//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ポジションの変更のみ

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TrainingOverwriteRoleAPIPost : AppAPIPostBase {
		public WrapperIntList[] idRoleList = null; // [tableType, id, roleNumber]を列挙した2次元配列

   }

   [Serializable]
   public class TrainingOverwriteRoleAPIResponse : AppAPIResponseBase {

   }
      
   public partial class TrainingOverwriteRoleAPIRequest : AppAPIRequestBase<TrainingOverwriteRoleAPIPost, TrainingOverwriteRoleAPIResponse> {
      public override string apiName{get{ return "training/overwriteRole"; } }
   }
}