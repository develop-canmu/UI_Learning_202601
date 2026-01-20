//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザー引き継ぎ用仮登録メールアドレス認証

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserAuthTransferMailAPIPost : AppAPIPostBase {
		public string authCode = ""; // 認証コード

   }

   [Serializable]
   public class UserAuthTransferMailAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserAuthTransferMailAPIRequest : AppAPIRequestBase<UserAuthTransferMailAPIPost, UserAuthTransferMailAPIResponse> {
      public override string apiName{get{ return "user/authTransferMail"; } }
   }
}