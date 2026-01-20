//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class SpotTacticsAbilityTacticsAbility {
		public long mDeckTacticsId = 0; // スキルの発動に必要となる作戦ID
		public long mCharaId = 0; // スキルの発動に必要となる不可変キャラのキャラID
		public long charaLevelMin = 0; // スキルの発動に必要となる不可変キャラのレベル
		public WrapperIntList[] abilityList = null; // スキルIDとスキルレベルの情報

   }
   
}