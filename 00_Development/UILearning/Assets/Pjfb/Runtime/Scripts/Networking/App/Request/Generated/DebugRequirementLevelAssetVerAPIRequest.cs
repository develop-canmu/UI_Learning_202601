//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
リクエストの共通部に osType, appVer, assetVer の指定が必要なAPIのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugRequirementLevelAssetVerAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugRequirementLevelAssetVerAPIResponse : AppAPIResponseBase {
		public string message = ""; // テストメッセージ

   }
      
   public partial class DebugRequirementLevelAssetVerAPIRequest : AppAPIRequestBase<DebugRequirementLevelAssetVerAPIPost, DebugRequirementLevelAssetVerAPIResponse> {
      public override string apiName{get{ return "debug/requirementLevelAssetVer"; } }
   }
}