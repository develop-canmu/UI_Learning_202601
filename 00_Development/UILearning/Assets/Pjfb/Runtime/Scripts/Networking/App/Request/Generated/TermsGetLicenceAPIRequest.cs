//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ライセンス表記を取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class TermsGetLicenceAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TermsGetLicenceAPIResponse : AppAPIResponseBase {
		public string licence = ""; // ライセンス表記の疑似HTML

   }
      
   public partial class TermsGetLicenceAPIRequest : AppAPIRequestBase<TermsGetLicenceAPIPost, TermsGetLicenceAPIResponse> {
      public override string apiName{get{ return "terms/getLicence"; } }
   }
}