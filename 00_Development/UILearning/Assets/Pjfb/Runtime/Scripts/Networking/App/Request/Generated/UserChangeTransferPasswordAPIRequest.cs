//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザデータ引き継ぎ用パスワード変更

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserChangeTransferPasswordAPIPost : AppAPIPostBase {
		public string newPassword = ""; // 変更後のパスワード

   }

   [Serializable]
   public class UserChangeTransferPasswordAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserChangeTransferPasswordAPIRequest : AppAPIRequestBase<UserChangeTransferPasswordAPIPost, UserChangeTransferPasswordAPIResponse> {
      public override string apiName{get{ return "user/changeTransferPassword"; } }
   }
}