//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainingStatusMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(2)]
		public bool _isUnique {get{ return isUnique;} set{ this.isUnique = value;}}
		[MessagePack.Key(3)]
		public string _uniqueName {get{ return uniqueName;} set{ this.uniqueName = value;}}
		[MessagePack.Key(4)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(5)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(6)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(7)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}
		[MessagePack.Key(9)]
		public long[] _typeList {get{ return typeList;} set{ this.typeList = value;}}
		[MessagePack.Key(10)]
		public long[] _valueList {get{ return valueList;} set{ this.valueList = value;}}
		[MessagePack.Key(11)]
		public string _firstParamAddMap {get{ return firstParamAddMap;} set{ this.firstParamAddMap = value;}}
		[MessagePack.Key(12)]
		public string _practiceParamAddMap {get{ return practiceParamAddMap;} set{ this.practiceParamAddMap = value;}}
		[MessagePack.Key(13)]
		public long _practiceParamEnhanceRate {get{ return practiceParamEnhanceRate;} set{ this.practiceParamEnhanceRate = value;}}
		[MessagePack.Key(14)]
		public long _battleParamEnhanceRate {get{ return battleParamEnhanceRate;} set{ this.battleParamEnhanceRate = value;}}
		[MessagePack.Key(15)]
		public long _conditionDiscountPractice {get{ return conditionDiscountPractice;} set{ this.conditionDiscountPractice = value;}}
		[MessagePack.Key(16)]
		public long _conditionDiscountFusion {get{ return conditionDiscountFusion;} set{ this.conditionDiscountFusion = value;}}
		[MessagePack.Key(17)]
		public long _rarePracticeEnhanceRate {get{ return rarePracticeEnhanceRate;} set{ this.rarePracticeEnhanceRate = value;}}
		[MessagePack.Key(18)]
		public long _skillLevelUp {get{ return skillLevelUp;} set{ this.skillLevelUp = value;}}
		[MessagePack.Key(19)]
		public long _practiceLevelEnhance {get{ return practiceLevelEnhance;} set{ this.practiceLevelEnhance = value;}}
		[MessagePack.Key(20)]
		public long _popRateUp {get{ return popRateUp;} set{ this.popRateUp = value;}}
		[MessagePack.Key(21)]
		public long _specialRateUp {get{ return specialRateUp;} set{ this.specialRateUp = value;}}
		[MessagePack.Key(22)]
		public long _conditionRecoverUp {get{ return conditionRecoverUp;} set{ this.conditionRecoverUp = value;}}
		[MessagePack.Key(23)]
		public long _eventStatusEnhanceRate {get{ return eventStatusEnhanceRate;} set{ this.eventStatusEnhanceRate = value;}}
		[MessagePack.Key(24)]
		public long _practiceExpEnhanceRate {get{ return practiceExpEnhanceRate;} set{ this.practiceExpEnhanceRate = value;}}
		[MessagePack.Key(25)]
		public string _battleParamEnhanceMap {get{ return battleParamEnhanceMap;} set{ this.battleParamEnhanceMap = value;}}
		[MessagePack.Key(26)]
		public string _eventStatusEnhanceMap {get{ return eventStatusEnhanceMap;} set{ this.eventStatusEnhanceMap = value;}}
		[MessagePack.Key(27)]
		public string _conditionEffectGradeUpMapOnType {get{ return conditionEffectGradeUpMapOnType;} set{ this.conditionEffectGradeUpMapOnType = value;}}
		[MessagePack.Key(28)]
		public string _practiceParamAddBonusMap {get{ return practiceParamAddBonusMap;} set{ this.practiceParamAddBonusMap = value;}}
		[MessagePack.Key(29)]
		public string _practiceParamEnhanceMapOnType {get{ return practiceParamEnhanceMapOnType;} set{ this.practiceParamEnhanceMapOnType = value;}}
		[MessagePack.Key(30)]
		public string _rarePracticeEnhanceRateMapOnType {get{ return rarePracticeEnhanceRateMapOnType;} set{ this.rarePracticeEnhanceRateMapOnType = value;}}
		[MessagePack.Key(31)]
		public string _popRateEnhanceMapOnType {get{ return popRateEnhanceMapOnType;} set{ this.popRateEnhanceMapOnType = value;}}
		[MessagePack.Key(32)]
		public string _firstMTrainingEventRewardIdList {get{ return firstMTrainingEventRewardIdList;} set{ this.firstMTrainingEventRewardIdList = value;}}
		[MessagePack.Key(33)]
		public long _conditionEffectGradeUpRate {get{ return conditionEffectGradeUpRate;} set{ this.conditionEffectGradeUpRate = value;}}
		[MessagePack.Key(34)]
		public string _practiceParamRateMap {get{ return practiceParamRateMap;} set{ this.practiceParamRateMap = value;}}
		[MessagePack.Key(35)]
		public long _conditionDiscountRate {get{ return conditionDiscountRate;} set{ this.conditionDiscountRate = value;}}
		[MessagePack.Key(36)]
		public string _practiceConcentrationEnhanceRateMapOnGrade {get{ return practiceConcentrationEnhanceRateMapOnGrade;} set{ this.practiceConcentrationEnhanceRateMapOnGrade = value;}}
		[MessagePack.Key(37)]
		public string _practiceConcentrationEnhanceRateMap {get{ return practiceConcentrationEnhanceRateMap;} set{ this.practiceConcentrationEnhanceRateMap = value;}}
		[MessagePack.Key(38)]
		public string _concentrationEnhanceRateMapOnGrade {get{ return concentrationEnhanceRateMapOnGrade;} set{ this.concentrationEnhanceRateMapOnGrade = value;}}
		[MessagePack.Key(39)]
		public string _concentrationEnhanceRateMap {get{ return concentrationEnhanceRateMap;} set{ this.concentrationEnhanceRateMap = value;}}
		[MessagePack.Key(40)]
		public long _inspireEnhanceRate {get{ return inspireEnhanceRate;} set{ this.inspireEnhanceRate = value;}}
		[MessagePack.Key(41)]
		public string _correctionEnhanceRateMap {get{ return correctionEnhanceRateMap;} set{ this.correctionEnhanceRateMap = value;}}
		[MessagePack.Key(42)]
		public string _firstParamRateMap {get{ return firstParamRateMap;} set{ this.firstParamRateMap = value;}}
		[MessagePack.Key(43)]
		public string _firstParamAddRateMap {get{ return firstParamAddRateMap;} set{ this.firstParamAddRateMap = value;}}
		[MessagePack.Key(44)]
		public string _practiceParamRateBonusMap {get{ return practiceParamRateBonusMap;} set{ this.practiceParamRateBonusMap = value;}}
		[MessagePack.Key(45)]
		public long _getAbilityEnhanceRate {get{ return getAbilityEnhanceRate;} set{ this.getAbilityEnhanceRate = value;}}
		[MessagePack.Key(46)]
		public long _practiceSkillLevelUpRate {get{ return practiceSkillLevelUpRate;} set{ this.practiceSkillLevelUpRate = value;}}
		[MessagePack.Key(47)]
		public string _eventStatusParamAddMap {get{ return eventStatusParamAddMap;} set{ this.eventStatusParamAddMap = value;}}
		[MessagePack.Key(48)]
		public long _eventSkillLevelUpRate {get{ return eventSkillLevelUpRate;} set{ this.eventSkillLevelUpRate = value;}}
		[MessagePack.Key(49)]
		public long _concentrationEnhanceRate {get{ return concentrationEnhanceRate;} set{ this.concentrationEnhanceRate = value;}}
		[MessagePack.Key(50)]
		public long _traineeJoinBonusEnhanceRate {get{ return traineeJoinBonusEnhanceRate;} set{ this.traineeJoinBonusEnhanceRate = value;}}
		[MessagePack.Key(51)]
		public long _coachEnhanceRate {get{ return coachEnhanceRate;} set{ this.coachEnhanceRate = value;}}
		[MessagePack.Key(52)]
		public string _coachEnhanceRateMapOnType {get{ return coachEnhanceRateMapOnType;} set{ this.coachEnhanceRateMapOnType = value;}}
		[MessagePack.Key(53)]
		public long _highRankCoachRate {get{ return highRankCoachRate;} set{ this.highRankCoachRate = value;}}
		[MessagePack.Key(54)]
		public long _coachRewardEnhanceRate {get{ return coachRewardEnhanceRate;} set{ this.coachRewardEnhanceRate = value;}}
		[MessagePack.Key(55)]
		public long _entireCoachRewardEnhanceRate {get{ return entireCoachRewardEnhanceRate;} set{ this.entireCoachRewardEnhanceRate = value;}}
		[MessagePack.Key(56)]
		public long _trainingPointEnhanceRate {get{ return trainingPointEnhanceRate;} set{ this.trainingPointEnhanceRate = value;}}
		[MessagePack.Key(57)]
		public long _trainingPointLevelCostDiscountRate {get{ return trainingPointLevelCostDiscountRate;} set{ this.trainingPointLevelCostDiscountRate = value;}}
		[MessagePack.Key(58)]
		public long _handResetCostDiscountRate {get{ return handResetCostDiscountRate;} set{ this.handResetCostDiscountRate = value;}}
		[MessagePack.Key(59)]
		public string _coachStatusEnhanceRateMap {get{ return coachStatusEnhanceRateMap;} set{ this.coachStatusEnhanceRateMap = value;}}
		[MessagePack.Key(60)]
		public string _entireCoachStatusEnhanceRateMap {get{ return entireCoachStatusEnhanceRateMap;} set{ this.entireCoachStatusEnhanceRateMap = value;}}
		[MessagePack.Key(61)]
		public string _additionConditionEffectGradeUpMapOnType {get{ return additionConditionEffectGradeUpMapOnType;} set{ this.additionConditionEffectGradeUpMapOnType = value;}}
		[MessagePack.Key(62)]
		public string _addConcentrationExp {get{ return addConcentrationExp;} set{ this.addConcentrationExp = value;}}
		[MessagePack.Key(63)]
		public string _addConcentrationExpRate {get{ return addConcentrationExpRate;} set{ this.addConcentrationExpRate = value;}}
		[MessagePack.Key(64)]
		public string _addConcentrationTotalExpRate {get{ return addConcentrationTotalExpRate;} set{ this.addConcentrationTotalExpRate = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId シナリオID
		[UnityEngine.SerializeField] bool isUnique = false; // $isUnique 1: 固有練習能力、2: 通常練習能力
		[UnityEngine.SerializeField] string uniqueName = ""; // $uniqueName 固有練習能力名
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId mChara のID
		[UnityEngine.SerializeField] long level = 0; // $level 倍率強化レベル。レベルごとに1レコード定義。存在しなければ、それより小さいレベルの中で最も大きいレコードを1つ探す。<br>固有練習能力は level = 解放レベル とし、1キャラに1レコード。
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 開始時刻。トレーニング開始時刻が期間内に入っていれば効果が有効となる
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 終了時刻。トレーニング開始時刻が期間内に入っていれば効果が有効となる
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除
		[UnityEngine.SerializeField] long[] typeList = null; // $typeList このレコードが持つ練習能力タイプ（m_training_status_type_detail.type）を詰めたjson配列。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		[UnityEngine.SerializeField] long[] valueList = null; // $valueList このレコードが持つ練習能力の効果量を詰めたjson配列。値の順番はtypeListに対応する。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		[UnityEngine.SerializeField] string firstParamAddMap = ""; // $firstParamAddMap 初期◯◯アップ（実数値）。ステータス初期値に加算される
		[UnityEngine.SerializeField] string practiceParamAddMap = ""; // $practiceParamAddMap 練習時◯◯ボーナス（万分率）。そのキャラがいる練習の時に、獲得する特定のステータスが割合でアップする
		[UnityEngine.SerializeField] long practiceParamEnhanceRate = 0; // $practiceParamEnhanceRate 練習ボーナス（万分率）。そのキャラがいる練習の時に、獲得するステータスが割合でアップする
		[UnityEngine.SerializeField] long battleParamEnhanceRate = 0; // $battleParamEnhanceRate 練習試合ボーナス（万分率）。練習試合で得られるステータスが割合でアップする
		[UnityEngine.SerializeField] long conditionDiscountPractice = 0; // $conditionDiscountPractice 練習コンディション消費削減（実数値）。そのキャラがいる練習の時に、消費するコンディションを軽減する
		[UnityEngine.SerializeField] long conditionDiscountFusion = 0; // $conditionDiscountFusion 合成コンディション消費削減（実数値）
		[UnityEngine.SerializeField] long rarePracticeEnhanceRate = 0; // $rarePracticeEnhanceRate レア練習率アップ（万分率）。1+rate/10000 がレア練習カードの重みに乗算される
		[UnityEngine.SerializeField] long skillLevelUp = 0; // $skillLevelUp スキルレベルアップ。そのキャラが所持する練習カードとサポートキャライベントから得られるスキルのレベルが加算される
		[UnityEngine.SerializeField] long practiceLevelEnhance = 0; // $practiceLevelEnhance 練習レベルボーナス。そのキャラが所持する練習メニューカードの初期レベルがアップする
		[UnityEngine.SerializeField] long popRateUp = 0; // $popRateUp レクチャー率アップ（万分率）。0を基準とし、10000で自分の所持練習カードにしか登場しなくなる
		[UnityEngine.SerializeField] long specialRateUp = 0; // $specialRateUp スペシャルレクチャーボーナス（万分率）。スペシャルレクチャーが発生した際に得られるステータスが割合でアップする
		[UnityEngine.SerializeField] long conditionRecoverUp = 0; // $conditionRecoverUp コンディション回復アップ（実数値）。そのキャラがいる練習のとき、コンディションが回復する練習だと回復量に加算される
		[UnityEngine.SerializeField] long eventStatusEnhanceRate = 0; // $eventStatusEnhanceRate イベントステータスアップ（万分率）。そのキャラのサポートキャライベントで得られる獲得ステータス量がアップ
		[UnityEngine.SerializeField] long practiceExpEnhanceRate = 0; // $practiceExpEnhanceRate 練習メニュー経験値獲得量アップ（万分率）。そのキャラがいる練習のとき、獲得できるカード経験値獲得量が割合でアップする
		[UnityEngine.SerializeField] string battleParamEnhanceMap = ""; // $battleParamEnhanceMap 練習試合時◯◯ボーナス（万分率）。練習試合で得られるステータスがパラメータ別に割合でアップする。battleParamEnhanceRate とは加算
		[UnityEngine.SerializeField] string eventStatusEnhanceMap = ""; // $eventStatusEnhanceMap イベント時◯◯ボーナス（万分率）。そのキャラのサポートキャライベントで得られるステータスがパラメータ別に割合でアップする。
		[UnityEngine.SerializeField] string conditionEffectGradeUpMapOnType = ""; // $conditionEffectGradeUpMapOnType xx実施時効果ボーナス上昇。練習実施時に発生するボーナス率に、指定確率でn段階の上昇補正がかかる。[{practiceType:0,grade:2,rate:1000},{practiceType:0,grade:1,rate:2000},{practiceType:1,grade:1,rate:1000}] のように指定する
		[UnityEngine.SerializeField] string practiceParamAddBonusMap = ""; // $practiceParamAddBonusMap 練習時固定ボーナス（実数値）。実施する練習の種別に関わらず、トレーニング実施時に特定ステータスを追加で獲得する。
		[UnityEngine.SerializeField] string practiceParamEnhanceMapOnType = ""; // $practiceParamEnhanceMapOnType xx実施時◯◯獲得量アップ（万分率）。特定の練習種別の練習を実施時に特定のステータスの獲得量が割合でアップする。[{practiceType:0,rateMap:{param1:1000,param2:500}},{practiceType:1,rateMap:{spd:2000}}] のように指定する
		[UnityEngine.SerializeField] string rarePracticeEnhanceRateMapOnType = ""; // $rarePracticeEnhanceRateMapOnType xxのレア練習出現率アップ（万分率）。特定の練習種別においてレア練習カードの重みに 1+rate/10000 を乗算し、レア練習出現率をアップする。rarePracticeEnhanceRate とは加算。[{practiceType:0,rate:1500},{practiceType:1,rate:1000}] のように指定する
		[UnityEngine.SerializeField] string popRateEnhanceMapOnType = ""; // $popRateEnhanceMapOnType xxのレクチャー率アップ（万分率）。特定の練習種別において、レア練習カードが採用された場合にカードの持ち主が参加する確率が上昇する
		[UnityEngine.SerializeField] string firstMTrainingEventRewardIdList = ""; // $firstMTrainingEventRewardIdList 初期イベント報酬指定。mTrainingEventRewardId を指定すると、既に該当の報酬を受け取った状態でトレーニングを開始する。指定しない場合は []
		[UnityEngine.SerializeField] long conditionEffectGradeUpRate = 0; // $conditionEffectGradeUpRate 練習実行時のボーナス値について、1つ上の倍率に切り替わる際の発生確率（万分率）常時発動
		[UnityEngine.SerializeField] string practiceParamRateMap = ""; // $practiceParamRateMap 効果量アップ倍率（万分率）。practiceParamAddMapと同じ形式で記載。指定ステータスに対して乗算される。常時発動
		[UnityEngine.SerializeField] long conditionDiscountRate = 0; // $conditionDiscountRate コンディションに対して軽減する倍率（万分率）
		[UnityEngine.SerializeField] string practiceConcentrationEnhanceRateMapOnGrade = ""; // $practiceConcentrationEnhanceRateMapOnGrade コンセントレーションのグレードを指定した発生率の加算。練習に参加している場合に発動。例：[{grade:1,rate:1500}]
		[UnityEngine.SerializeField] string practiceConcentrationEnhanceRateMap = ""; // $practiceConcentrationEnhanceRateMap 特定のコンセントレーションを指定した発生率の加算。練習に参加している場合に発動。例：[{id:1,rate:1500}]
		[UnityEngine.SerializeField] string concentrationEnhanceRateMapOnGrade = ""; // $concentrationEnhanceRateMapOnGrade コンセントレーションのグレードを指定した発生率の加算（常時発動）。例：[{grade:1,rate:1500}]
		[UnityEngine.SerializeField] string concentrationEnhanceRateMap = ""; // $concentrationEnhanceRateMap 特定のコンセントレーションを指定した発生率の加算（常時発動）。例：[{id:1,rate:1500}]
		[UnityEngine.SerializeField] long inspireEnhanceRate = 0; // $inspireEnhanceRate インスピレーション発生率の加算
		[UnityEngine.SerializeField] string correctionEnhanceRateMap = ""; // $correctionEnhanceRateMap 成長率加算（万分率）。例：{"hp": 1000, "sp": 2000}。1+rate/10000
		[UnityEngine.SerializeField] string firstParamRateMap = ""; // $firstParamRateMap 初期◯◯アップ（万分率）。ステータス初期値に乗算される。例：{"hp": 1000, "sp": 2000}。1+rate/10000
		[UnityEngine.SerializeField] string firstParamAddRateMap = ""; // $firstParamAddRateMap firstParamAddMapに対して乗算される。例：{"hp": 1000, "sp": 2000}。1+rate/10000
		[UnityEngine.SerializeField] string practiceParamRateBonusMap = ""; // $practiceParamRateBonusMap 練習時固定ボーナス（万分率）。実施する練習の種別に関わらず、トレーニング実施時に特定ステータスに乗算される。例：{"hp": 1000, "sp": 2000}。1+rate/10000
		[UnityEngine.SerializeField] long getAbilityEnhanceRate = 0; // $getAbilityEnhanceRate 練習カード実行時のスキル獲得率アップ（万分率）
		[UnityEngine.SerializeField] long practiceSkillLevelUpRate = 0; // $practiceSkillLevelUpRate 練習カード実行時のスキルレベルが+1される確率（万分率）
		[UnityEngine.SerializeField] string eventStatusParamAddMap = ""; // $eventStatusParamAddMap イベントステータスアップ（実数）。そのキャラのサポートキャライベントで得られる獲得ステータス量がアップ。例：{"hp": 100, "sp": 200}
		[UnityEngine.SerializeField] long eventSkillLevelUpRate = 0; // $eventSkillLevelUpRate そのキャラのサポートキャライベントで獲得できるスキルレベルが+1される確率（万分率）
		[UnityEngine.SerializeField] long concentrationEnhanceRate = 0; // $concentrationEnhanceRate 特定のコンセントレーションを指定した発生率の加算（万分率）
		[UnityEngine.SerializeField] long traineeJoinBonusEnhanceRate = 0; // $traineeJoinBonusEnhanceRate 育成対象キャラが参加している練習メニューカード実行時のボーナス加算倍率（万分率）
		[UnityEngine.SerializeField] long coachEnhanceRate = 0; // $coachEnhanceRate 特訓発生率アップ。そのキャラによる特訓の発生率が上昇する（万分率）
		[UnityEngine.SerializeField] string coachEnhanceRateMapOnType = ""; // $coachEnhanceRateMapOnType xx実施時特訓発生率アップ。特定の練習種別において、特訓の発生率が上昇する
		[UnityEngine.SerializeField] long highRankCoachRate = 0; // $highRankCoachRate 特訓高ランク率アップ。そのキャラがいる練習において高ランクの特訓の発生率が上昇する（万分率）
		[UnityEngine.SerializeField] long coachRewardEnhanceRate = 0; // $coachRewardEnhanceRate キャラごと特訓報酬の発生率アップ。そのキャラの特訓が発生した際、特訓報酬の発生率が上昇する（万分率）
		[UnityEngine.SerializeField] long entireCoachRewardEnhanceRate = 0; // $entireCoachRewardEnhanceRate 全体特訓報酬発生率アップ（全体）。特訓が発生した際、特訓報酬の発生率が上昇する。
		[UnityEngine.SerializeField] long trainingPointEnhanceRate = 0; // $trainingPointEnhanceRate m_training_point_convertの獲得量アップ（万分率）
		[UnityEngine.SerializeField] long trainingPointLevelCostDiscountRate = 0; // $trainingPointLevelCostDiscountRate m_training_point_status_levelの消費量削減（万分率）
		[UnityEngine.SerializeField] long handResetCostDiscountRate = 0; // $handResetCostDiscountRate m_training_point_hand_reset_costの消費量削減（万分率）
		[UnityEngine.SerializeField] string coachStatusEnhanceRateMap = ""; // $coachStatusEnhanceRateMap 特訓発生時獲得ステータスアップ（万分率）。そのキャラが参加している練習において特訓が発生している場合、獲得する特定のステータスが割合でアップする
		[UnityEngine.SerializeField] string entireCoachStatusEnhanceRateMap = ""; // $entireCoachStatusEnhanceRateMap 特訓発生時獲得ステータスアップ（全体）（万分率）。特訓が発生している全ての練習で、獲得する特定のステータスが割合でアップする
		[UnityEngine.SerializeField] string additionConditionEffectGradeUpMapOnType = ""; // $additionConditionEffectGradeUpMapOnType xx実施時効果ボーナス上昇。m_training_card_condition_effectのtype=2を取得。練習実施時に発生するボーナス率に、指定確率でn段階の上昇補正がかかる。canDuplicate=1の場合は重複して取得可能。[{"practiceType":0,"grade":2,"rate":1000,"canDuplicate":1},{"practiceType":0,"grade":1,"rate":2000,"canDuplicate":2},{"practiceType":1,"grade":1,"rate":1000,"canDuplicate":1}] のように指定する
		[UnityEngine.SerializeField] string addConcentrationExp = ""; // $addConcentrationExp concentrationの経験値獲得アップ（実数）
		[UnityEngine.SerializeField] string addConcentrationExpRate = ""; // $addConcentrationExpRate concentrationの経験値獲得率アップ（万分率）
		[UnityEngine.SerializeField] string addConcentrationTotalExpRate = ""; // $addConcentrationTotalExpRate concentrationの合計経験値率アップ（万分率）

    }

    public class CharaTrainingStatusMasterObjectBase {
		public virtual CharaTrainingStatusMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual bool isUnique => _rawData._isUnique;
		public virtual string uniqueName => _rawData._uniqueName;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long level => _rawData._level;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual bool deleteFlg => _rawData._deleteFlg;
		public virtual long[] typeList => _rawData._typeList;
		public virtual long[] valueList => _rawData._valueList;
		public virtual string firstParamAddMap => _rawData._firstParamAddMap;
		public virtual string practiceParamAddMap => _rawData._practiceParamAddMap;
		public virtual long practiceParamEnhanceRate => _rawData._practiceParamEnhanceRate;
		public virtual long battleParamEnhanceRate => _rawData._battleParamEnhanceRate;
		public virtual long conditionDiscountPractice => _rawData._conditionDiscountPractice;
		public virtual long conditionDiscountFusion => _rawData._conditionDiscountFusion;
		public virtual long rarePracticeEnhanceRate => _rawData._rarePracticeEnhanceRate;
		public virtual long skillLevelUp => _rawData._skillLevelUp;
		public virtual long practiceLevelEnhance => _rawData._practiceLevelEnhance;
		public virtual long popRateUp => _rawData._popRateUp;
		public virtual long specialRateUp => _rawData._specialRateUp;
		public virtual long conditionRecoverUp => _rawData._conditionRecoverUp;
		public virtual long eventStatusEnhanceRate => _rawData._eventStatusEnhanceRate;
		public virtual long practiceExpEnhanceRate => _rawData._practiceExpEnhanceRate;
		public virtual string battleParamEnhanceMap => _rawData._battleParamEnhanceMap;
		public virtual string eventStatusEnhanceMap => _rawData._eventStatusEnhanceMap;
		public virtual string conditionEffectGradeUpMapOnType => _rawData._conditionEffectGradeUpMapOnType;
		public virtual string practiceParamAddBonusMap => _rawData._practiceParamAddBonusMap;
		public virtual string practiceParamEnhanceMapOnType => _rawData._practiceParamEnhanceMapOnType;
		public virtual string rarePracticeEnhanceRateMapOnType => _rawData._rarePracticeEnhanceRateMapOnType;
		public virtual string popRateEnhanceMapOnType => _rawData._popRateEnhanceMapOnType;
		public virtual string firstMTrainingEventRewardIdList => _rawData._firstMTrainingEventRewardIdList;
		public virtual long conditionEffectGradeUpRate => _rawData._conditionEffectGradeUpRate;
		public virtual string practiceParamRateMap => _rawData._practiceParamRateMap;
		public virtual long conditionDiscountRate => _rawData._conditionDiscountRate;
		public virtual string practiceConcentrationEnhanceRateMapOnGrade => _rawData._practiceConcentrationEnhanceRateMapOnGrade;
		public virtual string practiceConcentrationEnhanceRateMap => _rawData._practiceConcentrationEnhanceRateMap;
		public virtual string concentrationEnhanceRateMapOnGrade => _rawData._concentrationEnhanceRateMapOnGrade;
		public virtual string concentrationEnhanceRateMap => _rawData._concentrationEnhanceRateMap;
		public virtual long inspireEnhanceRate => _rawData._inspireEnhanceRate;
		public virtual string correctionEnhanceRateMap => _rawData._correctionEnhanceRateMap;
		public virtual string firstParamRateMap => _rawData._firstParamRateMap;
		public virtual string firstParamAddRateMap => _rawData._firstParamAddRateMap;
		public virtual string practiceParamRateBonusMap => _rawData._practiceParamRateBonusMap;
		public virtual long getAbilityEnhanceRate => _rawData._getAbilityEnhanceRate;
		public virtual long practiceSkillLevelUpRate => _rawData._practiceSkillLevelUpRate;
		public virtual string eventStatusParamAddMap => _rawData._eventStatusParamAddMap;
		public virtual long eventSkillLevelUpRate => _rawData._eventSkillLevelUpRate;
		public virtual long concentrationEnhanceRate => _rawData._concentrationEnhanceRate;
		public virtual long traineeJoinBonusEnhanceRate => _rawData._traineeJoinBonusEnhanceRate;
		public virtual long coachEnhanceRate => _rawData._coachEnhanceRate;
		public virtual string coachEnhanceRateMapOnType => _rawData._coachEnhanceRateMapOnType;
		public virtual long highRankCoachRate => _rawData._highRankCoachRate;
		public virtual long coachRewardEnhanceRate => _rawData._coachRewardEnhanceRate;
		public virtual long entireCoachRewardEnhanceRate => _rawData._entireCoachRewardEnhanceRate;
		public virtual long trainingPointEnhanceRate => _rawData._trainingPointEnhanceRate;
		public virtual long trainingPointLevelCostDiscountRate => _rawData._trainingPointLevelCostDiscountRate;
		public virtual long handResetCostDiscountRate => _rawData._handResetCostDiscountRate;
		public virtual string coachStatusEnhanceRateMap => _rawData._coachStatusEnhanceRateMap;
		public virtual string entireCoachStatusEnhanceRateMap => _rawData._entireCoachStatusEnhanceRateMap;
		public virtual string additionConditionEffectGradeUpMapOnType => _rawData._additionConditionEffectGradeUpMapOnType;
		public virtual string addConcentrationExp => _rawData._addConcentrationExp;
		public virtual string addConcentrationExpRate => _rawData._addConcentrationExpRate;
		public virtual string addConcentrationTotalExpRate => _rawData._addConcentrationTotalExpRate;

        CharaTrainingStatusMasterValueObject _rawData = null;
		public CharaTrainingStatusMasterObjectBase( CharaTrainingStatusMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainingStatusMasterObject : CharaTrainingStatusMasterObjectBase, IMasterObject {
		public CharaTrainingStatusMasterObject( CharaTrainingStatusMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainingStatusMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Training_Status;

        [UnityEngine.SerializeField]
        CharaTrainingStatusMasterValueObject[] m_Chara_Training_Status = null;
    }


}
