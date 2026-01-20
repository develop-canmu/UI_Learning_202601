//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2UnitRoleOperation {
		public long id { get; set; } = 0; // ID
		public long mDeckUnitRoleId { get; set; } = 0; // ユニット役割ID
		public bool isDefault { get; set; } = false; // デフォルトの指示かどうか
		public string name { get; set; } = ""; // 指示名
		public string description { get; set; } = ""; // 説明
		public BattleV2UnitRoleAction[] unitRoleActionList { get; set; } = null; // 作戦詳細

   }
   
}

#endif