//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザデータ引き継ぎ

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserTransferAPIPost : AppAPIPostBase {
		public string loginId = ""; // データ引き継ぎ用のログインID (u_web_account.loginId)。もしくはメールアドレス
		public string password = ""; // データ引き継ぎ用のパスワード
		public DeviceDeviceInfo deviceInfo = null; // デバイス情報

   }

   [Serializable]
   public class UserTransferAPIResponse : AppAPIResponseBase {
		public string appToken = ""; // ログイン時に使用する生涯不変のトークン

   }
      
   public partial class UserTransferAPIRequest : AppAPIRequestBase<UserTransferAPIPost, UserTransferAPIResponse> {
      public override string apiName{get{ return "user/transfer"; } }
   }
}