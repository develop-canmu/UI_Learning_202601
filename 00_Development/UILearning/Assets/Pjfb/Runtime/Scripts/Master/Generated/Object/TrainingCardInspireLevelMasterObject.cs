//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardInspireLevelMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(2)]
		public long _exp {get{ return exp;} set{ this.exp = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long level = 0; // $level レベル
		[UnityEngine.SerializeField] long exp = 0; // $exp 累計経験値
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardInspireLevelMasterObjectBase {
		public virtual TrainingCardInspireLevelMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long level => _rawData._level;
		public virtual long exp => _rawData._exp;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardInspireLevelMasterValueObject _rawData = null;
		public TrainingCardInspireLevelMasterObjectBase( TrainingCardInspireLevelMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardInspireLevelMasterObject : TrainingCardInspireLevelMasterObjectBase, IMasterObject {
		public TrainingCardInspireLevelMasterObject( TrainingCardInspireLevelMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardInspireLevelMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card_Inspire_Level;

        [UnityEngine.SerializeField]
        TrainingCardInspireLevelMasterValueObject[] m_Training_Card_Inspire_Level = null;
    }


}
