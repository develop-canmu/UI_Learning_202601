//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingStatusTypeDetailMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingStatusTypeDetailCategoryId {get{ return mTrainingStatusTypeDetailCategoryId;} set{ this.mTrainingStatusTypeDetailCategoryId = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(4)]
		public long _valueType {get{ return valueType;} set{ this.valueType = value;}}
		[MessagePack.Key(5)]
		public long _iconId {get{ return iconId;} set{ this.iconId = value;}}
		[MessagePack.Key(6)]
		public long _buffIconId {get{ return buffIconId;} set{ this.buffIconId = value;}}
		[MessagePack.Key(7)]
		public long _boardEventEffectViewType {get{ return boardEventEffectViewType;} set{ this.boardEventEffectViewType = value;}}
		[MessagePack.Key(8)]
		public long _paramType {get{ return paramType;} set{ this.paramType = value;}}
		[MessagePack.Key(9)]
		public bool _isUsedAsTrainerStatus {get{ return isUsedAsTrainerStatus;} set{ this.isUsedAsTrainerStatus = value;}}
		[MessagePack.Key(10)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(11)]
		public bool _isDisplayAggregatedEffect {get{ return isDisplayAggregatedEffect;} set{ this.isDisplayAggregatedEffect = value;}}
		[MessagePack.Key(12)]
		public string _aggregatedEffectName {get{ return aggregatedEffectName;} set{ this.aggregatedEffectName = value;}}
		[MessagePack.Key(13)]
		public long _displayPriority {get{ return displayPriority;} set{ this.displayPriority = value;}}
		[MessagePack.Key(14)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingStatusTypeDetailCategoryId = 0; // $mTrainingStatusTypeDetailCategoryId カテゴリID
		[UnityEngine.SerializeField] long type = 0; // $type 練習能力タイプ。クライアント側で検索に用いる番号
		[UnityEngine.SerializeField] string name = ""; // $name 練習能力名
		[UnityEngine.SerializeField] long valueType = 0; // $valueType 練習能力の効果値タイプ。実数値か倍率かなどをクライアント側で判断するために使用する
		[UnityEngine.SerializeField] long iconId = 0; // $iconId 練習能力アイコンの画像ID
		[UnityEngine.SerializeField] long buffIconId = 0; // $buffIconId バフアイコンの画像ID
		[UnityEngine.SerializeField] long boardEventEffectViewType = 0; // $boardEventEffectViewType マスイベント上での表示種別
		[UnityEngine.SerializeField] long paramType = 0; // $paramType マスイベント上での表示種別に応じたパラメータ種別
		[UnityEngine.SerializeField] bool isUsedAsTrainerStatus = false; // $isUsedAsTrainerStatus トレーニング補助キャラの練習能力として使用するか。クライアント側で判断するために使用する
		[UnityEngine.SerializeField] string description = ""; // $description 練習能力の説明文
		[UnityEngine.SerializeField] bool isDisplayAggregatedEffect = false; // $isDisplayAggregatedEffect 合計効果表示とするか
		[UnityEngine.SerializeField] string aggregatedEffectName = ""; // $aggregatedEffectName 合計効果表示の場合の表示名称
		[UnityEngine.SerializeField] long displayPriority = 0; // $displayPriority 表示優先度
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingStatusTypeDetailMasterObjectBase {
		public virtual TrainingStatusTypeDetailMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingStatusTypeDetailCategoryId => _rawData._mTrainingStatusTypeDetailCategoryId;
		public virtual long type => _rawData._type;
		public virtual string name => _rawData._name;
		public virtual long valueType => _rawData._valueType;
		public virtual long iconId => _rawData._iconId;
		public virtual long buffIconId => _rawData._buffIconId;
		public virtual long boardEventEffectViewType => _rawData._boardEventEffectViewType;
		public virtual long paramType => _rawData._paramType;
		public virtual bool isUsedAsTrainerStatus => _rawData._isUsedAsTrainerStatus;
		public virtual string description => _rawData._description;
		public virtual bool isDisplayAggregatedEffect => _rawData._isDisplayAggregatedEffect;
		public virtual string aggregatedEffectName => _rawData._aggregatedEffectName;
		public virtual long displayPriority => _rawData._displayPriority;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingStatusTypeDetailMasterValueObject _rawData = null;
		public TrainingStatusTypeDetailMasterObjectBase( TrainingStatusTypeDetailMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingStatusTypeDetailMasterObject : TrainingStatusTypeDetailMasterObjectBase, IMasterObject {
		public TrainingStatusTypeDetailMasterObject( TrainingStatusTypeDetailMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingStatusTypeDetailMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Status_Type_Detail;

        [UnityEngine.SerializeField]
        TrainingStatusTypeDetailMasterValueObject[] m_Training_Status_Type_Detail = null;
    }


}
