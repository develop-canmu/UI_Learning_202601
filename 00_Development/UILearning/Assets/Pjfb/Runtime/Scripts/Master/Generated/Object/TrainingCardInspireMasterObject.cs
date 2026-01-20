//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardInspireMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(3)]
		public long _grade {get{ return grade;} set{ this.grade = value;}}
		[MessagePack.Key(4)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(5)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(6)]
		public long _effectNumber {get{ return effectNumber;} set{ this.effectNumber = value;}}
		[MessagePack.Key(7)]
		public string _practiceParamAddBonusMap {get{ return practiceParamAddBonusMap;} set{ this.practiceParamAddBonusMap = value;}}
		[MessagePack.Key(8)]
		public string _practiceParamAddMap {get{ return practiceParamAddMap;} set{ this.practiceParamAddMap = value;}}
		[MessagePack.Key(9)]
		public long _conditionRecoverUp {get{ return conditionRecoverUp;} set{ this.conditionRecoverUp = value;}}
		[MessagePack.Key(10)]
		public string _getAbilityRateMap {get{ return getAbilityRateMap;} set{ this.getAbilityRateMap = value;}}
		[MessagePack.Key(11)]
		public long _occursMTrainingEventId {get{ return occursMTrainingEventId;} set{ this.occursMTrainingEventId = value;}}
		[MessagePack.Key(12)]
		public long _rarePracticeEnhanceRate {get{ return rarePracticeEnhanceRate;} set{ this.rarePracticeEnhanceRate = value;}}
		[MessagePack.Key(13)]
		public string _invokeCondition {get{ return invokeCondition;} set{ this.invokeCondition = value;}}
		[MessagePack.Key(14)]
		public long _exp {get{ return exp;} set{ this.exp = value;}}
		[MessagePack.Key(15)]
		public bool _canDuplicate {get{ return canDuplicate;} set{ this.canDuplicate = value;}}
		[MessagePack.Key(16)]
		public long _mTrainingCardInspireGradeLotteryGroup {get{ return mTrainingCardInspireGradeLotteryGroup;} set{ this.mTrainingCardInspireGradeLotteryGroup = value;}}
		[MessagePack.Key(17)]
		public string _targetMCharaIdList {get{ return targetMCharaIdList;} set{ this.targetMCharaIdList = value;}}
		[MessagePack.Key(18)]
		public long[] _typeList {get{ return typeList;} set{ this.typeList = value;}}
		[MessagePack.Key(19)]
		public long[] _valueList {get{ return valueList;} set{ this.valueList = value;}}
		[MessagePack.Key(20)]
		public long _isConditionExpend {get{ return isConditionExpend;} set{ this.isConditionExpend = value;}}
		[MessagePack.Key(21)]
		public string _addConcentrationExp {get{ return addConcentrationExp;} set{ this.addConcentrationExp = value;}}
		[MessagePack.Key(22)]
		public string _addConcentrationExpRate {get{ return addConcentrationExpRate;} set{ this.addConcentrationExpRate = value;}}
		[MessagePack.Key(23)]
		public string _addConcentrationTotalExpRate {get{ return addConcentrationTotalExpRate;} set{ this.addConcentrationTotalExpRate = value;}}
		[MessagePack.Key(24)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] string description = ""; // $description 説明
		[UnityEngine.SerializeField] long grade = 0; // $grade グレード
		[UnityEngine.SerializeField] long priority = 0; // $priority 優先度。降順
		[UnityEngine.SerializeField] long imageId = 0; // $imageId アイコンID
		[UnityEngine.SerializeField] long effectNumber = 0; // $effectNumber 演出番号
		[UnityEngine.SerializeField] string practiceParamAddBonusMap = ""; // $practiceParamAddBonusMap 練習時固定ボーナス（実数値）。インスピレーションが付与されたカードが選択された場合、トレーニング実施時に特定ステータスを追加で獲得する
		[UnityEngine.SerializeField] string practiceParamAddMap = ""; // $practiceParamAddMap 練習時ボーナス（万分率）。インスピレーションが付与されたカードが選択された場合、獲得する特定のステータスが割合でアップする
		[UnityEngine.SerializeField] long conditionRecoverUp = 0; // $conditionRecoverUp コンディション回復アップ（実数値）。インスピレーションが付与されたカードが選択された場合、コンディションが回復する練習だと回復量に加算される
		[UnityEngine.SerializeField] string getAbilityRateMap = ""; // $getAbilityRateMap インスピレーションが付与されたカードが選択された場合、アビリティを抽選して獲得する。例: [{id:1,level:1,rate:1500}]
		[UnityEngine.SerializeField] long occursMTrainingEventId = 0; // $occursMTrainingEventId 付与されたSPトレーニングカードを練習メニューとして実行時に発生させるサポートイベント
		[UnityEngine.SerializeField] long rarePracticeEnhanceRate = 0; // $rarePracticeEnhanceRate レア練習率アップ（万分率）。1+rate/10000 がレア練習カードの重みに乗算される
		[UnityEngine.SerializeField] string invokeCondition = ""; // $invokeCondition 発動条件のmCharaIdのリストで複数のキャラ、サポートカードを指定することが可能。デッキ編成されているmCharaIdのOR条件
		[UnityEngine.SerializeField] long exp = 0; // $exp インスピレーション獲得時に累積する経験値
		[UnityEngine.SerializeField] bool canDuplicate = false; // $canDuplicate 重複付与可能か。1:可能、2:不可
		[UnityEngine.SerializeField] long mTrainingCardInspireGradeLotteryGroup = 0; // $mTrainingCardInspireGradeLotteryGroup m_training_card_inspire_grade_lotteryのgroup
		[UnityEngine.SerializeField] string targetMCharaIdList = ""; // $targetMCharaIdList 付与先のSPトレーニングカード特定カード指定。mCharaIdのリスト
		[UnityEngine.SerializeField] long[] typeList = null; // $typeList このレコードが持つ練習能力タイプ（m_training_status_type_detail.type）を詰めたjson配列
		[UnityEngine.SerializeField] long[] valueList = null; // $valueList このレコードが持つ練習能力の効果量を詰めたjson配列。値の順番はtypeListに対応する
		[UnityEngine.SerializeField] long isConditionExpend = 0; // $isConditionExpend このインスピレーションが実行された場合、コンディション消費をするか。1=>する、2=>しない
		[UnityEngine.SerializeField] string addConcentrationExp = ""; // $addConcentrationExp concentrationの経験値獲得アップ（実数）
		[UnityEngine.SerializeField] string addConcentrationExpRate = ""; // $addConcentrationExpRate concentrationの経験値獲得率アップ（万分率）
		[UnityEngine.SerializeField] string addConcentrationTotalExpRate = ""; // $addConcentrationTotalExpRate concentrationの合計経験値率アップ（万分率）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardInspireMasterObjectBase {
		public virtual TrainingCardInspireMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string description => _rawData._description;
		public virtual long grade => _rawData._grade;
		public virtual long priority => _rawData._priority;
		public virtual long imageId => _rawData._imageId;
		public virtual long effectNumber => _rawData._effectNumber;
		public virtual string practiceParamAddBonusMap => _rawData._practiceParamAddBonusMap;
		public virtual string practiceParamAddMap => _rawData._practiceParamAddMap;
		public virtual long conditionRecoverUp => _rawData._conditionRecoverUp;
		public virtual string getAbilityRateMap => _rawData._getAbilityRateMap;
		public virtual long occursMTrainingEventId => _rawData._occursMTrainingEventId;
		public virtual long rarePracticeEnhanceRate => _rawData._rarePracticeEnhanceRate;
		public virtual string invokeCondition => _rawData._invokeCondition;
		public virtual long exp => _rawData._exp;
		public virtual bool canDuplicate => _rawData._canDuplicate;
		public virtual long mTrainingCardInspireGradeLotteryGroup => _rawData._mTrainingCardInspireGradeLotteryGroup;
		public virtual string targetMCharaIdList => _rawData._targetMCharaIdList;
		public virtual long[] typeList => _rawData._typeList;
		public virtual long[] valueList => _rawData._valueList;
		public virtual long isConditionExpend => _rawData._isConditionExpend;
		public virtual string addConcentrationExp => _rawData._addConcentrationExp;
		public virtual string addConcentrationExpRate => _rawData._addConcentrationExpRate;
		public virtual string addConcentrationTotalExpRate => _rawData._addConcentrationTotalExpRate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardInspireMasterValueObject _rawData = null;
		public TrainingCardInspireMasterObjectBase( TrainingCardInspireMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardInspireMasterObject : TrainingCardInspireMasterObjectBase, IMasterObject {
		public TrainingCardInspireMasterObject( TrainingCardInspireMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardInspireMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card_Inspire;

        [UnityEngine.SerializeField]
        TrainingCardInspireMasterValueObject[] m_Training_Card_Inspire = null;
    }


}
