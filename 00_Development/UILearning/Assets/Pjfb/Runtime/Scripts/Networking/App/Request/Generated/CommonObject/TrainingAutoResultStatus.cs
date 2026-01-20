//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingAutoResultStatus {
		public ResultTurn[] turnList = null; // 目標イベントに対する延長ターンリスト
		public ResultCard[] cardList = null; // mTrainingCardIdとmCharaIdごとに選択回数リスト
		public ResultIdCount[] inspireExecuteList = null; // mTrainingInspireIdごとに実行回数リスト
		public ResultIdCount[] inspireList = null; // mTrainingInspireIdごとに獲得回数リスト
		public long rareTrainingCount = 0; // スペシャルレクチャー実行回数
		public long restCount = 0; // 休憩実行回数
		public long intentionalCount = 0; // 任意イベント実行回数
		public ResultTier[] tierList = null; // mTrainingConditionTier.tierごとに突入回数リスト
		public ResultIdCount[] concentrationList = null; // mTrainingConcentration.idごとに突入回数リスト
		public TrainingAbility[] abilityList = null; // アビリティ獲得リスト
		public long trainingPointAddedTurnValue = 0; // 延長ターンで変換されたトレーニングポイント獲得数
		public long trainingPointConditionValue = 0; // コンディション変化で変換されたトレーニングポイント獲得数
		public long handResetCount = 0; // トレーニング引き直し回数
		public long trainingPointLevelUpCount = 0; // トレーニングポイントのステータスレベルアップ回数
		public long trainingPointLevel = 0; // トレーニングポイントのステータスレベル
		public long trainingPointStatusAdditionCount = 0; // トレーニングポイントの追加ステータス発生回数
		public long[] mTrainingPointStatusEffectIdList = null; // トレーニングポイントの発生した効果IDリスト
		public ResultIdCount[] cardComboList = null; // mTrainingCardComboのid単位での発動回数リスト
		public long trainingPointStatusEffectCharaCount = 0; // 発動した選手専用ブースト回数
		public TrainingPointEffectCharaData[] trainingPointStatusEffectCharaList = null; // 発動中のキャラ効果リスト。mTrainingPointStatusEffectCharaにひもづく情報リスト
		public long concentrationLevel = 0; // コンセントレーションのレベル
		public long concentrationExp = 0; // コンセントレーションの経験値
		public long flowConcentrationCount = 0; // 特殊コンセントレーション発生回数
		public TrainingUnionCardReward[] unionCardRewardMap = null; // 発生したユニオンカード情報

   }
   
}