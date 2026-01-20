//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
APIのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugTestAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugTestAPIResponse : AppAPIResponseBase {
		public string message = ""; // テストメッセージ
		public long uMasterId = 0; // ユーザID
		public string name = ""; // ユーザ名

   }
      
   public partial class DebugTestAPIRequest : AppAPIRequestBase<DebugTestAPIPost, DebugTestAPIResponse> {
      public override string apiName{get{ return "debug/test"; } }
   }
}