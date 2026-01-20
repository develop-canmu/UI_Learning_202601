//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ログイン

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserLoginAPIPost : AppAPIPostBase {
		public string appToken = ""; // ログイン時に使用する生涯不変のトークン

   }

   [Serializable]
   public class UserLoginAPIResponse : AppAPIResponseBase {
		public string sessionId = ""; // セッションID
		public string loginId = ""; // ログインID
		public string lastLoginAt = ""; // ユーザ最終ログイン日時
		public string createdAt = ""; // ユーザ作成日時

   }
      
   public partial class UserLoginAPIRequest : AppAPIRequestBase<UserLoginAPIPost, UserLoginAPIResponse> {
      public override string apiName{get{ return "user/login"; } }
   }
}