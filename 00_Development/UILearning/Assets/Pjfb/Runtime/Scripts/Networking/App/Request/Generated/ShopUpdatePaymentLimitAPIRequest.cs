//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
生年月日を登録して月の課金上限額を確定する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopUpdatePaymentLimitAPIPost : AppAPIPostBase {
		public string birthday = ""; // yyyyMMdd

   }

   [Serializable]
   public class ShopUpdatePaymentLimitAPIResponse : AppAPIResponseBase {
		public bool hasParentalConsent = false; // 保護者の同意を得ているなら真
		public long monthPayment = 0; // 今月の課金額
		public long monthPaymentLimit = 0; // 月の課金額上限

   }
      
   public partial class ShopUpdatePaymentLimitAPIRequest : AppAPIRequestBase<ShopUpdatePaymentLimitAPIPost, ShopUpdatePaymentLimitAPIResponse> {
      public override string apiName{get{ return "shop/updatePaymentLimit"; } }
   }
}