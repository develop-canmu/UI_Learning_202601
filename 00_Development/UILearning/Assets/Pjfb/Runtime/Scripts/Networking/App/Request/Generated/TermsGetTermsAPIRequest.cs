//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
利用規約を取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TermsGetTermsAPIPost : AppAPIPostBase {
		public string appToken = ""; // ログイン時に使用する生涯不変のトークン（同意済みかどうかの判定が不要なら指定不要）

   }

   [Serializable]
   public class TermsGetTermsAPIResponse : AppAPIResponseBase {
		public bool hasAgreed = false; // 既に同意済みなら真
		public string serviceTerms = ""; // 利用規約の疑似HTML（既に同意済みなら空文字列）
		public string privacyPolicy = ""; // プライバシーポリシーの疑似HTML（既に同意済みなら空文字列）
		public string paymentLaw = ""; // 資金決済法に基づく表示の疑似HTML（既に同意済みなら空文字列）

   }
      
   public partial class TermsGetTermsAPIRequest : AppAPIRequestBase<TermsGetTermsAPIPost, TermsGetTermsAPIResponse> {
      public override string apiName{get{ return "terms/getTerms"; } }
   }
}