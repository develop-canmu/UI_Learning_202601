//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2Tactics {
		public long id = 0; // ID
		public string name = ""; // 作戦名
		public bool isPlayable = false; // プレイヤーが選択可能か
		public bool isPrimary = false; // プレイヤーが選択可能な場合、初期から選択可能か
		public BattleV2TacticsDetail[] tacticsDetailList = null; // 作戦詳細

   }
   
}