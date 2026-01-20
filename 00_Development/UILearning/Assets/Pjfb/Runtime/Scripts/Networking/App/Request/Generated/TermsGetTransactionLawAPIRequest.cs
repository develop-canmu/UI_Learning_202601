//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
特定商取引法に基づく表示を取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TermsGetTransactionLawAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TermsGetTransactionLawAPIResponse : AppAPIResponseBase {
		public string transactionLaw = ""; // 特定商取引法に基づく表示の疑似HTML

   }
      
   public partial class TermsGetTransactionLawAPIRequest : AppAPIRequestBase<TermsGetTransactionLawAPIPost, TermsGetTransactionLawAPIResponse> {
      public override string apiName{get{ return "terms/getTransactionLaw"; } }
   }
}