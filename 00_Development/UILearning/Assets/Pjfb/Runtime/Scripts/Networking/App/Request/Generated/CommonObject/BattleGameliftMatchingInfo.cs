//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleGameliftMatchingInfo {
		public long sBattleGameliftMatchingId = 0; // 試合ID
		public BattleV2ClientData clientData = null; // バトルに使用する情報
		public string battleStartAt = ""; // バトル開始時刻
		public string battleStartAtSub = ""; // バトル開始サブ時刻。battleStartAtの後に、それとは別に本格的に対戦が始動する時刻を設定する必要がある場合等に使用
		public string battleFinishAt = ""; // バトル終了時刻

   }
   
}