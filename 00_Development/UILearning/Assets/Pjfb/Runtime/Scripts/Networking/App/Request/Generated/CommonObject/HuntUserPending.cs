//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class HuntUserPending {
		public long state = 0; // $state レコードの状態。0 ⇒ 何もない、 1 ⇒ 戦闘中、 2 ⇒ 戦闘勝利後
		public long mHuntTimetableId = 0; // $mHuntTimetableId 狩猟イベントタイムテーブルID。stateが0の場合、null
		public long mHuntEnemyId = 0; // $mHuntEnemyId 対戦中の敵ID。stateが0の場合、null
		public BattleV2ClientData clientData = null; // 戦闘状態じゃない場合は場合はnull

   }
   
}