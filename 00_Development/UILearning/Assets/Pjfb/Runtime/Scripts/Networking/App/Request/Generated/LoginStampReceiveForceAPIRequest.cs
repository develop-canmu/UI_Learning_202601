//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
受け取り（強制実行）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class LoginStampReceiveForceAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class LoginStampReceiveForceAPIResponse : AppAPIResponseBase {
		public LoginStampReceiveResult[] resultList = null; // ログインボーナス受け取り結果

   }
      
   public partial class LoginStampReceiveForceAPIRequest : AppAPIRequestBase<LoginStampReceiveForceAPIPost, LoginStampReceiveForceAPIResponse> {
      public override string apiName{get{ return "login-stamp/receiveForce"; } }
   }
}