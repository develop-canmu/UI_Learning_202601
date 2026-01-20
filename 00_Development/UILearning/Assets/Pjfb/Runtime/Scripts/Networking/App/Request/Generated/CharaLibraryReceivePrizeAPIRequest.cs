//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
報酬の受け取り操作

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaLibraryReceivePrizeAPIPost : AppAPIPostBase {
		public long[] parentMCharaIdList = null; // 親キャラID一覧

   }

   [Serializable]
   public class CharaLibraryReceivePrizeAPIResponse : AppAPIResponseBase {
		public bool hasTrustPrize = false; // 未受け取りのキャラ信頼度報酬があれば真

   }
      
   public partial class CharaLibraryReceivePrizeAPIRequest : AppAPIRequestBase<CharaLibraryReceivePrizeAPIPost, CharaLibraryReceivePrizeAPIResponse> {
      public override string apiName{get{ return "chara-library/receivePrize"; } }
   }
}