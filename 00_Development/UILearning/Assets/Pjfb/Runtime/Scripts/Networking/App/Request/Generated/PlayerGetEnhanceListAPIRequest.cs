//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
強化情報一覧の取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class PlayerGetEnhanceListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class PlayerGetEnhanceListAPIResponse : AppAPIResponseBase {
		public PlayerEnhanceData[] playerEnhanceList = null; // 強化レベル情報一覧

   }
      
   public partial class PlayerGetEnhanceListAPIRequest : AppAPIRequestBase<PlayerGetEnhanceListAPIPost, PlayerGetEnhanceListAPIResponse> {
      public override string apiName{get{ return "player/getEnhanceList"; } }
   }
}