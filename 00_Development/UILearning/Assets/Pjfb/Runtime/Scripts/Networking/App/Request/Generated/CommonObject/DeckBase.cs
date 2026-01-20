//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class DeckBase {
		public long partyNumber = 0; // パーティ番号
		public string name = ""; // デッキ名
		public WrapperIntList[] memberIdList = null; // u_chara.idかu_chara_variable.idを格納する配列 [type, id]を列挙した配列になる。type = 1のときu_chara, 2のときu_chara_variable
		public string combatPower = ""; // 総戦力値
		public long rank = 0; // ランク
		public long optionValue = 0; // オプション値
		public DeckTiredness tiredness = null; // 疲労度管理。疲労度が無いデッキには無い

   }
   
}