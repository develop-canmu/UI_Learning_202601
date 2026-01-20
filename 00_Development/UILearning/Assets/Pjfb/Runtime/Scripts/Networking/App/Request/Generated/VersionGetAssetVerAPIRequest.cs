//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
assetVer の情報を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class VersionGetAssetVerAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class VersionGetAssetVerAPIResponse : AppAPIResponseBase {
		public long assetVer = 0; // 最新アセットバージョン

   }
      
   public partial class VersionGetAssetVerAPIRequest : AppAPIRequestBase<VersionGetAssetVerAPIPost, VersionGetAssetVerAPIResponse> {
      public override string apiName{get{ return "version/getAssetVer"; } }
   }
}