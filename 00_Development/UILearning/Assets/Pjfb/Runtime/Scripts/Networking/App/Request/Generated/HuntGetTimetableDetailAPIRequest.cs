//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
特定のタイムテーブルの、ユーザー状態を取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class HuntGetTimetableDetailAPIPost : AppAPIPostBase {
		public long mHuntTimetableId = 0; // 狩猟イベントのタイムテーブルID

   }

   [Serializable]
   public class HuntGetTimetableDetailAPIResponse : AppAPIResponseBase {
		public long[] mHuntEnemyIdList = null; // 敵ID一覧（抽選不要な区分に関しては、何も返さない）
		public HuntEnemyHistory enemyHistory = null; // 狩猟イベントのユーザークエクリア状況
		public float startAndFinishBulkFactor = 0.0f; // 一括狩猟時に、自身の総戦闘力に乗算される係数

   }
      
   public partial class HuntGetTimetableDetailAPIRequest : AppAPIRequestBase<HuntGetTimetableDetailAPIPost, HuntGetTimetableDetailAPIResponse> {
      public override string apiName{get{ return "hunt/getTimetableDetail"; } }
   }
}