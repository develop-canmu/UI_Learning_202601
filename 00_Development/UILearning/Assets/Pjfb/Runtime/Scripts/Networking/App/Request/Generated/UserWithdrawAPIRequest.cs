//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザ退会

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserWithdrawAPIPost : AppAPIPostBase {
		public string appToken = ""; // ログイン時に使用する生涯不変のトークン

   }

   [Serializable]
   public class UserWithdrawAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserWithdrawAPIRequest : AppAPIRequestBase<UserWithdrawAPIPost, UserWithdrawAPIResponse> {
      public override string apiName{get{ return "user/withdraw"; } }
   }
}