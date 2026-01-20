//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2ClientData {
		public BattleV2Group[] groupList { get; set; } = null;
		public BattleV2Player[] playerList { get; set; } = null;
		public BattleV2Chara[] charaList { get; set; } = null;
		public BattleV2Ability[] abilityList { get; set; } = null;
		public SpotComboAbilitySpotComboAbilityInfo spotComboAbilityInfo { get; set; } = null; // クライアント側でその場でコンボスキルを発動させる際に必要な情報
		public SpotTacticsAbilitySpotTacticsAbilityInfo spotTacticsAbilityInfo { get; set; } = null; // クライアント側でその場で戦術スキルを発動させる際に必要な情報
		public BattleV2BattleFbKeeper[] battleFbKeeperList { get; set; } = null;
		public BattleV2BattleWarField battleWarField { get; set; } = null;
		public BattleV2BattleConquestField battleConquestField { get; set; } = null;
		public BattleV2Tactics[] tacticsList { get; set; } = null;
		public BattleV2UnitRole[] unitRoleList { get; set; } = null;
		public BattleV2UnitRoleOperation[] unitRoleOperationList { get; set; } = null;
		public long firstAttackIndex { get; set; } = 0; // 先制陣営指定(nullの場合は、デフォルトの挙動をしてもらう感じで)
		public long seedNum { get; set; } = 0; // ランダム数値
		public DeckTirednessConditionDebug tirednessConditionDebug { get; set; } = null; // 本番や該当機能を使用しない環境では、適用しない
		public long[] offenseParticipationCoefficientList { get; set; } = null; // 攻撃時のポジション毎の参加係数
		public long[] defenseParticipationCoefficientList { get; set; } = null; // 守備時のポジション毎の参加係数
		public BattleV2BattleSetting battleSetting { get; set; } = null; // m_battle_gameliftのoptionJsonの値

   }
   
}

#endif