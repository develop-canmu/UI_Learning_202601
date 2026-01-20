//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class CharaVariableTrainerBase {
		public long id = 0; // ID
		public long uMasterId = 0; // ユーザーID
		public long mCharaId = 0; // 性能ベースキャラID
		public long level = 0; // レベル。デフォルトは1
		public long[] typeList = null; // このレコードが持つ練習能力タイプ（m_training_status_type_detail.type）を詰めたjson配列。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。このテーブルではレコード内に保存されている情報（サブ能力）のみ羅列する
		public long[] valueList = null; // このレコードが持つ練習能力の効果量を詰めたjson配列。値の順番はtypeListに対応する。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		public CharaVariableTrainerStatusCommon firstParamAddMap = null; // 初期◯◯アップ（実数値）。ステータス初期値に加算される
		public long battleParamEnhanceRate = 0; // 練習試合ボーナス（万分率）。練習試合で得られるステータスが割合でアップする
		public long rarePracticeEnhanceRate = 0; // レア練習率アップ（万分率）。1+rate/10000 がレア練習カードの重みに乗算される
		public CharaVariableTrainerStatusCommon battleParamEnhanceMap = null; // 練習試合時◯◯ボーナス（万分率）。練習試合で得られるステータスがパラメータ別に割合でアップする。battleParamEnhanceRate とは加算
		public CharaVariableTrainerConditionEffectGradeUpOnType[] conditionEffectGradeUpMapOnType = null; // xx実施時効果ボーナス上昇。練習実施時に発生するボーナス率に、指定確率でn段階の上昇補正がかかる。[{practiceType:0,grade:2,rate:1000},{practiceType:0,grade:1,rate:2000},{practiceType:1,grade:1,rate:1000}] のように指定する
		public CharaVariableTrainerStatusCommon practiceParamAddBonusMap = null; // 練習時固定ボーナス（実数値）。実施する練習の種別に関わらず、トレーニング実施時に特定ステータスを追加で獲得する。
		public CharaVariableTrainerPracticeParamEnhanceOnType[] practiceParamEnhanceMapOnType = null; // xx実施時◯◯獲得量アップ（万分率）。特定の練習種別の練習を実施時に特定のステータスの獲得量が割合でアップする。[{practiceType:0,rateMap:{param1:1000,param2:500}},{practiceType:1,rateMap:{spd:2000}}] のように指定する
		public CharaVariableTrainerRarePracticeEnhanceOnType[] rarePracticeEnhanceRateMapOnType = null; // xxのレア練習出現率アップ（万分率）。特定の練習種別においてレア練習カードの重みに 1+rate/10000 を乗算し、レア練習出現率をアップする。rarePracticeEnhanceRate とは加算。[{practiceType:0,rate:1500},{practiceType:1,rate:1000}] のように指定する
		public CharaVariableTrainerPopRateEnhanceOnType[] popRateEnhanceMapOnType = null; // xxのレクチャー率アップ（万分率）。特定の練習種別において、レア練習カードが採用された場合にカードの持ち主が参加する確率が上昇する
		public long[] firstMTrainingEventRewardIdList = null; // 初期イベント報酬指定。mTrainingEventRewardId を指定すると、既に該当の報酬を受け取った状態でトレーニングを開始する
		public CharaVariableTrainerStatusCommon practiceParamRateMap = null; // 効果量アップ倍率（万分率）。practiceParamAddMapと同じ形式で記載。指定ステータスに対して乗算される。常時発動
		public CharaVariableTrainerCoachEnhanceRateOnType[] coachEnhanceRateMapOnType = null; // xx実施時特訓発生率アップ。特定の練習種別において、特訓の発生率が上昇する
		public long entireCoachRewardEnhanceRate = 0; // 全体特訓報酬発生率アップ（全体）。特訓が発生した際、特訓報酬の発生率が上昇する。
		public CharaVariableTrainerStatusCommon entireCoachStatusEnhanceRateMap = null; // 特訓発生時獲得ステータスアップ（全体）（万分率）。特訓が発生している全ての練習で、獲得する特定のステータスが割合でアップする
		public CharaVariableTrainerLotteryProcess lotteryProcessJson = null; // 抽選の際に使用した中間情報
		public bool isLocked = false; // 売却ロック
		public bool isReLotteryLocked = false; // 再抽選ロック
		public long rankNumber = 0; // ランク番号
		public long lotteryRarityValue = 0; // 抽選レア指数（抽選結果自体の強さ・珍しさを合算した数値）
		public string createdAt = ""; // レコード生成日時（=獲得日時）

   }
   
}