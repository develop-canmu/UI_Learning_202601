//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingCharaVariable {
		public long uCharaVariableId = 0;
		public long mCharaId = 0; // 性能ベース mCharaId
		public long hp = 0; // hp
		public long mp = 0; // mp
		public long atk = 0; // atk
		public long def = 0; // def
		public long spd = 0; // spd
		public long tec = 0; // tec
		public long param1 = 0; // param1
		public long param2 = 0; // param2
		public long param3 = 0; // param3
		public long param4 = 0; // param4
		public long param5 = 0; // param5
		public long combatPower = 0; // 総合力
		public string rank = ""; // $rank ランク
		public TrainingAbility[] abilityList = null; // $abilityList アクティブスキルを表現するのに必要なJSON構造

   }
   
}