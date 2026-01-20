//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
受け取り。
1度実行すると、キャッシュに判定履歴が残り、しばらくは判定を行わずに空配列を返す。
クライアント側であまり叩かないようにする感じであれば、forceを使っても良い。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class LoginStampReceiveAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class LoginStampReceiveAPIResponse : AppAPIResponseBase {
		public LoginStampReceiveResult[] resultList = null; // ログインボーナス受け取り結果

   }
      
   public partial class LoginStampReceiveAPIRequest : AppAPIRequestBase<LoginStampReceiveAPIPost, LoginStampReceiveAPIResponse> {
      public override string apiName{get{ return "login-stamp/receive"; } }
   }
}