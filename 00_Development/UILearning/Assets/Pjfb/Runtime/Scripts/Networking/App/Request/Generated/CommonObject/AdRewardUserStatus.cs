//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class AdRewardUserStatus {
		public long mAdRewardId = 0; // $mAdRewardId mAdRewardId の Id
		public string date = ""; // $date 日付（Y-m-d 00:00:00形式で保存）
		public long grantedCount = 0; // $grantedCount 当日の報酬受取り回数
		public string lastGrantedAt = ""; // $lastGrantedAt 最後に動画再生報酬付与された日時
		public bool isEnabled = false; // $isEnabled 広告の閲覧が可能か

   }
   
}