//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
一括戦闘処理

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class HuntStartAndFinishBulkAPIPost : AppAPIPostBase {
		public long mHuntTimetableId = 0; // 狩猟イベントのタイムテーブルID
		public long mHuntEnemyId = 0; // 敵ID
		public long bulkVolume = 0; // 一括実行するクエストの数
		public long mHuntEnemyIdEnd = 0; // 一括実行するクエストの終端ID
		public long deckOptionValue = 0; // デッキのオプション値
		public long mStaminaCureId = 0; // スタミナ回復マスタID
		public long staminaCureValue = 0; // 何口分実行を行うか

   }

   [Serializable]
   public class HuntStartAndFinishBulkAPIResponse : AppAPIResponseBase {
		public HuntPrizeSet[] prizeSetList = null; // 報酬情報（報酬区分ごとに振り分け済み）
		public HuntPrizeCorrection[] prizeCorrectionList = null; // 報酬獲得量補正情報
		public HuntUserPending huntPending = null; // 狩猟のユーザー保留情報
		public HuntEnemyHistory enemyHistory = null; // 狩猟イベントのユーザークエクリア状況
		public long[] mHuntEnemyIdList = null; // 処理したm_hunt_enemyのIDリスト
		public long dailyPlayCount = 0; // 当日にプレイした回数
		public long mHuntTimetableId = 0; // 狩猟イベントのタイムテーブルID

   }
      
   public partial class HuntStartAndFinishBulkAPIRequest : AppAPIRequestBase<HuntStartAndFinishBulkAPIPost, HuntStartAndFinishBulkAPIResponse> {
      public override string apiName{get{ return "hunt/startAndFinishBulk"; } }
   }
}