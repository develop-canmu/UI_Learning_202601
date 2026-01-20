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
   public class ColosseumGetHistoryListAPIPost : AppAPIPostBase {
		public long mColosseumEventId = 0; // イベントID
		public long sColosseumEventId = 0; // シーズンID（シーズンIDを指定されていた場合は、こちらを優先する）
		public long lastId = 0; // 確認していた履歴の末尾ID。-1の場合は先頭から取得
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetHistoryListAPIResponse : AppAPIResponseBase {
		public ColosseumHistory[] historyList = null; // 対戦履歴
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetHistoryListAPIRequest : AppAPIRequestBase<ColosseumGetHistoryListAPIPost, ColosseumGetHistoryListAPIResponse> {
      public override string apiName{get{ return "colosseum/getHistoryList"; } }
   }
}