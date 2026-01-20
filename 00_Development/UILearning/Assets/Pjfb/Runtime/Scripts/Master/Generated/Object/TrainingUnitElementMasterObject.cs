//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingUnitElementMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingUnitId {get{ return mTrainingUnitId;} set{ this.mTrainingUnitId = value;}}
		[MessagePack.Key(2)]
		public long _joinType {get{ return joinType;} set{ this.joinType = value;}}
		[MessagePack.Key(3)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(4)]
		public long _masterId {get{ return masterId;} set{ this.masterId = value;}}
		[MessagePack.Key(5)]
		public long _minLevel {get{ return minLevel;} set{ this.minLevel = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingUnitId = 0; // $mTrainingUnitId トレーニングユニットID
		[UnityEngine.SerializeField] long joinType = 0; // $joinType 参加形態指定。0: 指定なし、1:育成対象、2: サポート
		[UnityEngine.SerializeField] long type = 0; // $type 親キャラID指定かキャラID指定か。1: parentId、2: mCharaId
		[UnityEngine.SerializeField] long masterId = 0; // $masterId 親キャラIDかキャラID。typeに合わせて指定する
		[UnityEngine.SerializeField] long minLevel = 0; // $minLevel 設定されているmasterIdに対してのレベル条件を設定する。type=2の場合のみ適用される
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingUnitElementMasterObjectBase {
		public virtual TrainingUnitElementMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingUnitId => _rawData._mTrainingUnitId;
		public virtual long joinType => _rawData._joinType;
		public virtual long type => _rawData._type;
		public virtual long masterId => _rawData._masterId;
		public virtual long minLevel => _rawData._minLevel;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingUnitElementMasterValueObject _rawData = null;
		public TrainingUnitElementMasterObjectBase( TrainingUnitElementMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingUnitElementMasterObject : TrainingUnitElementMasterObjectBase, IMasterObject {
		public TrainingUnitElementMasterObject( TrainingUnitElementMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingUnitElementMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Unit_Element;

        [UnityEngine.SerializeField]
        TrainingUnitElementMasterValueObject[] m_Training_Unit_Element = null;
    }


}
