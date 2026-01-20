//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
対戦相手一覧取得。
対戦可能な相手の一覧を表示する。サーバーサイドでのキャッシュ操作などは行わないため、クライアント側ではあまりにも連打されないための対策等はして欲しい。
この処理を実行しないと、attack等の操作は行えない。
スコア戦の場合は使わない

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetTargetListAPIPost : AppAPIPostBase {
		public long sColosseumEventId = 0; // シーズンのID（SeasonHome.idと同じもの）
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetTargetListAPIResponse : AppAPIResponseBase {
		public ColosseumTargetData targetData = null; // 攻撃対象リスト保存用構造体
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetTargetListAPIRequest : AppAPIRequestBase<ColosseumGetTargetListAPIPost, ColosseumGetTargetListAPIResponse> {
      public override string apiName{get{ return "colosseum/getTargetList"; } }
   }
}