//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
シーズンIDとユーザーを指定し、獲得予定スコアの詳細を確認する（現状、デバッグ用想定）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetScorePlanedDetailAPIPost : AppAPIPostBase {
		public long sColosseumEventId = 0; // シーズンのID（SeasonHome.idと同じもの）
		public long targetUMasterId = 0; // 対象ユーザーID
		public long userType = 0; // ユーザー区分（1 => 通常、 2 => npc）
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetScorePlanedDetailAPIResponse : AppAPIResponseBase {
		public ColosseumUserSeasonStatus userSeasonStatus = null; // ユーザーのシーズン情報を得る
		public ColosseumScoreBattleTurn scoreBattleTurnList = null; // ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す
		public BattleChangeResultScorePlanedDetail scorePlanedDetail = null; // 獲得予定スコアの内訳を返す
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetScorePlanedDetailAPIRequest : AppAPIRequestBase<ColosseumGetScorePlanedDetailAPIPost, ColosseumGetScorePlanedDetailAPIResponse> {
      public override string apiName{get{ return "colosseum/getScorePlanedDetail"; } }
   }
}