//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2BattleLog {
		public long result = 0; // 試合の結果（1=> 勝利, 2 => 敗北, 3 => 引き分け）
		public long pointGet = 0; // $pointGet 総得点
		public long pointLost = 0; // $pointLost 総失点
		public ColosseumDeck[] deckList = null; // $deckList 0index目に自身のデッキ、1index目に相手のデッキを格納する
		public DeckTirednessConditionDebug tirednessCondition = null; // デッキの疲労度状態記録
		public long opponentDebuffRate = 0; // 対戦相手のデバフ値
		public long groupScore = 0; // グループスコア
		public string bonusRankList = ""; // 順位ボーナスリスト
		public long deckTiredness = 0; // 戦闘時のデッキ疲労度
		public string battleKey = ""; // 一部の区分で使う、バトルログ識別用のキー

   }
   
}