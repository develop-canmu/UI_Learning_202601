//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
戦闘開始

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class HuntStartAPIPost : AppAPIPostBase {
		public long mHuntTimetableId = 0; // 狩猟イベントのタイムテーブルID
		public long mHuntEnemyId = 0; // 敵ID
		public long deckOptionValue = 0; // デッキのオプション値

   }

   [Serializable]
   public class HuntStartAPIResponse : AppAPIResponseBase {
		public HuntUserPending huntPending = null; // 狩猟のユーザー保留情報

   }
      
   public partial class HuntStartAPIRequest : AppAPIRequestBase<HuntStartAPIPost, HuntStartAPIResponse> {
      public override string apiName{get{ return "hunt/start"; } }
   }
}