//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
メールアドレス経由でのパスワード再設定、完了

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserEmailPasswordResetFinishAPIPost : AppAPIPostBase {
		public string mailAddress = ""; // メールアドレス
		public string password = ""; // データ引き継ぎ用のパスワード
		public string authCode = ""; // 認証コード
		public DeviceDeviceInfo deviceInfo = null; // デバイス情報

   }

   [Serializable]
   public class UserEmailPasswordResetFinishAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserEmailPasswordResetFinishAPIRequest : AppAPIRequestBase<UserEmailPasswordResetFinishAPIPost, UserEmailPasswordResetFinishAPIResponse> {
      public override string apiName{get{ return "user/emailPasswordResetFinish"; } }
   }
}