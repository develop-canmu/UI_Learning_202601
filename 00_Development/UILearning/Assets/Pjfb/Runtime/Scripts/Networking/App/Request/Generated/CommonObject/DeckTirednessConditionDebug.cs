//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class DeckTirednessConditionDebug {
		public bool isDisabled = false; // 期限切れやconditionBaseが0で、乱数引き自体をしない設定かどうか
		public long seedBase = 0; // シード値の、上位桁（uDeckTiredness->conditionBase）
		public long unitCountTotal = 0; // 1980年から、経過したunit数
		public long unitCountTrimmed = 0; // unitCountTotalを加工し(%1000)シード値の下3桁になるように調整したもの
		public long seed = 0; // 最終的に生成されたシード値
		public long rate = 0; // シード値で得た、condition抽選用確立（万分率）
		public long conditionTableType = 0; // $conditionTableType
		public long finalRate = 0; // 最終割合

   }
   
}