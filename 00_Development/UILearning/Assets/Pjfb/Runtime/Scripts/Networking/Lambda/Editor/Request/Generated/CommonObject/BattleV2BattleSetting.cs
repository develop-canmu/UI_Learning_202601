//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2BattleSetting {
		public long recoveryTurnCount { get; set; } = 0; // mBattleGamelift.type=2で使用：回復するターン周期
		public long recoveryMilitaryStrengthPerTurn { get; set; } = 0; // mBattleGamelift.type=2で使用：ターン周期ごとの回復量
		public long maxMilitaryStrengthPerDeck { get; set; } = 0; // mBattleGamelift.type=2で使用：１デッキ当たりの戦力（ボール）設定最大数
		public long defaultMilitaryStrength { get; set; } = 0; // mBattleGamelift.type=2で使用：デフォルトで所持しているユーザーごとの戦力（ボール）数
		public long revivalTurn { get; set; } = 0; // mBattleGamelift.type=2で使用：敗北時復活基礎ターン数
		public long revivalTurnPenaltyPerBeaten { get; set; } = 0; // mBattleGamelift.type=2で使用：敗北時復活追加ターン数(敗北毎)
		public long itemRecoveryValue { get; set; } = 0; // mBattleGamelift.type=2で使用：バトル用アイテムの回復量
		public long itemCoolTime { get; set; } = 0; // mBattleGamelift.type=2で使用：バトル用アイテムのクールタイム

   }
   
}

#endif