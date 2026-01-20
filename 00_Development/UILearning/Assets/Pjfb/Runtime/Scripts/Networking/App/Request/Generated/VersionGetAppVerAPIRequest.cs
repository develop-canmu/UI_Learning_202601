//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
appVer の情報を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class VersionGetAppVerAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class VersionGetAppVerAPIResponse : AppAPIResponseBase {
		public string minimum = ""; // 最低動作アプリバージョン
		public string latest = ""; // 最新アプリバージョン
		public string preview = ""; // 審査アプリバージョン

   }
      
   public partial class VersionGetAppVerAPIRequest : AppAPIRequestBase<VersionGetAppVerAPIPost, VersionGetAppVerAPIResponse> {
      public override string apiName{get{ return "version/getAppVer"; } }
   }
}