//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingBattlePending {
		public long state = 0; // $state バトル状態（0 => バトル発生状態でない、1 => バトル準備状態、2 => バトル中）
		public long mTrainingBattleId = 0; // バトルID。stateが0の場合、null
		public BattleV2ClientData clientData = null; // 戦闘状態じゃない場合は場合はnull

   }
   
}