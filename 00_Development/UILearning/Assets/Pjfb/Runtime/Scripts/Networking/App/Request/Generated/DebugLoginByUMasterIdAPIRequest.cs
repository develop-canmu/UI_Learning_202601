//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
uMasterId 指定でログインする（開発環境以外では許可IPであっても実行不可）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugLoginByUMasterIdAPIPost : AppAPIPostBase {
		public string uMasterId = ""; // ユーザID

   }

   [Serializable]
   public class DebugLoginByUMasterIdAPIResponse : AppAPIResponseBase {
		public string sessionId = ""; // セッションID
		public string loginId = ""; // ログインID
		public string lastLoginAt = ""; // ユーザ最終ログイン日時
		public string createdAt = ""; // ユーザ作成日時

   }
      
   public partial class DebugLoginByUMasterIdAPIRequest : AppAPIRequestBase<DebugLoginByUMasterIdAPIPost, DebugLoginByUMasterIdAPIResponse> {
      public override string apiName{get{ return "debug/loginByUMasterId"; } }
   }
}