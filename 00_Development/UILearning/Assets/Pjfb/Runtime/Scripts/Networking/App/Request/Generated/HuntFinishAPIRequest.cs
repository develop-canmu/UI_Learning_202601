//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
バトル終了

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class HuntFinishAPIPost : AppAPIPostBase {
		public long battleResult = 0; // バトル結果 1=> 勝利, 2 => 敗北, 3 => 引き分け

   }

   [Serializable]
   public class HuntFinishAPIResponse : AppAPIResponseBase {
		public HuntPrizeSet[] prizeSetList = null; // 報酬情報（報酬区分ごとに振り分け済み）
		public HuntPrizeCorrection[] prizeCorrectionList = null; // 報酬獲得量補正情報
		public HuntUserPending huntPending = null; // 狩猟のユーザー保留情報
		public HuntEnemyHistory enemyHistory = null; // 狩猟イベントのユーザークエクリア状況
		public long mHuntEnemyId = 0; // m_hunt_enemyのID
		public long dailyPlayCount = 0; // 当日にプレイした回数
		public long mHuntTimetableId = 0; // 狩猟イベントのタイムテーブルID

   }
      
   public partial class HuntFinishAPIRequest : AppAPIRequestBase<HuntFinishAPIPost, HuntFinishAPIResponse> {
      public override string apiName{get{ return "hunt/finish"; } }
   }
}