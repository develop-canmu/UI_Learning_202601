//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingBoardEventCharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(2)]
		public long _joinType {get{ return joinType;} set{ this.joinType = value;}}
		[MessagePack.Key(3)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(4)]
		public long _mTrainingBoardEventId {get{ return mTrainingBoardEventId;} set{ this.mTrainingBoardEventId = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId キャラID
		[UnityEngine.SerializeField] long joinType = 0; // $joinType 参加タイプ。1 => トレーニング対象キャラとしての参加時、2 => トレーニングサポートキャラとしての参加時
		[UnityEngine.SerializeField] long level = 0; // $level キャラがマスイベントを取得するレベル。0なら初期から持っている
		[UnityEngine.SerializeField] long mTrainingBoardEventId = 0; // $mTrainingBoardEventId 取得するマスイベントのID
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingBoardEventCharaMasterObjectBase {
		public virtual TrainingBoardEventCharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long joinType => _rawData._joinType;
		public virtual long level => _rawData._level;
		public virtual long mTrainingBoardEventId => _rawData._mTrainingBoardEventId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingBoardEventCharaMasterValueObject _rawData = null;
		public TrainingBoardEventCharaMasterObjectBase( TrainingBoardEventCharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingBoardEventCharaMasterObject : TrainingBoardEventCharaMasterObjectBase, IMasterObject {
		public TrainingBoardEventCharaMasterObject( TrainingBoardEventCharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingBoardEventCharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Board_Event_Chara;

        [UnityEngine.SerializeField]
        TrainingBoardEventCharaMasterValueObject[] m_Training_Board_Event_Chara = null;
    }


}
