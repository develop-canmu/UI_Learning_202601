//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
キャラの育成操作

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaGrowthAPIPost : AppAPIPostBase {
		public long uCharaId = 0; // 強化を行いたいキャラのID
		public long level = 0; // 成長後のレベル指定

   }

   [Serializable]
   public class CharaGrowthAPIResponse : AppAPIResponseBase {
		public DeckBase[] deckList = null; // 更新が生じたデッキ一覧

   }
      
   public partial class CharaGrowthAPIRequest : AppAPIRequestBase<CharaGrowthAPIPost, CharaGrowthAPIResponse> {
      public override string apiName{get{ return "chara/growth"; } }
   }
}