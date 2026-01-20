//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
保護者の同意を得た状態に更新する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopUpdateParentalConsentAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class ShopUpdateParentalConsentAPIResponse : AppAPIResponseBase {
		public bool hasParentalConsent = false; // 保護者の同意を得ているなら真

   }
      
   public partial class ShopUpdateParentalConsentAPIRequest : AppAPIRequestBase<ShopUpdateParentalConsentAPIPost, ShopUpdateParentalConsentAPIResponse> {
      public override string apiName{get{ return "shop/updateParentalConsent"; } }
   }
}