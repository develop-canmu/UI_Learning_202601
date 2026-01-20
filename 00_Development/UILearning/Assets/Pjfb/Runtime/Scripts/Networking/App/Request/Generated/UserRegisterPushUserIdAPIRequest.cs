//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
プッシュ通知用のユーザーIDを登録

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserRegisterPushUserIdAPIPost : AppAPIPostBase {
		public string pushUserId = ""; // プッシュのユーザーID
		public long pushType = 0; // プッシュの種別

   }

   [Serializable]
   public class UserRegisterPushUserIdAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserRegisterPushUserIdAPIRequest : AppAPIRequestBase<UserRegisterPushUserIdAPIPost, UserRegisterPushUserIdAPIResponse> {
      public override string apiName{get{ return "user/registerPushUserId"; } }
   }
}