//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ロック状態になっているデッキを判別する情報を取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DeckGetLockedListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DeckGetLockedListAPIResponse : AppAPIResponseBase {
		public DeckLocked[] lockedList = null; // ロック情報一覧

   }
      
   public partial class DeckGetLockedListAPIRequest : AppAPIRequestBase<DeckGetLockedListAPIPost, DeckGetLockedListAPIResponse> {
      public override string apiName{get{ return "deck/getLockedList"; } }
   }
}