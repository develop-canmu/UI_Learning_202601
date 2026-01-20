//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleReserveFormationMatchLineup {
		public long roundNumber = 0; // 回戦情報
		public long result = 0; // 試合結果（1 win 2 lose 3 draw  99未処理）
		public string openAt = "";
		public long pointGet = 0; // $pointGet 総得点
		public long pointLost = 0; // $pointLost 総失点
		public BattleReserveFormationFieldOption fieldOption = null; // フィールドオプション
		public BattleReserveFormationPlayerInfo playerInfo = null; // プレイヤー情報
		public BattleReserveFormationPlayerInfo playerInfoOpponent = null; // 対戦相手プレイヤー情報
		public long previewId = 0; // バトルログ参照に用いるId情報（s_battle_reserve_formation_log.id）

   }
   
}