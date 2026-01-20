//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
対戦前の画面等で表示する、相手の情報を得るAPI

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class PvpMimicGetPreparationDetailAPIPost : AppAPIPostBase {
		public long opponentUMasterId = 0; // 対戦相手のユーザーID

   }

   [Serializable]
   public class PvpMimicGetPreparationDetailAPIResponse : AppAPIResponseBase {
		public ColosseumDeck deck = null; // 指定の相手のデッキ

   }
      
   public partial class PvpMimicGetPreparationDetailAPIRequest : AppAPIRequestBase<PvpMimicGetPreparationDetailAPIPost, PvpMimicGetPreparationDetailAPIResponse> {
      public override string apiName{get{ return "pvp-mimic/getPreparationDetail"; } }
   }
}