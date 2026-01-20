//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
コロシアムのシーズン開催情報を得る。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetHomeDataAPIPost : AppAPIPostBase {
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetHomeDataAPIResponse : AppAPIResponseBase {
		public ColosseumHomeData colosseum = null; // colosseumの基本情報
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetHomeDataAPIRequest : AppAPIRequestBase<ColosseumGetHomeDataAPIPost, ColosseumGetHomeDataAPIResponse> {
      public override string apiName{get{ return "colosseum/getHomeData"; } }
   }
}