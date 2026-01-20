//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
イベント詳細の取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class FestivalGetFestivalDetailAPIPost : AppAPIPostBase {
		public long mFestivalTimetableId = 0; // イベントタイムテーブルID

   }

   [Serializable]
   public class FestivalGetFestivalDetailAPIResponse : AppAPIResponseBase {
		public FestivalUserStatus userStatus = null; // 新イベントの表示用ユーザーデータ等

   }
      
   public partial class FestivalGetFestivalDetailAPIRequest : AppAPIRequestBase<FestivalGetFestivalDetailAPIPost, FestivalGetFestivalDetailAPIResponse> {
      public override string apiName{get{ return "festival/getFestivalDetail"; } }
   }
}