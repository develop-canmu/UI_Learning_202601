//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
資金決済法に基づく表示を取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TermsGetPaymentLawAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TermsGetPaymentLawAPIResponse : AppAPIResponseBase {
		public string paymentLaw = ""; // 資金決済法に基づく表示の疑似HTML

   }
      
   public partial class TermsGetPaymentLawAPIRequest : AppAPIRequestBase<TermsGetPaymentLawAPIPost, TermsGetPaymentLawAPIResponse> {
      public override string apiName{get{ return "terms/getPaymentLaw"; } }
   }
}