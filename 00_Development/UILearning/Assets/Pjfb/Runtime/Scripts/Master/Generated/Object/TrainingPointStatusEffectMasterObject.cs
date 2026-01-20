//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingPointStatusEffectMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingPointStatusEffectGroup {get{ return mTrainingPointStatusEffectGroup;} set{ this.mTrainingPointStatusEffectGroup = value;}}
		[MessagePack.Key(2)]
		public long _additionEffectGroup {get{ return additionEffectGroup;} set{ this.additionEffectGroup = value;}}
		[MessagePack.Key(3)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(4)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(5)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(6)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(7)]
		public long[] _typeList {get{ return typeList;} set{ this.typeList = value;}}
		[MessagePack.Key(8)]
		public long[] _valueList {get{ return valueList;} set{ this.valueList = value;}}
		[MessagePack.Key(9)]
		public long _effectRate {get{ return effectRate;} set{ this.effectRate = value;}}
		[MessagePack.Key(10)]
		public string _practiceParamRateMap {get{ return practiceParamRateMap;} set{ this.practiceParamRateMap = value;}}
		[MessagePack.Key(11)]
		public string _practiceParamAddMap {get{ return practiceParamAddMap;} set{ this.practiceParamAddMap = value;}}
		[MessagePack.Key(12)]
		public string _getAbilityCountMap {get{ return getAbilityCountMap;} set{ this.getAbilityCountMap = value;}}
		[MessagePack.Key(13)]
		public string _displayTypeRateJson {get{ return displayTypeRateJson;} set{ this.displayTypeRateJson = value;}}
		[MessagePack.Key(14)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(15)]
		public bool _displayFlg {get{ return displayFlg;} set{ this.displayFlg = value;}}
		[MessagePack.Key(16)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingPointStatusEffectGroup = 0; // $mTrainingPointStatusEffectGroup グループID
		[UnityEngine.SerializeField] long additionEffectGroup = 0; // $additionEffectGroup 追加効果グループID。同じターンに連続してブースト効果の抽選が発生した場合、同じ追加効果グループIDの効果を発生させないようにする
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] string description = ""; // $description 説明
		[UnityEngine.SerializeField] long imageId = 0; // $imageId 画像ID
		[UnityEngine.SerializeField] long type = 0; // $type 種別。1=>常時発動、2=>ブーストが発生したターンのみ発動
		[UnityEngine.SerializeField] long[] typeList = null; // このレコードが持つ練習能力タイプ（m_training_status_type_detail.type）を詰めたjson配列。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		[UnityEngine.SerializeField] long[] valueList = null; // このレコードが持つ練習能力の効果量を詰めたjson配列。値の順番はtypeListに対応する。主にクライアント側で各能力を持つレコードを検索する際に使用し、現状トレーニング内の処理では参照せず、性能に影響しない。
		[UnityEngine.SerializeField] long effectRate = 0; // $effectRate 練習効果倍率。万分率で指定し、等倍なら10000とする
		[UnityEngine.SerializeField] string practiceParamRateMap = ""; // $practiceParamRateMap 練習実行時ステータスアップ（倍率）。全てのステータスに適用の場合はallで指定。例：{"all":1000}
		[UnityEngine.SerializeField] string practiceParamAddMap = ""; // $practiceParamAddMap 練習実行時ステータスアップ（実数）。全てのステータスに適用の場合はallで指定。例：{"spd":1,"tec":1,"param1":1,"param2":1,"param4":1,"param5":1}
		[UnityEngine.SerializeField] string getAbilityCountMap = ""; // $getAbilityCountMap 練習実行時の獲得スキル数。例：[{"count":1, "rate":10000}]
		[UnityEngine.SerializeField] string displayTypeRateJson = ""; // $displayTypeRateJson 表示種別抽選リスト。例：[[A,B]]。A=>カラー種別、B=>倍率（万分率）
		[UnityEngine.SerializeField] long displayPriority = 0; // $displayPriority 表示優先度
		[UnityEngine.SerializeField] bool displayFlg = false; // $displayFlg 1=>表示する、2=>表示しない
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingPointStatusEffectMasterObjectBase {
		public virtual TrainingPointStatusEffectMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingPointStatusEffectGroup => _rawData._mTrainingPointStatusEffectGroup;
		public virtual long additionEffectGroup => _rawData._additionEffectGroup;
		public virtual string name => _rawData._name;
		public virtual string description => _rawData._description;
		public virtual long imageId => _rawData._imageId;
		public virtual long type => _rawData._type;
		public virtual long[] typeList => _rawData._typeList;
		public virtual long[] valueList => _rawData._valueList;
		public virtual long effectRate => _rawData._effectRate;
		public virtual string practiceParamRateMap => _rawData._practiceParamRateMap;
		public virtual string practiceParamAddMap => _rawData._practiceParamAddMap;
		public virtual string getAbilityCountMap => _rawData._getAbilityCountMap;
		public virtual string displayTypeRateJson => _rawData._displayTypeRateJson;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual bool displayFlg => _rawData._displayFlg;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingPointStatusEffectMasterValueObject _rawData = null;
		public TrainingPointStatusEffectMasterObjectBase( TrainingPointStatusEffectMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingPointStatusEffectMasterObject : TrainingPointStatusEffectMasterObjectBase, IMasterObject {
		public TrainingPointStatusEffectMasterObject( TrainingPointStatusEffectMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingPointStatusEffectMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Point_Status_Effect;

        [UnityEngine.SerializeField]
        TrainingPointStatusEffectMasterValueObject[] m_Training_Point_Status_Effect = null;
    }


}
