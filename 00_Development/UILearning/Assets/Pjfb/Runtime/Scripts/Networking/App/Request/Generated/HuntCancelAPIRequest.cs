//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
リタイア

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class HuntCancelAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class HuntCancelAPIResponse : AppAPIResponseBase {
		public HuntUserPending huntPending = null; // 狩猟のユーザー保留情報
		public long dailyPlayCount = 0; // 当日にプレイした回数
		public long mHuntTimetableId = 0; // 狩猟イベントのタイムテーブルID

   }
      
   public partial class HuntCancelAPIRequest : AppAPIRequestBase<HuntCancelAPIPost, HuntCancelAPIResponse> {
      public override string apiName{get{ return "hunt/cancel"; } }
   }
}