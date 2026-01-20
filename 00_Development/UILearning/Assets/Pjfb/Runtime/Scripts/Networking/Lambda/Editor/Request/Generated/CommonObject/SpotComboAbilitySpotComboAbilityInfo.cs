//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class SpotComboAbilitySpotComboAbilityInfo {
		public long[] mAbilityIdList { get; set; } = null; // コンボスキルによって追加されるスキルIDのリスト
		public SpotComboAbilityComboAbility[] comboAbilityList { get; set; } = null; // 発生しうるコンボスキルの情報リスト
		public SpotComboAbilityComboAbilityElement[] comboAbilityElementList { get; set; } = null; // 発生しうるコンボスキルの構成要素情報リスト

   }
   
}

#endif