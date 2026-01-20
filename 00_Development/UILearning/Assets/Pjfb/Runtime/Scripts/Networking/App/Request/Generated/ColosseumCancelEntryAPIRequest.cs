//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルドエントリーキャンセル

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumCancelEntryAPIPost : AppAPIPostBase {
		public long mColosseumEventId = 0; // m_colosseum_eventのid
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumCancelEntryAPIResponse : AppAPIResponseBase {
		public long[] entryMColosseumEventIdList = null; // エントリー中のmColosseumEventのidリスト
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumCancelEntryAPIRequest : AppAPIRequestBase<ColosseumCancelEntryAPIPost, ColosseumCancelEntryAPIResponse> {
      public override string apiName{get{ return "colosseum/cancelEntry"; } }
   }
}