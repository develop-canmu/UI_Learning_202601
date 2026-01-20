//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumDeck {
		public long rank = 0; // デッキのランク
		public string combatPower = ""; // 総戦力
		public BattleV2Player player = null; // プレイヤー情報
		public ColosseumDeckChara[] charaList = null; // デッキ内包キャラ
		public BattleV2Chara[] charaVariableList = null; // デッキ内包可変キャラ

   }
   
}