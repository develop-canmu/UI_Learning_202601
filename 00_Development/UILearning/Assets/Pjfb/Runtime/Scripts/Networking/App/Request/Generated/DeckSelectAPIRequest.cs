//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
使用デッキの選択

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DeckSelectAPIPost : AppAPIPostBase {
		public long useType = 0; // 用途番号
		public long partyNumber = 0; // 編成番号

   }

   [Serializable]
   public class DeckSelectAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DeckSelectAPIRequest : AppAPIRequestBase<DeckSelectAPIPost, DeckSelectAPIResponse> {
      public override string apiName{get{ return "deck/select"; } }
   }
}