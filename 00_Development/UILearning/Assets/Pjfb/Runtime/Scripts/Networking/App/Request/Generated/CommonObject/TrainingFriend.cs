//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingFriend {
		public UserCommunityUserStatus communityUserStatus = null; // コミュニティ関連のユーザの情報
		public long relationType = 0; // 関係性（0 => フォロー無し、 1 => フォロー中、 2 => フォローされている、 3 => 相互フォロー）
		public long mCharaId = 0; // mCharaId
		public long mCharaSkinId = 0; // スキンID
		public long level = 0; // レベル
		public long newLiberationLevel = 0; // 潜在解放レベル
		public long combatPower = 0; // 総合力
		public long uMasterId = 0; // フレンドキャラの持ち主ユーザーID

   }
   
}