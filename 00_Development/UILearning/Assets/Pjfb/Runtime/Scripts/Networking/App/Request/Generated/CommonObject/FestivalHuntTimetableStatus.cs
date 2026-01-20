//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class FestivalHuntTimetableStatus {
		public long mHuntTimetableId = 0; // 狩猟タイムテーブルID
		public long nextPointValue = 0; // 次の敵・ストーリーが解放されるまでに必要なポイント量
		public long enemyCount = 0; // この狩猟タイムテーブルの総敵・ストーリー数
		public long openEnemyCount = 0; // 解放済みの敵・ストーリー数
		public bool hasNotification = false; // 通知のバッジをつけるか

   }
   
}