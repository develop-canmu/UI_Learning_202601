//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
リクエストの共通部に何も指定しなくていいAPIのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugRequirementLevelNothingAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugRequirementLevelNothingAPIResponse : AppAPIResponseBase {
		public string message = ""; // テストメッセージ

   }
      
   public partial class DebugRequirementLevelNothingAPIRequest : AppAPIRequestBase<DebugRequirementLevelNothingAPIPost, DebugRequirementLevelNothingAPIResponse> {
      public override string apiName{get{ return "debug/requirementLevelNothing"; } }
   }
}