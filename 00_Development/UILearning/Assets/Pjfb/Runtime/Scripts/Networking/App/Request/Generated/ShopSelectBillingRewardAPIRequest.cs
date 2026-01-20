//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
課金パックを選択する。また、その課金パックを購入可能かどうかのバリデーションも行う。
これを呼び出した直後に native-api/shop/buyBillingReward を呼び出すことが前提となる。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopSelectBillingRewardAPIPost : AppAPIPostBase {
		public long mBillingRewardId = 0; // 課金商品ID
		public long mBillingRewardBonusId = 0; // 課金パックID

   }

   [Serializable]
   public class ShopSelectBillingRewardAPIResponse : AppAPIResponseBase {

   }
      
   public partial class ShopSelectBillingRewardAPIRequest : AppAPIRequestBase<ShopSelectBillingRewardAPIPost, ShopSelectBillingRewardAPIResponse> {
      public override string apiName{get{ return "shop/selectBillingReward"; } }
   }
}