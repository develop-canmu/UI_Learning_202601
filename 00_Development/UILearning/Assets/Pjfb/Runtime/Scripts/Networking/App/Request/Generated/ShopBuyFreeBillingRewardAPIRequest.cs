//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
無料の課金パックを購入する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopBuyFreeBillingRewardAPIPost : AppAPIPostBase {
		public long mBillingRewardId = 0; // 課金商品ID
		public long mBillingRewardBonusId = 0; // 課金パックID

   }

   [Serializable]
   public class ShopBuyFreeBillingRewardAPIResponse : AppAPIResponseBase {

   }
      
   public partial class ShopBuyFreeBillingRewardAPIRequest : AppAPIRequestBase<ShopBuyFreeBillingRewardAPIPost, ShopBuyFreeBillingRewardAPIResponse> {
      public override string apiName{get{ return "shop/buyFreeBillingReward"; } }
   }
}