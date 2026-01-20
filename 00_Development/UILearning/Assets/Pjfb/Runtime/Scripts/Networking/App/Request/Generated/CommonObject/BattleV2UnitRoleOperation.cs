//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2UnitRoleOperation {
		public long id = 0; // ID
		public long mDeckUnitRoleId = 0; // ユニット役割ID
		public bool isDefault = false; // デフォルトの指示かどうか
		public string name = ""; // 指示名
		public string description = ""; // 説明
		public BattleV2UnitRoleAction[] unitRoleActionList = null; // 作戦詳細

   }
   
}