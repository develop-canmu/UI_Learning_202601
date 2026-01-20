//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class SpotTacticsAbilityTacticsAbility {
		public long mDeckTacticsId { get; set; } = 0; // スキルの発動に必要となる作戦ID
		public long mCharaId { get; set; } = 0; // スキルの発動に必要となる不可変キャラのキャラID
		public long charaLevelMin { get; set; } = 0; // スキルの発動に必要となる不可変キャラのレベル
		public WrapperIntList[] abilityList { get; set; } = null; // スキルIDとスキルレベルの情報

   }
   
}

#endif