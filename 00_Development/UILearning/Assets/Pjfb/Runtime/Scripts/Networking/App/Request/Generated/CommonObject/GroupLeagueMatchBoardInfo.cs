//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GroupLeagueMatchBoardInfo {
		public GroupLeagueMatchBoardRow[] rowList = null; // リーグ表
		public GroupLeagueMatchTodayMatch todayMatch = null; // 当日の試合
		public GroupLeagueMatchGroupStatusDetail groupStatusDetailSelf = null; // 自ギルドの累計成績情報
		public GroupLeagueMatchGroupStatusDetail groupStatusDetailOpponent = null; // 相手ギルドの累計成績情報
		public ColosseumShiftMatchInfo shiftMatchInfo = null; // 入れ替え戦の進捗・結果の紐づけ。入れ替え戦にマッチングサれていない場合は、生成サれない
		public bool isFinishMatchingGradeShiftMatch = false; // 入れ替え戦マッチングが終わっているかどうか（入れ替え戦がそもそもないイベントの場合、シーズン戦が全て終わり順位集計が完了しているかどうか）
		public bool isForcePromotedWithoutShiftMatch = false; // 入れ替え戦なしでの昇格が発生しているかどうか。入れ替え戦のマッチングが完了されていない場合は、生成されない
		public PlayerGameliftOptionSetting battleGameliftSetting = null; // GameLift対戦におけるプレイヤーごとの事前設定

   }
   
}