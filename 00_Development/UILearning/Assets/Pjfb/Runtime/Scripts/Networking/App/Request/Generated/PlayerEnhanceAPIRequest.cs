//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
強化

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class PlayerEnhanceAPIPost : AppAPIPostBase {
		public long mPlayerEnhanceId = 0; // プレイヤー強化ID
		public long level = 0; // 強化後のレベル

   }

   [Serializable]
   public class PlayerEnhanceAPIResponse : AppAPIResponseBase {
		public PlayerEnhanceData playerEnhance = null; // 強化レベル情報

   }
      
   public partial class PlayerEnhanceAPIRequest : AppAPIRequestBase<PlayerEnhanceAPIPost, PlayerEnhanceAPIResponse> {
      public override string apiName{get{ return "player/enhance"; } }
   }
}