//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザ作成

ユーザ作成時にユーザ名や性別を入力しないタイトルについては、
name と gender にどんな値を指定したとしても無視され、ユーザデータには仮の初期値が入ります。
そのようなタイトルでは正式なユーザ名および性別が確定したタイミングで native-api/user/initialize を実行してください。

ユーザ作成時にユーザ名や性別を入力するかどうかは下記の設定値で設定します。
<a href="/setting/searchResult?keyword=OnUserCreation&type=1">shouldInputUserNameOnUserCreation, shouldInputGenderOnUserCreation</a>

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserCreateAPIPost : AppAPIPostBase {
		public string name = ""; // ユーザ名
		public long gender = 0; // 性別（1: 男性, 2: 女性）
		public long ignoreLogin = 0; // ついでにログインする挙動をしない（1 => しない、 0 => する）。デフォルト0
		public string lastTermsAgreementAt = ""; // 最終規約同意日時
		public DeviceDeviceInfo deviceInfo = null; // デバイス情報

   }

   [Serializable]
   public class UserCreateAPIResponse : AppAPIResponseBase {
		public string appToken = ""; // ログイン時に使用する生涯不変のトークン（ユーザ作成に失敗した場合は null）
		public string sessionId = ""; // セッションID（ignoreLogin時はなし）
		public string loginId = ""; // ログインID（ignoreLogin時はなし）
		public long uMasterId = 0; // ユーザーID

   }
      
   public partial class UserCreateAPIRequest : AppAPIRequestBase<UserCreateAPIPost, UserCreateAPIResponse> {
      public override string apiName{get{ return "user/create"; } }
   }
}