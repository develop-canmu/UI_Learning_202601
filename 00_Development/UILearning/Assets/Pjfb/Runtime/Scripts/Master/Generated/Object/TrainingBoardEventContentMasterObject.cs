//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingBoardEventContentMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingBoardEventContentGroupId {get{ return mTrainingBoardEventContentGroupId;} set{ this.mTrainingBoardEventContentGroupId = value;}}
		[MessagePack.Key(2)]
		public long _mTrainingUnitId {get{ return mTrainingUnitId;} set{ this.mTrainingUnitId = value;}}
		[MessagePack.Key(3)]
		public long _mTrainingBoardEventStatusId {get{ return mTrainingBoardEventStatusId;} set{ this.mTrainingBoardEventStatusId = value;}}
		[MessagePack.Key(4)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingBoardEventContentGroupId = 0; // $mTrainingBoardEventContentGroupId 抽選時に使用するグループ
		[UnityEngine.SerializeField] long mTrainingUnitId = 0; // $mTrainingUnitId トレーニングユニットID。特定キャラの組み合わせにより発生させる効果として取り扱う場合はトレーニングユニットIDを指定する。無条件で発生させる場合は0を設定する
		[UnityEngine.SerializeField] long mTrainingBoardEventStatusId = 0; // $mTrainingBoardEventStatusId 獲得する練習能力効果実体ID
		[UnityEngine.SerializeField] long rate = 0; // $rate 抽選重み
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingBoardEventContentMasterObjectBase {
		public virtual TrainingBoardEventContentMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingBoardEventContentGroupId => _rawData._mTrainingBoardEventContentGroupId;
		public virtual long mTrainingUnitId => _rawData._mTrainingUnitId;
		public virtual long mTrainingBoardEventStatusId => _rawData._mTrainingBoardEventStatusId;
		public virtual long rate => _rawData._rate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingBoardEventContentMasterValueObject _rawData = null;
		public TrainingBoardEventContentMasterObjectBase( TrainingBoardEventContentMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingBoardEventContentMasterObject : TrainingBoardEventContentMasterObjectBase, IMasterObject {
		public TrainingBoardEventContentMasterObject( TrainingBoardEventContentMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingBoardEventContentMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Board_Event_Content;

        [UnityEngine.SerializeField]
        TrainingBoardEventContentMasterValueObject[] m_Training_Board_Event_Content = null;
    }


}
