//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
問い合わせ送信（ログイン時）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class InquirySendDuringLoginAPIPost : AppAPIPostBase {
		public string subject = ""; // 問い合わせカテゴリ名
		public string body = ""; // ユーザが入力した問い合わせ本文
		public string mailFrom = ""; // ユーザのメールアドレス
		public DeviceDeviceInfo deviceInfo = null; // デバイス情報

   }

   [Serializable]
   public class InquirySendDuringLoginAPIResponse : AppAPIResponseBase {

   }
      
   public partial class InquirySendDuringLoginAPIRequest : AppAPIRequestBase<InquirySendDuringLoginAPIPost, InquirySendDuringLoginAPIResponse> {
      public override string apiName{get{ return "inquiry/sendDuringLogin"; } }
   }
}