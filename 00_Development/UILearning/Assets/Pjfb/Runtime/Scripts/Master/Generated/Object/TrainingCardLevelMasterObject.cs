//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardLevelMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingCardId {get{ return mTrainingCardId;} set{ this.mTrainingCardId = value;}}
		[MessagePack.Key(2)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(3)]
		public long _hp {get{ return hp;} set{ this.hp = value;}}
		[MessagePack.Key(4)]
		public long _mp {get{ return mp;} set{ this.mp = value;}}
		[MessagePack.Key(5)]
		public long _atk {get{ return atk;} set{ this.atk = value;}}
		[MessagePack.Key(6)]
		public long _def {get{ return def;} set{ this.def = value;}}
		[MessagePack.Key(7)]
		public long _spd {get{ return spd;} set{ this.spd = value;}}
		[MessagePack.Key(8)]
		public long _tec {get{ return tec;} set{ this.tec = value;}}
		[MessagePack.Key(9)]
		public long _param1 {get{ return param1;} set{ this.param1 = value;}}
		[MessagePack.Key(10)]
		public long _param2 {get{ return param2;} set{ this.param2 = value;}}
		[MessagePack.Key(11)]
		public long _param3 {get{ return param3;} set{ this.param3 = value;}}
		[MessagePack.Key(12)]
		public long _param4 {get{ return param4;} set{ this.param4 = value;}}
		[MessagePack.Key(13)]
		public long _param5 {get{ return param5;} set{ this.param5 = value;}}
		[MessagePack.Key(14)]
		public long _conditionMin {get{ return conditionMin;} set{ this.conditionMin = value;}}
		[MessagePack.Key(15)]
		public long _conditionMax {get{ return conditionMax;} set{ this.conditionMax = value;}}
		[MessagePack.Key(16)]
		public long _excessiveConditionMin {get{ return excessiveConditionMin;} set{ this.excessiveConditionMin = value;}}
		[MessagePack.Key(17)]
		public long _excessiveConditionMax {get{ return excessiveConditionMax;} set{ this.excessiveConditionMax = value;}}
		[MessagePack.Key(18)]
		public long _conditionFreeRate {get{ return conditionFreeRate;} set{ this.conditionFreeRate = value;}}
		[MessagePack.Key(19)]
		public long _specialPracticeRate {get{ return specialPracticeRate;} set{ this.specialPracticeRate = value;}}
		[MessagePack.Key(20)]
		public string _conditionEffectGradeUpMapOnType {get{ return conditionEffectGradeUpMapOnType;} set{ this.conditionEffectGradeUpMapOnType = value;}}
		[MessagePack.Key(21)]
		public long _turnAddRate {get{ return turnAddRate;} set{ this.turnAddRate = value;}}
		[MessagePack.Key(22)]
		public long _turnAddMin {get{ return turnAddMin;} set{ this.turnAddMin = value;}}
		[MessagePack.Key(23)]
		public long _turnAddMax {get{ return turnAddMax;} set{ this.turnAddMax = value;}}
		[MessagePack.Key(24)]
		public long _concentrationEnhanceRate {get{ return concentrationEnhanceRate;} set{ this.concentrationEnhanceRate = value;}}
		[MessagePack.Key(25)]
		public long _mTrainingConcentrationLotteryGroup {get{ return mTrainingConcentrationLotteryGroup;} set{ this.mTrainingConcentrationLotteryGroup = value;}}
		[MessagePack.Key(26)]
		public long _inspireRate {get{ return inspireRate;} set{ this.inspireRate = value;}}
		[MessagePack.Key(27)]
		public string _mTrainingCardInspireIdList {get{ return mTrainingCardInspireIdList;} set{ this.mTrainingCardInspireIdList = value;}}
		[MessagePack.Key(28)]
		public string _additionConditionEffectGradeUpRateMap {get{ return additionConditionEffectGradeUpRateMap;} set{ this.additionConditionEffectGradeUpRateMap = value;}}
		[MessagePack.Key(29)]
		public string _boardEventStatusJson {get{ return boardEventStatusJson;} set{ this.boardEventStatusJson = value;}}
		[MessagePack.Key(30)]
		public string _addConcentrationExp {get{ return addConcentrationExp;} set{ this.addConcentrationExp = value;}}
		[MessagePack.Key(31)]
		public string _addConcentrationExpRate {get{ return addConcentrationExpRate;} set{ this.addConcentrationExpRate = value;}}
		[MessagePack.Key(32)]
		public string _addConcentrationTotalExpRate {get{ return addConcentrationTotalExpRate;} set{ this.addConcentrationTotalExpRate = value;}}
		[MessagePack.Key(33)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingCardId = 0; // $mTrainingCardId 練習カードID
		[UnityEngine.SerializeField] long level = 0; // $level 練習カードのレベル
		[UnityEngine.SerializeField] long hp = 0; // $hp hp
		[UnityEngine.SerializeField] long mp = 0; // $mp mp
		[UnityEngine.SerializeField] long atk = 0; // $atk atk
		[UnityEngine.SerializeField] long def = 0; // $def def
		[UnityEngine.SerializeField] long spd = 0; // $spd spd
		[UnityEngine.SerializeField] long tec = 0; // $tec tec
		[UnityEngine.SerializeField] long param1 = 0; // $param1 param1
		[UnityEngine.SerializeField] long param2 = 0; // $param2 param2
		[UnityEngine.SerializeField] long param3 = 0; // $param3 param3
		[UnityEngine.SerializeField] long param4 = 0; // $param4 param4
		[UnityEngine.SerializeField] long param5 = 0; // $param5 param5
		[UnityEngine.SerializeField] long conditionMin = 0; // $conditionMin m_training_condition_tierのconditionType=1の場合に参照。最小コンディション変動（消費はマイナス、回復はプラス）
		[UnityEngine.SerializeField] long conditionMax = 0; // $conditionMax m_training_condition_tierのconditionType=1の場合に参照。最大コンディション変動（消費はマイナス、回復はプラス）
		[UnityEngine.SerializeField] long excessiveConditionMin = 0; // $excessiveConditionMin m_training_condition_tierのconditionType=2の場合に参照。最小コンディション変動（消費はマイナス、回復はプラス）
		[UnityEngine.SerializeField] long excessiveConditionMax = 0; // $excessiveConditionMax m_training_condition_tierのconditionType=2の場合に参照。最大コンディション変動（消費はマイナス、回復はプラス）
		[UnityEngine.SerializeField] long conditionFreeRate = 0; // $conditionFreeRate コンディション変動が0となる確率（万分率）
		[UnityEngine.SerializeField] long specialPracticeRate = 0; // $specialPracticeRate スペシャルレクチャー参加率ベース（万分率）
		[UnityEngine.SerializeField] string conditionEffectGradeUpMapOnType = ""; // $conditionEffectGradeUpMapOnType 効果ボーナス上昇設定。練習実施時に発生するボーナス率に、指定確率でn段階の上昇補正がかかる。[{"practiceType":0,"grade":2,"rate":1000},{"practiceType":0,"grade":1,"rate":2000},{"practiceType":1,"grade":1,"rate":1000}] のように指定する
		[UnityEngine.SerializeField] long turnAddRate = 0; // $turnAddRate ターン追加が発生する確率。追加ターン数はturnAddMin以上turnAddMax以下の値から等確率で抽選される
		[UnityEngine.SerializeField] long turnAddMin = 0; // $turnAddMin ターン追加が発生した際に追加される最低ターン数
		[UnityEngine.SerializeField] long turnAddMax = 0; // $turnAddMax ターン追加が発生した際に追加される最高ターン数
		[UnityEngine.SerializeField] long concentrationEnhanceRate = 0; // $concentrationEnhanceRate mTrainingConcentrationLotteryGroupに紐づくMTrainingConcentrationLotteryのrateに加算
		[UnityEngine.SerializeField] long mTrainingConcentrationLotteryGroup = 0; // $mTrainingConcentrationLotteryGroup カード実行時に発生させるコンセントレーショングループ
		[UnityEngine.SerializeField] long inspireRate = 0; // $inspireRate mTrainingCardInspireIdListに指定しているインスピレーションを獲得できるかの確率
		[UnityEngine.SerializeField] string mTrainingCardInspireIdList = ""; // $mTrainingCardInspireIdList カード実行時に抽選に当選した場合に獲得できるインスピレーションのIDリスト。例：[1,2]
		[UnityEngine.SerializeField] string additionConditionEffectGradeUpRateMap = ""; // $additionConditionEffectGradeUpRateMap xx実施時効果ボーナス上昇。他の効果と重複した場合はrateを加算する。例：[{"grade":2,"rate":1000,"canDuplicate":1}]
		[UnityEngine.SerializeField] string boardEventStatusJson = ""; // $boardEventStatusJson 同時に獲得する臨時練習能力の設定。リストから1つを抽選して獲得する。rateの合計が10000に満たない場合は、10000-rate合計の確率で何も当選しない。activationType => 1: 即時（同時に獲得したステータス等にも効果をつける）、2: 次のtimingから、3: 次のturnから。例：[{"mTrainingBoardEventStatusId":1234, "rate":5000, "activationType":2, "turnCount":3},...]
		[UnityEngine.SerializeField] string addConcentrationExp = ""; // $addConcentrationExp concentrationの経験値獲得アップ（実数）
		[UnityEngine.SerializeField] string addConcentrationExpRate = ""; // $addConcentrationExpRate concentrationの経験値獲得率アップ（万分率）
		[UnityEngine.SerializeField] string addConcentrationTotalExpRate = ""; // $addConcentrationTotalExpRate concentrationの合計経験値率アップ（万分率）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardLevelMasterObjectBase {
		public virtual TrainingCardLevelMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingCardId => _rawData._mTrainingCardId;
		public virtual long level => _rawData._level;
		public virtual long hp => _rawData._hp;
		public virtual long mp => _rawData._mp;
		public virtual long atk => _rawData._atk;
		public virtual long def => _rawData._def;
		public virtual long spd => _rawData._spd;
		public virtual long tec => _rawData._tec;
		public virtual long param1 => _rawData._param1;
		public virtual long param2 => _rawData._param2;
		public virtual long param3 => _rawData._param3;
		public virtual long param4 => _rawData._param4;
		public virtual long param5 => _rawData._param5;
		public virtual long conditionMin => _rawData._conditionMin;
		public virtual long conditionMax => _rawData._conditionMax;
		public virtual long excessiveConditionMin => _rawData._excessiveConditionMin;
		public virtual long excessiveConditionMax => _rawData._excessiveConditionMax;
		public virtual long conditionFreeRate => _rawData._conditionFreeRate;
		public virtual long specialPracticeRate => _rawData._specialPracticeRate;
		public virtual string conditionEffectGradeUpMapOnType => _rawData._conditionEffectGradeUpMapOnType;
		public virtual long turnAddRate => _rawData._turnAddRate;
		public virtual long turnAddMin => _rawData._turnAddMin;
		public virtual long turnAddMax => _rawData._turnAddMax;
		public virtual long concentrationEnhanceRate => _rawData._concentrationEnhanceRate;
		public virtual long mTrainingConcentrationLotteryGroup => _rawData._mTrainingConcentrationLotteryGroup;
		public virtual long inspireRate => _rawData._inspireRate;
		public virtual string mTrainingCardInspireIdList => _rawData._mTrainingCardInspireIdList;
		public virtual string additionConditionEffectGradeUpRateMap => _rawData._additionConditionEffectGradeUpRateMap;
		public virtual string boardEventStatusJson => _rawData._boardEventStatusJson;
		public virtual string addConcentrationExp => _rawData._addConcentrationExp;
		public virtual string addConcentrationExpRate => _rawData._addConcentrationExpRate;
		public virtual string addConcentrationTotalExpRate => _rawData._addConcentrationTotalExpRate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardLevelMasterValueObject _rawData = null;
		public TrainingCardLevelMasterObjectBase( TrainingCardLevelMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardLevelMasterObject : TrainingCardLevelMasterObjectBase, IMasterObject {
		public TrainingCardLevelMasterObject( TrainingCardLevelMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardLevelMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card_Level;

        [UnityEngine.SerializeField]
        TrainingCardLevelMasterValueObject[] m_Training_Card_Level = null;
    }


}
