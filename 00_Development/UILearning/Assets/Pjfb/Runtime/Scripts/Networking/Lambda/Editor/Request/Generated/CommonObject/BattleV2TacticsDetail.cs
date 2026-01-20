//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2TacticsDetail {
		public long id { get; set; } = 0; // $id
		public long mDeckTacticsId { get; set; } = 0; // $mDeckTacticsId 作戦設定ID
		public long unitNumber { get; set; } = 0; // $unitNumber 1~5のユニット番号。pjshinでは1に指定されたユニットが撃破されると敗北となる。
		public long mDeckUnitRoleId { get; set; } = 0; // $mDeckUnitRoleId ロールID
		public long mDeckUnitRoleOperationIdDefault { get; set; } = 0; // $mDeckUnitRoleOperationIdDefault この作戦詳細設定にデフォルトで割り当てられている指示ID
		public long[] mDeckUnitRoleOperationIdList { get; set; } = null; // $mDeckUnitRoleOperationIdList この作戦詳細設定にて選択できる指示IDのリスト
		public long positionX { get; set; } = 0; // $positionX 初期配置X座標。pjshinでは本陣からどれだけ離すかを指定する
		public long positionY { get; set; } = 0; // $positionY 初期配置Y座標。pjshinでは何番目のレーンかを指定する
		public long militaryStrengthRate { get; set; } = 0; // $militaryStrengthRate 割り当てられる兵力の割合。万分率
		public string displayName { get; set; } = ""; // $displayName 表示名

   }
   
}

#endif