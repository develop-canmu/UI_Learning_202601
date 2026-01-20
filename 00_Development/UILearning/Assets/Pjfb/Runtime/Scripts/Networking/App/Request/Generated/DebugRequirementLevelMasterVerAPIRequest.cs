//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
リクエストの共通部に osType, appVer, assetVer, masterVer の指定が必要なAPIのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugRequirementLevelMasterVerAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugRequirementLevelMasterVerAPIResponse : AppAPIResponseBase {
		public string message = ""; // テストメッセージ

   }
      
   public partial class DebugRequirementLevelMasterVerAPIRequest : AppAPIRequestBase<DebugRequirementLevelMasterVerAPIPost, DebugRequirementLevelMasterVerAPIResponse> {
      public override string apiName{get{ return "debug/requirementLevelMasterVer"; } }
   }
}