//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
キャラ図鑑画面に移動して受け取れる物があるかどうかを返す（最終、別APIに統合だと思う）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaLibraryHasTrustPrizeAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CharaLibraryHasTrustPrizeAPIResponse : AppAPIResponseBase {
		public bool hasTrustPrize = false; // キャラ図鑑画面に移動して受け取れる物があるかどうか

   }
      
   public partial class CharaLibraryHasTrustPrizeAPIRequest : AppAPIRequestBase<CharaLibraryHasTrustPrizeAPIPost, CharaLibraryHasTrustPrizeAPIResponse> {
      public override string apiName{get{ return "chara-library/hasTrustPrize"; } }
   }
}