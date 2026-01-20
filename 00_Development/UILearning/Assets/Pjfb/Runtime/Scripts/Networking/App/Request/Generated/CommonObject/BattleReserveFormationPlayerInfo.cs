//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleReserveFormationPlayerInfo {
		public long partyNumber = 0; // パーティ番号
		public string groupName = ""; // グループ名
		public long rank = 0; // デッキのランク
		public string combatPower = ""; // 総戦力
		public BattleV2Player player = null; // プレイヤー情報
		public ColosseumDeckChara[] charaList = null; // デッキ内包キャラ
		public BattleV2Chara[] charaVariableList = null; // デッキ内包可変キャラ

   }
   
}