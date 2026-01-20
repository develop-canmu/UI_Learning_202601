//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaTrainerLotteryStatusMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(3)]
		public long[] _typeList {get{ return typeList;} set{ this.typeList = value;}}
		[MessagePack.Key(4)]
		public long[] _valueList {get{ return valueList;} set{ this.valueList = value;}}
		[MessagePack.Key(5)]
		public string _firstParamAddMap {get{ return firstParamAddMap;} set{ this.firstParamAddMap = value;}}
		[MessagePack.Key(6)]
		public long _battleParamEnhanceRate {get{ return battleParamEnhanceRate;} set{ this.battleParamEnhanceRate = value;}}
		[MessagePack.Key(7)]
		public long _rarePracticeEnhanceRate {get{ return rarePracticeEnhanceRate;} set{ this.rarePracticeEnhanceRate = value;}}
		[MessagePack.Key(8)]
		public string _battleParamEnhanceMap {get{ return battleParamEnhanceMap;} set{ this.battleParamEnhanceMap = value;}}
		[MessagePack.Key(9)]
		public string _conditionEffectGradeUpMapOnType {get{ return conditionEffectGradeUpMapOnType;} set{ this.conditionEffectGradeUpMapOnType = value;}}
		[MessagePack.Key(10)]
		public string _practiceParamAddBonusMap {get{ return practiceParamAddBonusMap;} set{ this.practiceParamAddBonusMap = value;}}
		[MessagePack.Key(11)]
		public string _practiceParamEnhanceMapOnType {get{ return practiceParamEnhanceMapOnType;} set{ this.practiceParamEnhanceMapOnType = value;}}
		[MessagePack.Key(12)]
		public string _rarePracticeEnhanceRateMapOnType {get{ return rarePracticeEnhanceRateMapOnType;} set{ this.rarePracticeEnhanceRateMapOnType = value;}}
		[MessagePack.Key(13)]
		public string _popRateEnhanceMapOnType {get{ return popRateEnhanceMapOnType;} set{ this.popRateEnhanceMapOnType = value;}}
		[MessagePack.Key(14)]
		public string _firstMTrainingEventRewardIdList {get{ return firstMTrainingEventRewardIdList;} set{ this.firstMTrainingEventRewardIdList = value;}}
		[MessagePack.Key(15)]
		public string _practiceParamRateMap {get{ return practiceParamRateMap;} set{ this.practiceParamRateMap = value;}}
		[MessagePack.Key(16)]
		public string _coachEnhanceRateMapOnType {get{ return coachEnhanceRateMapOnType;} set{ this.coachEnhanceRateMapOnType = value;}}
		[MessagePack.Key(17)]
		public long _entireCoachRewardEnhanceRate {get{ return entireCoachRewardEnhanceRate;} set{ this.entireCoachRewardEnhanceRate = value;}}
		[MessagePack.Key(18)]
		public string _entireCoachStatusEnhanceRateMap {get{ return entireCoachStatusEnhanceRateMap;} set{ this.entireCoachStatusEnhanceRateMap = value;}}
		[MessagePack.Key(19)]
		public long _trainingPointEnhanceRate {get{ return trainingPointEnhanceRate;} set{ this.trainingPointEnhanceRate = value;}}
		[MessagePack.Key(20)]
		public long _trainingPointLevelCostDiscountRate {get{ return trainingPointLevelCostDiscountRate;} set{ this.trainingPointLevelCostDiscountRate = value;}}
		[MessagePack.Key(21)]
		public long _handResetCostDiscountRate {get{ return handResetCostDiscountRate;} set{ this.handResetCostDiscountRate = value;}}
		[MessagePack.Key(22)]
		public string _additionConditionEffectGradeUpMapOnType {get{ return additionConditionEffectGradeUpMapOnType;} set{ this.additionConditionEffectGradeUpMapOnType = value;}}
		[MessagePack.Key(23)]
		public long _eventSkillLevelUpRate {get{ return eventSkillLevelUpRate;} set{ this.eventSkillLevelUpRate = value;}}
		[MessagePack.Key(24)]
		public long _practiceSkillLevelUpRate {get{ return practiceSkillLevelUpRate;} set{ this.practiceSkillLevelUpRate = value;}}
		[MessagePack.Key(25)]
		public long _getAbilityEnhanceRate {get{ return getAbilityEnhanceRate;} set{ this.getAbilityEnhanceRate = value;}}
		[MessagePack.Key(26)]
		public string _correctionEnhanceRateMap {get{ return correctionEnhanceRateMap;} set{ this.correctionEnhanceRateMap = value;}}
		[MessagePack.Key(27)]
		public long _lotteryRarityValue {get{ return lotteryRarityValue;} set{ this.lotteryRarityValue = value;}}
		[MessagePack.Key(28)]
		public bool _isReloadOverwrite {get{ return isReloadOverwrite;} set{ this.isReloadOverwrite = value;}}
		[MessagePack.Key(29)]
		public string _addConcentrationExp {get{ return addConcentrationExp;} set{ this.addConcentrationExp = value;}}
		[MessagePack.Key(30)]
		public string _addConcentrationExpRate {get{ return addConcentrationExpRate;} set{ this.addConcentrationExpRate = value;}}
		[MessagePack.Key(31)]
		public string _addConcentrationTotalExpRate {get{ return addConcentrationTotalExpRate;} set{ this.addConcentrationTotalExpRate = value;}}
		[MessagePack.Key(32)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理表示名
		[UnityEngine.SerializeField] long[] typeList = null; // $typeList このレコードが持つ練習能力タイプ（m_training_status_type_detail.type）を詰めたjson配列。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		[UnityEngine.SerializeField] long[] valueList = null; // $valueList このレコードが持つ練習能力の効果量を詰めたjson配列。値の順番はtypeListに対応する。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		[UnityEngine.SerializeField] string firstParamAddMap = ""; // $firstParamAddMap 初期◯◯アップ（実数値）。ステータス初期値に加算される
		[UnityEngine.SerializeField] long battleParamEnhanceRate = 0; // $battleParamEnhanceRate 練習試合ボーナス（万分率）。練習試合で得られるステータスが割合でアップする
		[UnityEngine.SerializeField] long rarePracticeEnhanceRate = 0; // $rarePracticeEnhanceRate レア練習率アップ（万分率）。1+rate/10000 がレア練習カードの重みに乗算される
		[UnityEngine.SerializeField] string battleParamEnhanceMap = ""; // $battleParamEnhanceMap 練習試合時◯◯ボーナス（万分率）。練習試合で得られるステータスがパラメータ別に割合でアップする。battleParamEnhanceRate とは加算
		[UnityEngine.SerializeField] string conditionEffectGradeUpMapOnType = ""; // $conditionEffectGradeUpMapOnType xx実施時効果ボーナス上昇。練習実施時に発生するボーナス率に、指定確率でn段階の上昇補正がかかる。[{practiceType:0,grade:2,rate:1000},{practiceType:0,grade:1,rate:2000},{practiceType:1,grade:1,rate:1000}] のように指定する
		[UnityEngine.SerializeField] string practiceParamAddBonusMap = ""; // $practiceParamAddBonusMap 練習時固定ボーナス（実数値）。実施する練習の種別に関わらず、トレーニング実施時に特定ステータスを追加で獲得する。
		[UnityEngine.SerializeField] string practiceParamEnhanceMapOnType = ""; // $practiceParamEnhanceMapOnType xx実施時◯◯獲得量アップ（万分率）。特定の練習種別の練習を実施時に特定のステータスの獲得量が割合でアップする。[{practiceType:0,rateMap:{param1:1000,param2:500}},{practiceType:1,rateMap:{spd:2000}}] のように指定する
		[UnityEngine.SerializeField] string rarePracticeEnhanceRateMapOnType = ""; // $rarePracticeEnhanceRateMapOnType xxのレア練習出現率アップ（万分率）。特定の練習種別においてレア練習カードの重みに 1+rate/10000 を乗算し、レア練習出現率をアップする。rarePracticeEnhanceRate とは加算。[{practiceType:0,rate:1500},{practiceType:1,rate:1000}] のように指定する
		[UnityEngine.SerializeField] string popRateEnhanceMapOnType = ""; // $popRateEnhanceMapOnType xxのレクチャー率アップ（万分率）。特定の練習種別において、レア練習カードが採用された場合にカードの持ち主が参加する確率が上昇する
		[UnityEngine.SerializeField] string firstMTrainingEventRewardIdList = ""; // $firstMTrainingEventRewardIdList 初期イベント報酬指定。mTrainingEventRewardId を指定すると、既に該当の報酬を受け取った状態でトレーニングを開始する。指定しない場合は []
		[UnityEngine.SerializeField] string practiceParamRateMap = ""; // $practiceParamRateMap 効果量アップ倍率（万分率）。practiceParamAddMapと同じ形式で記載。指定ステータスに対して乗算される。常時発動
		[UnityEngine.SerializeField] string coachEnhanceRateMapOnType = ""; // $coachEnhanceRateMapOnType xx実施時特訓発生率アップ。特定の練習種別において、特訓の発生率が上昇する
		[UnityEngine.SerializeField] long entireCoachRewardEnhanceRate = 0; // $entireCoachRewardEnhanceRate 全体特訓報酬発生率アップ（全体）。特訓が発生した際、特訓報酬の発生率が上昇する。
		[UnityEngine.SerializeField] string entireCoachStatusEnhanceRateMap = ""; // $entireCoachStatusEnhanceRateMap 特訓発生時獲得ステータスアップ（全体）（万分率）。特訓が発生している全ての練習で、獲得する特定のステータスが割合でアップする
		[UnityEngine.SerializeField] long trainingPointEnhanceRate = 0; // $trainingPointEnhanceRate m_training_point_convertの獲得量アップ（万分率）
		[UnityEngine.SerializeField] long trainingPointLevelCostDiscountRate = 0; // $trainingPointLevelCostDiscountRate m_training_point_status_levelの消費量削減（万分率）
		[UnityEngine.SerializeField] long handResetCostDiscountRate = 0; // $handResetCostDiscountRate m_training_point_hand_reset_costの消費量削減（万分率）
		[UnityEngine.SerializeField] string additionConditionEffectGradeUpMapOnType = ""; // $additionConditionEffectGradeUpMapOnType xx実施時効果ボーナス上昇。m_training_card_condition_effectのtype=2を取得。練習実施時に発生するボーナス率に、指定確率でn段階の上昇補正がかかる。canDuplicate=1の場合は重複して取得可能。[{"practiceType":0,"grade":2,"rate":1000,"canDuplicate":1},{"practiceType":0,"grade":1,"rate":2000,"canDuplicate":2},{"practiceType":1,"grade":1,"rate":1000,"canDuplicate":1}] のように指定する
		[UnityEngine.SerializeField] long eventSkillLevelUpRate = 0; // $eventSkillLevelUpRate 全てのイベントで獲得できるスキルレベルが+1される確率（万分率）
		[UnityEngine.SerializeField] long practiceSkillLevelUpRate = 0; // $practiceSkillLevelUpRate 全ての練習カード実行時のスキルレベルが+1される確率（万分率）
		[UnityEngine.SerializeField] long getAbilityEnhanceRate = 0; // $getAbilityEnhanceRate 全ての練習カード実行時のスキル獲得率アップ（万分率）
		[UnityEngine.SerializeField] string correctionEnhanceRateMap = ""; // $correctionEnhanceRateMap 成長率加算（万分率）。例：{"hp": 1000, "sp": 2000}。1+rate/10000
		[UnityEngine.SerializeField] long lotteryRarityValue = 0; // $lotteryRarityValue 当選時加算レアリティポイント
		[UnityEngine.SerializeField] bool isReloadOverwrite = false; // $isReloadOverwrite 再抽選で上書きするか。1=>する、2=>しない
		[UnityEngine.SerializeField] string addConcentrationExp = ""; // $addConcentrationExp concentrationの経験値獲得アップ（実数）
		[UnityEngine.SerializeField] string addConcentrationExpRate = ""; // $addConcentrationExpRate concentrationの経験値獲得率アップ（万分率）
		[UnityEngine.SerializeField] string addConcentrationTotalExpRate = ""; // $addConcentrationTotalExpRate concentrationの合計経験値率アップ（万分率）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaTrainerLotteryStatusMasterObjectBase {
		public virtual CharaTrainerLotteryStatusMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string adminName => _rawData._adminName;
		public virtual long[] typeList => _rawData._typeList;
		public virtual long[] valueList => _rawData._valueList;
		public virtual string firstParamAddMap => _rawData._firstParamAddMap;
		public virtual long battleParamEnhanceRate => _rawData._battleParamEnhanceRate;
		public virtual long rarePracticeEnhanceRate => _rawData._rarePracticeEnhanceRate;
		public virtual string battleParamEnhanceMap => _rawData._battleParamEnhanceMap;
		public virtual string conditionEffectGradeUpMapOnType => _rawData._conditionEffectGradeUpMapOnType;
		public virtual string practiceParamAddBonusMap => _rawData._practiceParamAddBonusMap;
		public virtual string practiceParamEnhanceMapOnType => _rawData._practiceParamEnhanceMapOnType;
		public virtual string rarePracticeEnhanceRateMapOnType => _rawData._rarePracticeEnhanceRateMapOnType;
		public virtual string popRateEnhanceMapOnType => _rawData._popRateEnhanceMapOnType;
		public virtual string firstMTrainingEventRewardIdList => _rawData._firstMTrainingEventRewardIdList;
		public virtual string practiceParamRateMap => _rawData._practiceParamRateMap;
		public virtual string coachEnhanceRateMapOnType => _rawData._coachEnhanceRateMapOnType;
		public virtual long entireCoachRewardEnhanceRate => _rawData._entireCoachRewardEnhanceRate;
		public virtual string entireCoachStatusEnhanceRateMap => _rawData._entireCoachStatusEnhanceRateMap;
		public virtual long trainingPointEnhanceRate => _rawData._trainingPointEnhanceRate;
		public virtual long trainingPointLevelCostDiscountRate => _rawData._trainingPointLevelCostDiscountRate;
		public virtual long handResetCostDiscountRate => _rawData._handResetCostDiscountRate;
		public virtual string additionConditionEffectGradeUpMapOnType => _rawData._additionConditionEffectGradeUpMapOnType;
		public virtual long eventSkillLevelUpRate => _rawData._eventSkillLevelUpRate;
		public virtual long practiceSkillLevelUpRate => _rawData._practiceSkillLevelUpRate;
		public virtual long getAbilityEnhanceRate => _rawData._getAbilityEnhanceRate;
		public virtual string correctionEnhanceRateMap => _rawData._correctionEnhanceRateMap;
		public virtual long lotteryRarityValue => _rawData._lotteryRarityValue;
		public virtual bool isReloadOverwrite => _rawData._isReloadOverwrite;
		public virtual string addConcentrationExp => _rawData._addConcentrationExp;
		public virtual string addConcentrationExpRate => _rawData._addConcentrationExpRate;
		public virtual string addConcentrationTotalExpRate => _rawData._addConcentrationTotalExpRate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaTrainerLotteryStatusMasterValueObject _rawData = null;
		public CharaTrainerLotteryStatusMasterObjectBase( CharaTrainerLotteryStatusMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaTrainerLotteryStatusMasterObject : CharaTrainerLotteryStatusMasterObjectBase, IMasterObject {
		public CharaTrainerLotteryStatusMasterObject( CharaTrainerLotteryStatusMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaTrainerLotteryStatusMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Trainer_Lottery_Status;

        [UnityEngine.SerializeField]
        CharaTrainerLotteryStatusMasterValueObject[] m_Chara_Trainer_Lottery_Status = null;
    }


}
