//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingRewardAdditionHistory {
		public long historyType = 0; // 履歴タイプ。101=>原点、201=>MTrainingAdditionalRewardによる加算
		public string conditionParam = ""; // 履歴タイプ201（バトル結果による追加ボーナス）の場合、使用されたパラメータ名
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

   }
   
}