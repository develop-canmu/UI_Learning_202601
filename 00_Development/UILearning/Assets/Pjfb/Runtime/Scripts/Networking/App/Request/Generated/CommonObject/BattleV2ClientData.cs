//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2ClientData {
		public BattleV2Group[] groupList = null;
		public BattleV2Player[] playerList = null;
		public BattleV2Chara[] charaList = null;
		public BattleV2Ability[] abilityList = null;
		public SpotComboAbilitySpotComboAbilityInfo spotComboAbilityInfo = null; // クライアント側でその場でコンボスキルを発動させる際に必要な情報
		public SpotTacticsAbilitySpotTacticsAbilityInfo spotTacticsAbilityInfo = null; // クライアント側でその場で戦術スキルを発動させる際に必要な情報
		public BattleV2BattleFbKeeper[] battleFbKeeperList = null;
		public BattleV2BattleWarField battleWarField = null;
		public BattleV2BattleConquestField battleConquestField = null;
		public BattleV2Tactics[] tacticsList = null;
		public BattleV2UnitRole[] unitRoleList = null;
		public BattleV2UnitRoleOperation[] unitRoleOperationList = null;
		public long firstAttackIndex = 0; // 先制陣営指定(nullの場合は、デフォルトの挙動をしてもらう感じで)
		public long seedNum = 0; // ランダム数値
		public DeckTirednessConditionDebug tirednessConditionDebug = null; // 本番や該当機能を使用しない環境では、適用しない
		public long[] offenseParticipationCoefficientList = null; // 攻撃時のポジション毎の参加係数
		public long[] defenseParticipationCoefficientList = null; // 守備時のポジション毎の参加係数
		public BattleV2BattleSetting battleSetting = null; // m_battle_gameliftのoptionJsonの値

   }
   
}