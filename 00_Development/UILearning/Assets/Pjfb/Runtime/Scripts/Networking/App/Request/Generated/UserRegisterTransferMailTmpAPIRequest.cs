//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザー引き継ぎ用メールアドレス仮登録

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserRegisterTransferMailTmpAPIPost : AppAPIPostBase {
		public string mailAddress = ""; // メールアドレス

   }

   [Serializable]
   public class UserRegisterTransferMailTmpAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserRegisterTransferMailTmpAPIRequest : AppAPIRequestBase<UserRegisterTransferMailTmpAPIPost, UserRegisterTransferMailTmpAPIResponse> {
      public override string apiName{get{ return "user/registerTransferMailTmp"; } }
   }
}