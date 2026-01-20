//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2UnitRoleAction {
		public long id = 0; // $id
		public long mDeckUnitRoleOperationId = 0; // $mDeckUnitRoleOperationId 指示ID
		public long actionType = 0; // $actionType 行動内容
		public long actionValue = 0; // $actionValue 行動内容のvalue
		public string invokeCondition = ""; // $invokeCondition 行動条件を指定するjson文字列
		public long priority = 0; // $priority 優先度

   }
   
}