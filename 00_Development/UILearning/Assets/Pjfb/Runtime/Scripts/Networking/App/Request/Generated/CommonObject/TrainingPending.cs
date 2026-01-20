//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingPending {
		public long uTrainingPendingId = 0;
		public long mCharaId = 0; // 育成キャラのID
		public long mTrainingScenarioId = 0; // プレイ中のシナリオ
		public long turn = 0; // プレイ中のシナリオの何ターン目か
		public long turnAddEffectType = 0; // 追加ターン演出種別
		public bool occursNextAddedTurn = false; // 次の追加ターン実行可能か
		public long addedTurnMTrainingEventId = 0; // ターンの加算を行うmTrainingEventId
		public long timing = 0; // ターン上のどのフェーズか。次に reward 等を受け取ることになるフェーズを保存する。1001: ターン前、2001: 行動、3001: 休息とバトル、4001: 行動後イベント、5001: ターン後
		public long condition = 0; // コンディション値
		public long previousCondition = 0; // コンディションが変動する前の値
		public long maxCondition = 0; // 最大コンディション値
		public long conditionType = 0; // m_training_condition_tierのtype
		public TrainingIntentionalEvent[] intentionalEventList = null; // 選択可能な任意イベントの情報のリスト
		public TrainingPracticeProgress[] practiceProgressList = null; // 練習カテゴリごとのレベル進捗情報
		public TrainingOverallProgress overallProgress = null; // 練習効率テーブル等に影響する、トレーニングの全体を通したレベル進捗情報
		public WrapperIntList[] supportMCharaIdList = null; // サポートキャラの mCharaId を二重配列で保存する。初期サポートキャラリスト、フレンドリスト、スペシャルサポートリスト、イベントで追加されたサポートキャラのリスト、の順番で4つの mCharaIdList を保持する。
		public TrainingCard[] handList = null; // 手札の情報。 n番目の要素は、手札n番目のカードの情報
		public WrapperIntList[] handSupportMCharaIdList = null; // 練習に参加するキャラの情報。n番目の要素は、手札n番目のカードに参加するサポートキャラの idList例：[[1,2],[],[3],[4,5,6],[]]
		public TrainingCardReward[] handRewardList = null; // 獲得ステータスの情報。n番目の要素は、手札n番目のカードを使用した際に得られるステータス値
		public TrainingSupport[] supportDetailList = null; // サポートの詳細情報
		public TrainingGoal[] goalList = null; // 目標情報
		public long nextGoalIndex = 0; // 次の目標の index 。もう存在しない場合は -1
		public string createdAt = ""; // トレーニング開始時刻
		public TrainingInspire[] inspireList = null; // 発動中のインスピレーションリスト
		public long inspireExp = 0; // インスピレーションブーストの経験値
		public long mTrainingConcentrationId = 0; // 発動中のコンセントレーションID
		public bool isFinishedConcentration = false; // 発動中のコンセントレーションが終了したか
		public long concentrationLevel = 0; // 現在のコンセントレーションレベル
		public long concentrationExp = 0; // 現在のコンセントレーション所持経験値
		public TrainingActiveTrainingStatusType[] activeTrainingStatusTypeList = null; // 有効になっている練習能力のリスト
		public TrainingBoardTurnConditionTier[] boardTurnConditionTierList = null; // ターンと、そのターンのマス進行時のコンディション帯情報。現在のターンまでの情報を列挙している
		public TrainingBoardTurn[] boardTurnList = null; // ターンごとのマス情報リスト
		public TrainingBoardCondition[] boardConditionList = null; // 特定のコンディション帯の時に使用するマス盤情報のリスト
		public TrainingActiveTrainingBoardEventStatus[] activeTrainingBoardEventStatusList = null; // 発動中の臨時練習能力IDリスト
		public long[] activatedBoardEventStatusIdList = null;
		public FestivalPrizeStatus[] festivalPrizeStatusList = null; // イベントを通して獲得可能な残り追加報酬に関する情報
		public TrainingPrizePending prizePending = null; // （現在の修行中における）獲得済みの追加報酬に関する情報

   }
   
}