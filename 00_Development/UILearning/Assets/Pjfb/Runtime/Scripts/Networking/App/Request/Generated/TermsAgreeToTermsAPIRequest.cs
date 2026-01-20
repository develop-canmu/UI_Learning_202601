//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
利用規約に同意する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TermsAgreeToTermsAPIPost : AppAPIPostBase {
		public string appToken = ""; // ログイン時に使用する生涯不変のトークン

   }

   [Serializable]
   public class TermsAgreeToTermsAPIResponse : AppAPIResponseBase {

   }
      
   public partial class TermsAgreeToTermsAPIRequest : AppAPIRequestBase<TermsAgreeToTermsAPIPost, TermsAgreeToTermsAPIResponse> {
      public override string apiName{get{ return "terms/agreeToTerms"; } }
   }
}