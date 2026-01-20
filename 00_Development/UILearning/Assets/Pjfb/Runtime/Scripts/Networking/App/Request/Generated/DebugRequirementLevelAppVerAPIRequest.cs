//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
リクエストの共通部に osType, appVer の指定が必要なAPIのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugRequirementLevelAppVerAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugRequirementLevelAppVerAPIResponse : AppAPIResponseBase {
		public string message = ""; // テストメッセージ

   }
      
   public partial class DebugRequirementLevelAppVerAPIRequest : AppAPIRequestBase<DebugRequirementLevelAppVerAPIPost, DebugRequirementLevelAppVerAPIResponse> {
      public override string apiName{get{ return "debug/requirementLevelAppVer"; } }
   }
}