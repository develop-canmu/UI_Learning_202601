//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
キャラの潜在解放操作

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaLiberationAPIPost : AppAPIPostBase {
		public long uCharaId = 0; // 強化を行いたいキャラのID
		public long level = 0; // 成長後のレベル指定

   }

   [Serializable]
   public class CharaLiberationAPIResponse : AppAPIResponseBase {
		public DeckBase[] deckList = null; // 更新が生じたデッキ一覧

   }
      
   public partial class CharaLiberationAPIRequest : AppAPIRequestBase<CharaLiberationAPIPost, CharaLiberationAPIResponse> {
      public override string apiName{get{ return "chara/liberation"; } }
   }
}