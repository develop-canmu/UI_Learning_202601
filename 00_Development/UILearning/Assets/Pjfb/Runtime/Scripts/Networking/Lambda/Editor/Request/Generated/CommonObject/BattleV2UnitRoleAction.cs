//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2UnitRoleAction {
		public long id { get; set; } = 0; // $id
		public long mDeckUnitRoleOperationId { get; set; } = 0; // $mDeckUnitRoleOperationId 指示ID
		public long actionType { get; set; } = 0; // $actionType 行動内容
		public long actionValue { get; set; } = 0; // $actionValue 行動内容のvalue
		public string invokeCondition { get; set; } = ""; // $invokeCondition 行動条件を指定するjson文字列
		public long priority { get; set; } = 0; // $priority 優先度

   }
   
}

#endif