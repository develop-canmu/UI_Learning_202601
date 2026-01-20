//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
リクエストの共通部に osType, appVer, assetVer, masterVer, sessionId, loginId の指定が必要なAPIのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugRequirementLevelLoginAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugRequirementLevelLoginAPIResponse : AppAPIResponseBase {
		public string message = ""; // テストメッセージ
		public long uMasterId = 0; // ユーザID
		public string name = ""; // ユーザ名

   }
      
   public partial class DebugRequirementLevelLoginAPIRequest : AppAPIRequestBase<DebugRequirementLevelLoginAPIPost, DebugRequirementLevelLoginAPIResponse> {
      public override string apiName{get{ return "debug/requirementLevelLogin"; } }
   }
}