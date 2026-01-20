//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class SpotTacticsAbilitySpotTacticsAbilityInfo {
		public long[] mAbilityIdList { get; set; } = null; // 作戦固有スキル持ち不可変キャラ（ex. 軍師）によって追加されうるスキルIDのリスト
		public SpotTacticsAbilityTacticsAbility[] tacticsAbilityList { get; set; } = null; // 発生しうるスキルの情報リスト

   }
   
}

#endif