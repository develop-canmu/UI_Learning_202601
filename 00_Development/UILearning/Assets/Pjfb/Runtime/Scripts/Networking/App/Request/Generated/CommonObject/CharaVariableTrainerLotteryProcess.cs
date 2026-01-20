//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class CharaVariableTrainerLotteryProcess {
		public long slotLength = 0; // スロット数。指定なしの場合は-1が入る。
		public long[] frameList = null; // 各枠が、どの抽選枠からの抽選にすることになったかの中間情報。mCharaTrainerLotteryContentGroupIdの列挙配列
		public long[] statusList = null; // 実際に、どのスキルが抽選されることになったのかの情報。mCharaTrainerLotteryStatusIdの列挙配列

   }
   
}