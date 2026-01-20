//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ワンタイムトークンを発行する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserGenerateOneTimeTokenAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class UserGenerateOneTimeTokenAPIResponse : AppAPIResponseBase {
		public string oneTimeToken = ""; // ワンタイムトークン

   }
      
   public partial class UserGenerateOneTimeTokenAPIRequest : AppAPIRequestBase<UserGenerateOneTimeTokenAPIPost, UserGenerateOneTimeTokenAPIResponse> {
      public override string apiName{get{ return "user/generateOneTimeToken"; } }
   }
}