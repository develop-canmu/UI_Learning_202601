//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2BattleSetting {
		public long recoveryTurnCount = 0; // mBattleGamelift.type=2で使用：回復するターン周期
		public long recoveryMilitaryStrengthPerTurn = 0; // mBattleGamelift.type=2で使用：ターン周期ごとの回復量
		public long maxMilitaryStrengthPerDeck = 0; // mBattleGamelift.type=2で使用：１デッキ当たりの戦力（ボール）設定最大数
		public long defaultMilitaryStrength = 0; // mBattleGamelift.type=2で使用：デフォルトで所持しているユーザーごとの戦力（ボール）数
		public long revivalTurn = 0; // mBattleGamelift.type=2で使用：敗北時復活基礎ターン数
		public long revivalTurnPenaltyPerBeaten = 0; // mBattleGamelift.type=2で使用：敗北時復活追加ターン数(敗北毎)
		public long itemRecoveryValue = 0; // mBattleGamelift.type=2で使用：バトル用アイテムの回復量
		public long itemCoolTime = 0; // mBattleGamelift.type=2で使用：バトル用アイテムのクールタイム

   }
   
}