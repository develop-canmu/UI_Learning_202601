//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class CharaV2FriendLend {
		public string lastLogin = ""; // 最終ログイン
		public string userName = ""; // ユーザー名
		public long relationType = 0; // 関係性（0 => フォロー無し、 1 => フォロー中、 2 => フォローされている、 3 => 相互フォロー）
		public long id = 0; // ユーザキャラID
		public long uMasterId = 0; // ユーザID
		public long mCharaSkinId = 0; // キャラスキンID
		public long combatPower = 0; // 総合力
		public long mCharaId = 0; // キャラID
		public long level = 0; // レベル
		public long newLiberationLevel = 0; // 潜在解放レベル

   }
   
}