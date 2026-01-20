//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザ名更新

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserUpdateNameAPIPost : AppAPIPostBase {
		public string name = ""; // 変更後のユーザ名

   }

   [Serializable]
   public class UserUpdateNameAPIResponse : AppAPIResponseBase {
		public string name = ""; // 変更後のユーザ名（実際にDBに登録された値）

   }
      
   public partial class UserUpdateNameAPIRequest : AppAPIRequestBase<UserUpdateNameAPIPost, UserUpdateNameAPIResponse> {
      public override string apiName{get{ return "user/updateName"; } }
   }
}