//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザの称号更新

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserUpdateTitleAPIPost : AppAPIPostBase {
		public long mTitleId = 0; // 称号ID

   }

   [Serializable]
   public class UserUpdateTitleAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserUpdateTitleAPIRequest : AppAPIRequestBase<UserUpdateTitleAPIPost, UserUpdateTitleAPIResponse> {
      public override string apiName{get{ return "user/updateTitle"; } }
   }
}