//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingBoardPiece {
		public long index = 0; // 1始まりでいくつめのマスか。マス進行イベントの value はこの番号を参照する
		public TrainingBoardEvent boardEvent = null; // マス上のイベント
		public bool isSudden = false; // 突然イベントが出現するマスかどうか
		public bool isAdded = false; // ターン延長によって追加されたマスかどうか
		public long checkPointType = 0; // チェックポイント種別。0 => 通常マス, 1 => チェックポイントマス（battleCountの切り替わりが発生する試合等を含むターンに配置されたマス）
		public long checkPointIndex = 0; // チェックポイント番号。何番目の目標マスであるか判別するため、checkPointTypeが1のときbattleCountの値を入れる（現状チェックポイントの種類は1種類のみだが、種類ごとに値の意味合いが変わる可能性がある）
		public TrainingBoardFestivalPrize[] boardFestivalPrizeList = null; // 獲得した追加報酬に関する情報

   }
   
}