//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
攻撃を実施する。
ただし、getTargetListアクション経由で、攻撃対象として上がっている対象以外への攻撃は行えない。
報酬などを獲得する場合があるので、それらのデータはitemContainerやprizeJsonList等経由で受け取る。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumAttackAPIPost : AppAPIPostBase {
		public long sColosseumEventId = 0; // シーズンのID（SeasonHome.idと同じもの）
		public long opponentUserType = 0; // 対戦相手のユーザー種別（1 => 通常、2 => NPC）
		public long opponentUserId = 0; // 対象ユーザーID（uMasterId, m_colosseum_npc.id）
		public BattleV2BattleLog battleLog = null; // バトルの保存するログ情報
		public long opponentRanking = 0; // 対戦相手の順位（閲覧時から変動したときの対策用）
		public long costTypeForValidation = 0; // 画面表示段階での消費コスト区分。0の場合は検証しない、1はスタミナ、2はポイント
		public long costValueForValidation = 0; // 画面表示段階での消費コスト量。typeが0の場合は参照しない
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumAttackAPIResponse : AppAPIResponseBase {
		public ColosseumUserSeasonStatus userSeasonStatus = null; // ユーザーのシーズン情報
		public ColosseumHistoryLite historyLite = null; // バトル結果の一部切り出し
		public ColosseumDailyStatus dailyStatus = null; // 日毎の状態
		public BattleChangeResultBase battleChangeResult = null; // バトルでの状態変動（gvg方式の場合のみ存在を保証）
		public DeckBase[] deckList = null; // 更新が生じたデッキ一覧
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumAttackAPIRequest : AppAPIRequestBase<ColosseumAttackAPIPost, ColosseumAttackAPIResponse> {
      public override string apiName{get{ return "colosseum/attack"; } }
   }
}