//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class SpotComboAbilityComboAbilityElement {
		public long id { get; set; } = 0; // m_chara_combo_ability_element.id
		public long mCharaComboAbilityId { get; set; } = 0; // コンボスキルのID
		public long mCharaId { get; set; } = 0; // 対象キャラID
		public bool isTarget { get; set; } = false; // スキルの付与対象かどうか

   }
   
}

#endif