//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class SpotComboAbilityComboAbility {
		public long id { get; set; } = 0; // m_chara_combo_ability.id
		public string name { get; set; } = ""; // 名称
		public long requireCount { get; set; } = 0; // 発動に必要なキャラの数
		public long[] mAbilityIdList { get; set; } = null; // 発動スキルID一覧
		public long[] abilityLevelList { get; set; } = null; // 発動スキルレベル一覧

   }
   
}

#endif