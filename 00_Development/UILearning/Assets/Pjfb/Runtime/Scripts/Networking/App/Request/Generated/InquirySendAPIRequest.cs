//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
問い合わせ送信（非ログイン時）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class InquirySendAPIPost : AppAPIPostBase {
		public string appToken = ""; // ログイン時に使用する生涯不変のトークン（ユーザ未作成の場合は未指定で良い）
		public string subject = ""; // 問い合わせカテゴリ名
		public string body = ""; // ユーザが入力した問い合わせ本文
		public string mailFrom = ""; // ユーザのメールアドレス
		public DeviceDeviceInfo deviceInfo = null; // デバイス情報

   }

   [Serializable]
   public class InquirySendAPIResponse : AppAPIResponseBase {

   }
      
   public partial class InquirySendAPIRequest : AppAPIRequestBase<InquirySendAPIPost, InquirySendAPIResponse> {
      public override string apiName{get{ return "inquiry/send"; } }
   }
}