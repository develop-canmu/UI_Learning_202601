//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2BattleFbKeeper {
		public long id { get; set; } = 0; // ID
		public string visualKey { get; set; } = ""; // キャラクターの画像パス
		public long level { get; set; } = 0; // レベル
		public long requestValueMax { get; set; } = 0; // シュート要求値max
		public long requestValueMin { get; set; } = 0; // シュート要求値min
		public long groupIndex { get; set; } = 0; // バトルデータ内の所属グループ区別用の識別子

   }
   
}

#endif