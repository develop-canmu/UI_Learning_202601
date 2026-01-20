//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
履歴取得。ページング的に取得しない場合はlastIdは無視する。ページング的に取得する場合は「前回取得したリストのID」の中で最も古いものを選ぶ

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleReserveFormationGetHistoryListAPIPost : AppAPIPostBase {
		public long eventType = 0; // イベント種別
		public long eventId = 0; // イベント・シーズンのID
		public long lastId = 0; // 確認していた履歴の末尾ID。-1の場合は先頭から取得

   }

   [Serializable]
   public class BattleReserveFormationGetHistoryListAPIResponse : AppAPIResponseBase {
		public BattleReserveFormationHistory[] historyList = null; // 対戦履歴

   }
      
   public partial class BattleReserveFormationGetHistoryListAPIRequest : AppAPIRequestBase<BattleReserveFormationGetHistoryListAPIPost, BattleReserveFormationGetHistoryListAPIResponse> {
      public override string apiName{get{ return "battle-reserve-formation/getHistoryList"; } }
   }
}