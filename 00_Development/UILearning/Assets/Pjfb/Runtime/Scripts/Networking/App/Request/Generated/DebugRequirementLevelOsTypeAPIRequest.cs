//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
リクエストの共通部に osType の指定が必要なAPIのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugRequirementLevelOsTypeAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugRequirementLevelOsTypeAPIResponse : AppAPIResponseBase {
		public string message = ""; // テストメッセージ

   }
      
   public partial class DebugRequirementLevelOsTypeAPIRequest : AppAPIRequestBase<DebugRequirementLevelOsTypeAPIPost, DebugRequirementLevelOsTypeAPIResponse> {
      public override string apiName{get{ return "debug/requirementLevelOsType"; } }
   }
}