//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
履歴のデッキ情報。取得ができない場合はnullが帰る。
getHistoryListでとる一覧には、デッキ情報を内包していないため、必要であればこちらを実行して取得する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleReserveFormationGetHistoryDeckAPIPost : AppAPIPostBase {
		public long id = 0; // uBattleReserveFormationHistoryId

   }

   [Serializable]
   public class BattleReserveFormationGetHistoryDeckAPIResponse : AppAPIResponseBase {
		public ColosseumDeck deck = null; // 指定の相手のデッキ

   }
      
   public partial class BattleReserveFormationGetHistoryDeckAPIRequest : AppAPIRequestBase<BattleReserveFormationGetHistoryDeckAPIPost, BattleReserveFormationGetHistoryDeckAPIResponse> {
      public override string apiName{get{ return "battle-reserve-formation/getHistoryDeck"; } }
   }
}