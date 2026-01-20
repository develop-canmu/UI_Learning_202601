//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
対戦相手不在の敵データを指定し、一方的にクリアを実施する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class HuntStartAndFinishAPIPost : AppAPIPostBase {
		public long mHuntTimetableId = 0; // 狩猟イベントのタイムテーブルID
		public long mHuntEnemyId = 0; // 敵ID

   }

   [Serializable]
   public class HuntStartAndFinishAPIResponse : AppAPIResponseBase {
		public HuntPrizeSet[] prizeSetList = null; // 報酬情報（報酬区分ごとに振り分け済み）
		public HuntPrizeCorrection[] prizeCorrectionList = null; // 報酬獲得量補正情報
		public HuntUserPending huntPending = null; // 狩猟のユーザー保留情報
		public HuntEnemyHistory enemyHistory = null; // 狩猟イベントのユーザークエクリア状況
		public long mHuntEnemyId = 0; // m_hunt_enemyのID
		public long dailyPlayCount = 0; // 当日にプレイした回数
		public long mHuntTimetableId = 0; // 狩猟イベントのタイムテーブルID

   }
      
   public partial class HuntStartAndFinishAPIRequest : AppAPIRequestBase<HuntStartAndFinishAPIPost, HuntStartAndFinishAPIResponse> {
      public override string apiName{get{ return "hunt/startAndFinish"; } }
   }
}