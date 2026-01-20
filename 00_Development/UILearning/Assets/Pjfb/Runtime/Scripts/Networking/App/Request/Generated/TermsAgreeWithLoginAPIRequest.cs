//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
規約に同意する（ログイン状態で実施）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TermsAgreeWithLoginAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TermsAgreeWithLoginAPIResponse : AppAPIResponseBase {

   }
      
   public partial class TermsAgreeWithLoginAPIRequest : AppAPIRequestBase<TermsAgreeWithLoginAPIPost, TermsAgreeWithLoginAPIResponse> {
      public override string apiName{get{ return "terms/agreeWithLogin"; } }
   }
}