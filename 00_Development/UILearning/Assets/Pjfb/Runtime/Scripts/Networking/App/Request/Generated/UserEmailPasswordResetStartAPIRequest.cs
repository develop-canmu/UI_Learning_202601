//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
メールアドレス経由でのパスワード再設定、開始

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserEmailPasswordResetStartAPIPost : AppAPIPostBase {
		public string mailAddress = ""; // メールアドレス

   }

   [Serializable]
   public class UserEmailPasswordResetStartAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserEmailPasswordResetStartAPIRequest : AppAPIRequestBase<UserEmailPasswordResetStartAPIPost, UserEmailPasswordResetStartAPIResponse> {
      public override string apiName{get{ return "user/emailPasswordResetStart"; } }
   }
}