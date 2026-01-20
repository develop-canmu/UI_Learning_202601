//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
デッキ名称変更

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DeckNameChangeAPIPost : AppAPIPostBase {
		public long partyNumber = 0; // 編成番号
		public string name = ""; // 名前

   }

   [Serializable]
   public class DeckNameChangeAPIResponse : AppAPIResponseBase {
		public DeckBase[] deckList = null; // 更新が生じたデッキ一覧

   }
      
   public partial class DeckNameChangeAPIRequest : AppAPIRequestBase<DeckNameChangeAPIPost, DeckNameChangeAPIResponse> {
      public override string apiName{get{ return "deck/nameChange"; } }
   }
}