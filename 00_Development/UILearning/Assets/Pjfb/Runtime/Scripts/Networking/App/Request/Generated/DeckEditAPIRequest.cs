//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
デッキ編成

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DeckEditAPIPost : AppAPIPostBase {
		public long partyNumber = 0; // 編成番号
		public WrapperIntList[] typeIdList = null; // 編成に組み込むキャラ指定。 [type, id, roleNumber]を列挙した2次元配列。type = 1のときu_chara, 2のときu_chara_variable, 4のときu_chara_variable_trainer。roleNumberは不要な区分では省略可能
		public long optionValue = 0; // オプション値
		public long useType = 0; // 用途番号。0以外を指定すれば同時にデッキ選択も行う。デッキ選択を同時に行わない場合は0を指定する

   }

   [Serializable]
   public class DeckEditAPIResponse : AppAPIResponseBase {
		public DeckBase[] deckList = null; // 更新が生じたデッキ一覧

   }
      
   public partial class DeckEditAPIRequest : AppAPIRequestBase<DeckEditAPIPost, DeckEditAPIResponse> {
      public override string apiName{get{ return "deck/edit"; } }
   }
}