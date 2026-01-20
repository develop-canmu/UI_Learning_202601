//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2AbilityEffect {
		public long mElementId = 0; // スキル属性
		public long targetType = 0; // ターゲットタイプ
		public long targetNumber = 0; // 対象人数（ランダムの場合のみ有効）
		public long effectType = 0; // 対象区分
		public long statusEffectType = 0; // 効果区分
		public long invokeRateAll = 0; // この効果自体の発動成功率
		public long invokeRate = 0; // 発動率
		public long criticalRate = 0; // クリティカル率
		public long powerRate = 0; // 威力倍率
		public long powerValue = 0; // 威力固定値
		public long turnCount = 0; // 持続ターン数
		public long additionInvokeRate = 0; // レベルによる増加発動率
		public long additionPowerRate = 0; // スキルレベルによる追加威力倍率補正の基礎値
		public long additionPowerValue = 0; // スキルレベルによる追加威力固定値補正の基礎値
		public long additionTurnCount = 0; // スキルレベルによる追加持続ターン数補正の基礎値
		public long animationType = 0; // スキルヒット時にヒット対象に表示するアニメーションのタイプ
		public long isOmitSameAnimation = 0; // 同じanimationTypeが続く時に演出を省略するか否か
		public long selfAnimationType = 0; // スキル発動時に発動者自身に表示するアニメーションのタイプ
		public long isWholeAnimation = 0; // 全体攻撃のアニメーションを画面全体に出すか（animationTypeにのみ適用）
		public long soundType = 0; // 音声区分
		public long[] motionTypeList = null; // スキル発動時に再生するモーション番号の一覧
		public long[] receiveMotionTypeList = null; // スキルを受けた側が再生するモーション番号の一覧
		public long turnDecrementTiming = 0; // 残りターン数の減算タイミング
		public string invokeCondition = ""; // 発動条件
		public string applyingCondition = ""; // 申請条件
		public long targetStatusType = 0; // ステータス種別

   }
   
}