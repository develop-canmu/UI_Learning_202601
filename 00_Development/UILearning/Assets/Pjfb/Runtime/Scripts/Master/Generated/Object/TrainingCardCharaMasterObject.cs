//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardCharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(2)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(3)]
		public long _newLiberationLevel {get{ return newLiberationLevel;} set{ this.newLiberationLevel = value;}}
		[MessagePack.Key(4)]
		public long _mTrainingCardId {get{ return mTrainingCardId;} set{ this.mTrainingCardId = value;}}
		[MessagePack.Key(5)]
		public long _groupId {get{ return groupId;} set{ this.groupId = value;}}
		[MessagePack.Key(6)]
		public long _displayEnhanceLevel {get{ return displayEnhanceLevel;} set{ this.displayEnhanceLevel = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId カードを取得する mChara のID
		[UnityEngine.SerializeField] long level = 0; // $level mChara がカードを取得するレベル。0なら初期から持っている
		[UnityEngine.SerializeField] long newLiberationLevel = 0; // $newLiberationLevel mChara がカードを取得する潜在解放レベル。0なら初期から持っている
		[UnityEngine.SerializeField] long mTrainingCardId = 0; // $mTrainingCardId 取得する練習カードのID
		[UnityEngine.SerializeField] long groupId = 0; // $groupId グループID。mCharaId-groupIdが同一の場合、キャラの強化levelを満たす一番高いlevelのレコードのみ取得される
		[UnityEngine.SerializeField] long displayEnhanceLevel = 0; // $displayEnhanceLevel クライアント表示用の練習カードの強化レベル
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardCharaMasterObjectBase {
		public virtual TrainingCardCharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long level => _rawData._level;
		public virtual long newLiberationLevel => _rawData._newLiberationLevel;
		public virtual long mTrainingCardId => _rawData._mTrainingCardId;
		public virtual long groupId => _rawData._groupId;
		public virtual long displayEnhanceLevel => _rawData._displayEnhanceLevel;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardCharaMasterValueObject _rawData = null;
		public TrainingCardCharaMasterObjectBase( TrainingCardCharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardCharaMasterObject : TrainingCardCharaMasterObjectBase, IMasterObject {
		public TrainingCardCharaMasterObject( TrainingCardCharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardCharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card_Chara;

        [UnityEngine.SerializeField]
        TrainingCardCharaMasterValueObject[] m_Training_Card_Chara = null;
    }


}
