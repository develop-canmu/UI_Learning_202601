//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingPointStatus {
		public long value = 0; // 所持ポイント数
		public long level = 0; // レベル
		public long handResetCount = 0; // 練習カード引き直し済み回数
		public long[] mTrainingPointStatusEffectIdList = null; // 発動中のmTrainingPointStatusEffectのidリスト
		public bool enableLevelUp = false; // レベルアップ実行可能か
		public bool enableHandReset = false; // 練習カード引き直し実行可能か
		public long remainLevelUpEnableTurn = 0; // レベルアップ可能までの残りターン数
		public long remainHandResetEnableTurn = 0; // 手札リセット可能までの残りターン数
		public long levelUpCostValue = 0; // レベルアップ消費量
		public long handResetCostValue = 0; // 手札リセット消費量
		public TrainingPointEffectCharaData[] trainingPointStatusEffectCharaList = null; // 発動中のキャラ効果リスト。mTrainingPointStatusEffectCharaにひもづく情報リスト

   }
   
}