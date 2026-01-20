//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class DeckTirednessConditionDebug {
		public bool isDisabled { get; set; } = false; // 期限切れやconditionBaseが0で、乱数引き自体をしない設定かどうか
		public long seedBase { get; set; } = 0; // シード値の、上位桁（uDeckTiredness->conditionBase）
		public long unitCountTotal { get; set; } = 0; // 1980年から、経過したunit数
		public long unitCountTrimmed { get; set; } = 0; // unitCountTotalを加工し(%1000)シード値の下3桁になるように調整したもの
		public long seed { get; set; } = 0; // 最終的に生成されたシード値
		public long rate { get; set; } = 0; // シード値で得た、condition抽選用確立（万分率）
		public long conditionTableType { get; set; } = 0; // $conditionTableType
		public long finalRate { get; set; } = 0; // 最終割合

   }
   
}

#endif