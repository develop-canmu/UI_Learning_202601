//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザデータ引き継ぎ設定情報取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserGetTransferSettingAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class UserGetTransferSettingAPIResponse : AppAPIResponseBase {
		public string loginId = ""; // データ引き継ぎ用のログインID (u_web_account.loginId)
		public string hasPassword = ""; // データ引き継ぎ用のパスワードが設定済みなら真
		public string mailAddress = ""; // 引き継ぎ用メールアドレス。認証していない場合はnull

   }
      
   public partial class UserGetTransferSettingAPIRequest : AppAPIRequestBase<UserGetTransferSettingAPIPost, UserGetTransferSettingAPIResponse> {
      public override string apiName{get{ return "user/getTransferSetting"; } }
   }
}