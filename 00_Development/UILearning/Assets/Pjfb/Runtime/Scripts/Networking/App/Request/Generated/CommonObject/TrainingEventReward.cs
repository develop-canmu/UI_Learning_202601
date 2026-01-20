//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingEventReward {
		public long mTrainingEventRewardId = 0; // 元となった mTrainingEventReward の ID
		public long mTrainingEventId = 0; // 実施された mTrainingEvent の ID
		public long hp = 0; // hp
		public long mp = 0; // mp
		public long atk = 0; // atk
		public long def = 0; // def
		public long spd = 0; // spd
		public long tec = 0; // tec
		public long param1 = 0; // param1
		public long param2 = 0; // param2
		public long param3 = 0; // param3
		public long param4 = 0; // param4
		public long param5 = 0; // param5
		public long condition = 0; // コンディション（消費はマイナス、回復はプラス）
		public long conditionEffectRate = 0; // コンディションによる効果倍率
		public long baseConditionEffectRate = 0; // エクストラボーナス発動前のコンディションによる効果倍率
		public long exConditionEffectRate = 0; // エクストラボーナス発動後、追加ボーナス発動前のコンディションによる効果倍率
		public bool isConditionFree = false; // コンディション消費免除か
		public long maxConditionChange = 0; // 最大コンディションの増減
		public long conditionDistributeType = 0; // コンディション付与方法の種別。1 => condition の値だけ変動、2 => condition の値に変更（上昇する場合のみ）、3 => condition の値に変更（上昇・減少問わず）
		public long conditionBoardEventStatusId = 0; // コンディション付与に付随して付与された臨時練習能力効果実体のID。m_training_event_reward.conditionBoardEventStatusIdMap で定められたもの
		public TrainingBoardEventStatusOccurrence boardEventStatus = null; // 同時に獲得する臨時練習能力効果実体のID。m_training_event_reward.mTrainingBoardEventStatusId で定められたもの
		public long practiceType = 0; // このイベント報酬が練習カードを消費するイベントで得られたものである場合、経験値を獲得した練習カードの練習種別
		public long exp = 0; // 練習種別に対して得られた経験値
		public TrainingPracticeExp[] practiceExpList = null; // 練習種別に対して得られた経験値のうち、m_training_event_reward に紐づいていたもの
		public TrainingAbility[] getAbilityMapList = null; // このイベントで得られたスキル情報を羅列したリスト。abilityFluctuationListの下位互換
		public TrainingAbility[] getPointAbilityMapList = null; // トレーニングポイントで得られたスキル情報を羅列したリスト。abilityFluctuationListの下位互換
		public TrainingAbilityFluctuation[] abilityFluctuationList = null; // このイベントで得られたスキル情報詳細を羅列したリスト
		public long[] addMCharaIdList = null; // このイベントから今回の個人トレーニングにサポートキャラとして追加される mChara の idList
		public long hasAchievedGoal = 0; // このイベントの消化の際、目標を達成したか。1: 達成した、2: 達成できなかった。
		public long turnAddValue = 0; // 加算されたターン数
		public long mTrainingConcentrationId = 0; // 発動したコンセントレーションのid
		public long concentrationType = 0; // 発動したコンセントレーションの種別
		public TrainingInspire[] flowInspireList = null; // FLOWコンセントレーションで獲得したインスピレーションリスト
		public TrainingInspire[] inspireList = null; // 獲得したインスピレーションリスト
		public long[] mTrainingCardComboIdList = null; // 発動したコンボのidリスト
		public bool isCardReward = false; // このイベント報酬が練習カードを消費するイベントで得られるものであるか。
		public bool isGradeUp = false; // エクストラボーナスが発生したか
		public bool isTrainingGradeUp = false; // 練習時発生ボーナスが発生したか
		public bool isConcentrationGradeUp = false; // コンセントレーションのグレードが上がったか
		public bool isConcentrationExtended = false; // コンセントレーションが延長されたか
		public long concentrationExpConditionTier = 0; // コンディション帯変化で獲得したコンセントレーションのexp
		public long concentrationExpAddTurn = 0; // 追加ターンで獲得したコンセントレーションのexp
		public long concentrationExp = 0; // 報酬で獲得したコンセントレーションのexp
		public bool isInspireLevelUp = false; // インスピレーションレベルが上がったか
		public long inspireEnhanceRate = 0; // インスピレーション効果でのステータス加算倍率（万分率）
		public TrainingRewardAdditionHistory[] rewardAdditionHistoryList = null; // 報酬の追加履歴リスト
		public TrainingBoardReward boardReward = null; // マス盤で得られる報酬
		public bool isBoardEventStatusOverwritten = false; // BoardReward 内または BoardEventStatusOccurrence 内で、同種の臨時練習能力の獲得により発生中の効果が延長されたかどうか
		public TrainingBoardFestivalPrize[] festivalPrizeList = null; // 獲得した追加報酬
		public TrainingUnionCardReward concentrationUnionCard = null; // コンセントレーションで獲得したユニオンカードの報酬
		public long pointConvertAddedTurnValue = 0; // ポイントに変換された追加ターン数
		public long pointConvertConditionTier = 0; // ポイントに変換されたコンディション帯
		public long addedTurnPointValue = 0; // 追加ターンで獲得したトレーニングポイント数
		public long conditionTierPointValue = 0; // コンディション帯変化で獲得したトレーニングポイント数
		public long pointStatusEffectRate = 0; // 発生中のmTrainingPointStatusEffectの効果倍率
		public long pointStatusEffectRateLabelType = 0; // 発生中の効果倍率の表示用ラベル種別
		public WrapperIntList[] mAbilityTrainingPointStatusList = null; // mTrainingPointStatusEffectで得られたスキルIDとレベルを羅列したリスト。[[m_ability_training_point_status.id, level]]

   }
   
}