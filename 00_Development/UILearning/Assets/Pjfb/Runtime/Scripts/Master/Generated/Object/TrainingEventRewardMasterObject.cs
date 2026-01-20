//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingEventRewardMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _hp {get{ return hp;} set{ this.hp = value;}}
		[MessagePack.Key(3)]
		public long _mp {get{ return mp;} set{ this.mp = value;}}
		[MessagePack.Key(4)]
		public long _atk {get{ return atk;} set{ this.atk = value;}}
		[MessagePack.Key(5)]
		public long _def {get{ return def;} set{ this.def = value;}}
		[MessagePack.Key(6)]
		public long _spd {get{ return spd;} set{ this.spd = value;}}
		[MessagePack.Key(7)]
		public long _tec {get{ return tec;} set{ this.tec = value;}}
		[MessagePack.Key(8)]
		public long _param1 {get{ return param1;} set{ this.param1 = value;}}
		[MessagePack.Key(9)]
		public long _param2 {get{ return param2;} set{ this.param2 = value;}}
		[MessagePack.Key(10)]
		public long _param3 {get{ return param3;} set{ this.param3 = value;}}
		[MessagePack.Key(11)]
		public long _param4 {get{ return param4;} set{ this.param4 = value;}}
		[MessagePack.Key(12)]
		public long _param5 {get{ return param5;} set{ this.param5 = value;}}
		[MessagePack.Key(13)]
		public long _conditionMin {get{ return conditionMin;} set{ this.conditionMin = value;}}
		[MessagePack.Key(14)]
		public long _conditionMax {get{ return conditionMax;} set{ this.conditionMax = value;}}
		[MessagePack.Key(15)]
		public long _maxConditionChange {get{ return maxConditionChange;} set{ this.maxConditionChange = value;}}
		[MessagePack.Key(16)]
		public string _getAbilityJson {get{ return getAbilityJson;} set{ this.getAbilityJson = value;}}
		[MessagePack.Key(17)]
		public string _addMCharaIdList {get{ return addMCharaIdList;} set{ this.addMCharaIdList = value;}}
		[MessagePack.Key(18)]
		public string _eventRateCorrectionJson {get{ return eventRateCorrectionJson;} set{ this.eventRateCorrectionJson = value;}}
		[MessagePack.Key(19)]
		public string _practiceExpJson {get{ return practiceExpJson;} set{ this.practiceExpJson = value;}}
		[MessagePack.Key(20)]
		public long _turnAddMin {get{ return turnAddMin;} set{ this.turnAddMin = value;}}
		[MessagePack.Key(21)]
		public long _turnAddMax {get{ return turnAddMax;} set{ this.turnAddMax = value;}}
		[MessagePack.Key(22)]
		public long _displayNumber {get{ return displayNumber;} set{ this.displayNumber = value;}}
		[MessagePack.Key(23)]
		public bool _isLimitCondition {get{ return isLimitCondition;} set{ this.isLimitCondition = value;}}
		[MessagePack.Key(24)]
		public long _mTrainingConcentrationLotteryGroup {get{ return mTrainingConcentrationLotteryGroup;} set{ this.mTrainingConcentrationLotteryGroup = value;}}
		[MessagePack.Key(25)]
		public long _mTrainingCardInspireId {get{ return mTrainingCardInspireId;} set{ this.mTrainingCardInspireId = value;}}
		[MessagePack.Key(26)]
		public long _conditionDistributeType {get{ return conditionDistributeType;} set{ this.conditionDistributeType = value;}}
		[MessagePack.Key(27)]
		public WrapperIntList[] _conditionBoardEventStatusIdMap {get{ return conditionBoardEventStatusIdMap;} set{ this.conditionBoardEventStatusIdMap = value;}}
		[MessagePack.Key(28)]
		public string _boardEventStatusJson {get{ return boardEventStatusJson;} set{ this.boardEventStatusJson = value;}}
		[MessagePack.Key(29)]
		public string _addConcentrationExp {get{ return addConcentrationExp;} set{ this.addConcentrationExp = value;}}
		[MessagePack.Key(30)]
		public string _addConcentrationExpRate {get{ return addConcentrationExpRate;} set{ this.addConcentrationExpRate = value;}}
		[MessagePack.Key(31)]
		public string _addConcentrationTotalExpRate {get{ return addConcentrationTotalExpRate;} set{ this.addConcentrationTotalExpRate = value;}}
		[MessagePack.Key(32)]
		public long[] _typeList {get{ return typeList;} set{ this.typeList = value;}}
		[MessagePack.Key(33)]
		public long[] _valueList {get{ return valueList;} set{ this.valueList = value;}}
		[MessagePack.Key(34)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
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
		[UnityEngine.SerializeField] long conditionMin = 0; // $conditionMin コンディション変動最小値（消費はマイナス、回復はプラス）
		[UnityEngine.SerializeField] long conditionMax = 0; // $conditionMax コンディション変動最大値（消費はマイナス、回復はプラス）
		[UnityEngine.SerializeField] long maxConditionChange = 0; // $maxConditionChange 最大コンディションの増減
		[UnityEngine.SerializeField] string getAbilityJson = ""; // $getAbilityJsonこのイベントで得られるスキル情報を羅列したJSON。特訓報酬としてこの報酬を獲得する際は、特訓発生キャラのレベルがcoachCharaLevelMin以上のときにスキルを獲得する（指定しなければ0を指定した扱い）。<br>例：[{"id":1, "level":2}, {"id":2, "level":1, "coachCharaLevelMin":50}]
		[UnityEngine.SerializeField] string addMCharaIdList = ""; // $addMCharaIdList このイベントから今回の個人トレーニングにサポートキャラとして追加される mChara の idList
		[UnityEngine.SerializeField] string eventRateCorrectionJson = ""; // $eventRateCorrectionJson 指定したターンの間、イベントの発生率に補正をかける。[[イベントID1, 発生率加算分1, 有効ターン数1], [イベントID2, 発生率加算分2, 有効ターン数2], ...]の形で指定する
		[UnityEngine.SerializeField] string practiceExpJson = ""; // $practiceExpJson 指定した練習種別に対し経験値やレベルを加算する。[{"practiceType":0,"exp":100,"level":1},...] のように指定すると、練習種別0の練習に対し経験値100を与えたあとレベルを1加算する。（※同オブジェクト内では順番によらず経験値が先に加算される）
		[UnityEngine.SerializeField] long turnAddMin = 0; // $turnAddMin ターン延長min値
		[UnityEngine.SerializeField] long turnAddMax = 0; // $turnAddMax ターン延長max値
		[UnityEngine.SerializeField] long displayNumber = 0; // $displayNumber練習能力の firstMTrainingEventRewardId で本報酬が指定された際、その練習能力の効果量の表示用に使用する数値。サーバ側では使用せず、クライアントでの表示等に使用する<br>一つの使用例として、eventRateCorrectionJson で複数のイベントの発生率を共通の値で調整した場合に、その値を displayNumber に入れておくことでクライアント側で容易に報酬の数値を取得できる
		[UnityEngine.SerializeField] bool isLimitCondition = false; // ターン延長max値config.standardMaxConditionを超えるコンディションの時に報酬で獲得するコンディション値変動値に制限を設けるか。1 => 制限を設ける, 2 => 制限を設けない
		[UnityEngine.SerializeField] long mTrainingConcentrationLotteryGroup = 0; // コンセントレーションゾーンのグループID。サポートイベントを再生した際に誘発するコンセントレーションを指定する
		[UnityEngine.SerializeField] long mTrainingCardInspireId = 0; // インスピレーションID。サポートイベントを再生した際に誘発するインスピレーションを指定する
		[UnityEngine.SerializeField] long conditionDistributeType = 0; // $conditionDistributeType コンディション付与方法設定。1 => condition の値だけ変動、2 => condition の値に変更（上昇する場合のみ）、3 => condition の値に変更（上昇・減少問わず）
		[UnityEngine.SerializeField] WrapperIntList[] conditionBoardEventStatusIdMap = null; // $conditionBoardEventStatusIdMap 報酬付与前のコンディション値が特定の閾値以上であった場合に追加で臨時練習能力効果を与える設定。[[101,1],[131,2]] のように設定すると、101以上130未満だった場合にID:1の臨時練習能力を、131以上だった場合にID:2の臨時練習能力を付与する
		[UnityEngine.SerializeField] string boardEventStatusJson = ""; // $boardEventStatusJson 同時に獲得する臨時練習能力の設定。リストから1つを抽選して獲得する。rateの合計が10000に満たない場合は、10000-rate合計の確率で何も当選しない。activationType => 1: 即時（同時に獲得したステータス等にも効果をつける）、2: 次のtimingから、3: 次のturnから。例：[{"mTrainingBoardEventStatusId":1234, "rate":5000, "activationType":2, "turnCount":3},...]
		[UnityEngine.SerializeField] string addConcentrationExp = ""; // $addConcentrationExp concentrationの経験値獲得アップ（実数）
		[UnityEngine.SerializeField] string addConcentrationExpRate = ""; // $addConcentrationExpRate concentrationの経験値獲得率アップ（万分率）
		[UnityEngine.SerializeField] string addConcentrationTotalExpRate = ""; // $addConcentrationTotalExpRate concentrationの合計経験値率アップ（万分率）
		[UnityEngine.SerializeField] long[] typeList = null; // このレコードが持つイベント報酬の効果（m_training_event_reward_type_detail.type）を詰めたjson配列。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		[UnityEngine.SerializeField] long[] valueList = null; // このレコードが持つイベント報酬の効果量を詰めたjson配列。値の順番はtypeListに対応する。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingEventRewardMasterObjectBase {
		public virtual TrainingEventRewardMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
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
		public virtual long maxConditionChange => _rawData._maxConditionChange;
		public virtual string getAbilityJson => _rawData._getAbilityJson;
		public virtual string addMCharaIdList => _rawData._addMCharaIdList;
		public virtual string eventRateCorrectionJson => _rawData._eventRateCorrectionJson;
		public virtual string practiceExpJson => _rawData._practiceExpJson;
		public virtual long turnAddMin => _rawData._turnAddMin;
		public virtual long turnAddMax => _rawData._turnAddMax;
		public virtual long displayNumber => _rawData._displayNumber;
		public virtual bool isLimitCondition => _rawData._isLimitCondition;
		public virtual long mTrainingConcentrationLotteryGroup => _rawData._mTrainingConcentrationLotteryGroup;
		public virtual long mTrainingCardInspireId => _rawData._mTrainingCardInspireId;
		public virtual long conditionDistributeType => _rawData._conditionDistributeType;
		public virtual WrapperIntList[] conditionBoardEventStatusIdMap => _rawData._conditionBoardEventStatusIdMap;
		public virtual string boardEventStatusJson => _rawData._boardEventStatusJson;
		public virtual string addConcentrationExp => _rawData._addConcentrationExp;
		public virtual string addConcentrationExpRate => _rawData._addConcentrationExpRate;
		public virtual string addConcentrationTotalExpRate => _rawData._addConcentrationTotalExpRate;
		public virtual long[] typeList => _rawData._typeList;
		public virtual long[] valueList => _rawData._valueList;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingEventRewardMasterValueObject _rawData = null;
		public TrainingEventRewardMasterObjectBase( TrainingEventRewardMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingEventRewardMasterObject : TrainingEventRewardMasterObjectBase, IMasterObject {
		public TrainingEventRewardMasterObject( TrainingEventRewardMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingEventRewardMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Event_Reward;

        [UnityEngine.SerializeField]
        TrainingEventRewardMasterValueObject[] m_Training_Event_Reward = null;
    }


}
