//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2Tactics {
		public long id { get; set; } = 0; // ID
		public string name { get; set; } = ""; // 作戦名
		public bool isPlayable { get; set; } = false; // プレイヤーが選択可能か
		public bool isPrimary { get; set; } = false; // プレイヤーが選択可能な場合、初期から選択可能か
		public BattleV2TacticsDetail[] tacticsDetailList { get; set; } = null; // 作戦詳細

   }
   
}

#endif