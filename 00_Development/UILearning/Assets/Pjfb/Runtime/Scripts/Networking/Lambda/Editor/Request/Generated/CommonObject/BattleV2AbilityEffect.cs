//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2AbilityEffect {
		public long mElementId { get; set; } = 0; // スキル属性
		public long targetType { get; set; } = 0; // ターゲットタイプ
		public long targetNumber { get; set; } = 0; // 対象人数（ランダムの場合のみ有効）
		public long effectType { get; set; } = 0; // 対象区分
		public long statusEffectType { get; set; } = 0; // 効果区分
		public long invokeRateAll { get; set; } = 0; // この効果自体の発動成功率
		public long invokeRate { get; set; } = 0; // 発動率
		public long criticalRate { get; set; } = 0; // クリティカル率
		public long powerRate { get; set; } = 0; // 威力倍率
		public long powerValue { get; set; } = 0; // 威力固定値
		public long turnCount { get; set; } = 0; // 持続ターン数
		public long additionInvokeRate { get; set; } = 0; // レベルによる増加発動率
		public long additionPowerRate { get; set; } = 0; // スキルレベルによる追加威力倍率補正の基礎値
		public long additionPowerValue { get; set; } = 0; // スキルレベルによる追加威力固定値補正の基礎値
		public long additionTurnCount { get; set; } = 0; // スキルレベルによる追加持続ターン数補正の基礎値
		public long animationType { get; set; } = 0; // スキルヒット時にヒット対象に表示するアニメーションのタイプ
		public long isOmitSameAnimation { get; set; } = 0; // 同じanimationTypeが続く時に演出を省略するか否か
		public long selfAnimationType { get; set; } = 0; // スキル発動時に発動者自身に表示するアニメーションのタイプ
		public long isWholeAnimation { get; set; } = 0; // 全体攻撃のアニメーションを画面全体に出すか（animationTypeにのみ適用）
		public long soundType { get; set; } = 0; // 音声区分
		public long[] motionTypeList { get; set; } = null; // スキル発動時に再生するモーション番号の一覧
		public long[] receiveMotionTypeList { get; set; } = null; // スキルを受けた側が再生するモーション番号の一覧
		public long turnDecrementTiming { get; set; } = 0; // 残りターン数の減算タイミング
		public string invokeCondition { get; set; } = ""; // 発動条件
		public string applyingCondition { get; set; } = ""; // 申請条件
		public long targetStatusType { get; set; } = 0; // ステータス種別

   }
   
}

#endif