//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
デッキ一覧の取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DeckGetListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DeckGetListAPIResponse : AppAPIResponseBase {
		public DeckBase[] deckList = null; // デッキ一蘭
		public WrapperIntList[] useTypePartyNumberList = null; // [用途番号・partyNumber]が格納された配列を返却します。

   }
      
   public partial class DeckGetListAPIRequest : AppAPIRequestBase<DeckGetListAPIPost, DeckGetListAPIResponse> {
      public override string apiName{get{ return "deck/getList"; } }
   }
}