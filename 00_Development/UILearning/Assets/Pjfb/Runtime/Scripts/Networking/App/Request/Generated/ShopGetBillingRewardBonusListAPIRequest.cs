//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
課金パックの情報を取得する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopGetBillingRewardBonusListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class ShopGetBillingRewardBonusListAPIResponse : AppAPIResponseBase {
		public BillingRewardBonusDetail[] billingRewardBonusList = null; // 課金パック情報の配列
		public BillingRewardBonusRecommended recommendedBillingRewardBonus = null; // おすすめ課金パック情報
		public long freeBillingRewardBonusCount = 0; // 購入可能な無料課金パックの数（フッターのバッジ表示の判定に使う）
		public long paymentPenaltyLevel = 0; // 課金罰則レベル 0 => 無し, 998 => 警告, 999 => BAN

   }
      
   public partial class ShopGetBillingRewardBonusListAPIRequest : AppAPIRequestBase<ShopGetBillingRewardBonusListAPIPost, ShopGetBillingRewardBonusListAPIResponse> {
      public override string apiName{get{ return "shop/getBillingRewardBonusList"; } }
   }
}