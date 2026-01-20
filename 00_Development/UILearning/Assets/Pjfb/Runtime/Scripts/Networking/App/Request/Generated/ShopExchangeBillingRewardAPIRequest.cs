//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ポイントを消費して課金パックを購入する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopExchangeBillingRewardAPIPost : AppAPIPostBase {
		public long mBillingRewardId = 0; // 課金商品ID
		public long mBillingRewardBonusId = 0; // 課金パックID
		public long mBillingRewardAlternativePointId = 0; // 課金商品引き換えポイント設定ID

   }

   [Serializable]
   public class ShopExchangeBillingRewardAPIResponse : AppAPIResponseBase {

   }
      
   public partial class ShopExchangeBillingRewardAPIRequest : AppAPIRequestBase<ShopExchangeBillingRewardAPIPost, ShopExchangeBillingRewardAPIResponse> {
      public override string apiName{get{ return "shop/exchangeBillingReward"; } }
   }
}