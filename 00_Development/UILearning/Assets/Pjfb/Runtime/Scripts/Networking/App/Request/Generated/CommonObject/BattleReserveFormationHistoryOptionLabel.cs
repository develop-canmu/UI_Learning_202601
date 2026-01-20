//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleReserveFormationHistoryOptionLabel {
		public long mBattleReserveFormationId = 0; // mBattleReserveFormationId
		public long matchType = 0; // 試合の種別。m_battle_reserve_formation経由のバトルの場合、1 => シーズン戦、2 => 入れ替え戦。(不要な区分では格納しない)
		public long dayNumber = 0; // 日付番号(不要な区分では格納しない)
		public long roundNumber = 0; // ラウンド番号(不要な区分では格納しない)

   }
   
}