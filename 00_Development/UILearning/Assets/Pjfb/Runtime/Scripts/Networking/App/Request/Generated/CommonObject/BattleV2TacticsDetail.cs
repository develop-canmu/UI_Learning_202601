//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleV2TacticsDetail {
		public long id = 0; // $id
		public long mDeckTacticsId = 0; // $mDeckTacticsId 作戦設定ID
		public long unitNumber = 0; // $unitNumber 1~5のユニット番号。pjshinでは1に指定されたユニットが撃破されると敗北となる。
		public long mDeckUnitRoleId = 0; // $mDeckUnitRoleId ロールID
		public long mDeckUnitRoleOperationIdDefault = 0; // $mDeckUnitRoleOperationIdDefault この作戦詳細設定にデフォルトで割り当てられている指示ID
		public long[] mDeckUnitRoleOperationIdList = null; // $mDeckUnitRoleOperationIdList この作戦詳細設定にて選択できる指示IDのリスト
		public long positionX = 0; // $positionX 初期配置X座標。pjshinでは本陣からどれだけ離すかを指定する
		public long positionY = 0; // $positionY 初期配置Y座標。pjshinでは何番目のレーンかを指定する
		public long militaryStrengthRate = 0; // $militaryStrengthRate 割り当てられる兵力の割合。万分率
		public string displayName = ""; // $displayName 表示名

   }
   
}