//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleGameliftMatchingResult {
		public long sBattleGameliftMatchingId = 0; // 試合ID
		public long result = 0; // left視点での勝敗。1:win 2:lose 3:draw
		public BattleGameliftMatchingResultArgs[] argsList = null; // 試合結果詳細オブジェクト。0個目の要素が左陣営、1個目の要素が右陣営のもの
		public long[] joinedUMasterIdList = null; // 参加したユーザーのIDリスト
		public BattleV2GvgItemUser[] usedItemList = null; // ユーザーごとに使用したアイテムリストをまとめた情報のリスト
		public string logData = ""; // 圧縮済み試合ログ文字列

   }
   
}