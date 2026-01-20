//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingPointStatusLevelMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingPointStatusLevelGroup {get{ return mTrainingPointStatusLevelGroup;} set{ this.mTrainingPointStatusLevelGroup = value;}}
		[MessagePack.Key(2)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(3)]
		public long _costValue {get{ return costValue;} set{ this.costValue = value;}}
		[MessagePack.Key(4)]
		public long _mTrainingPointStatusEffectGroup {get{ return mTrainingPointStatusEffectGroup;} set{ this.mTrainingPointStatusEffectGroup = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingPointStatusLevelGroup = 0; // $mTrainingPointStatusLevelGroup グループID
		[UnityEngine.SerializeField] long level = 0; // $level レベル
		[UnityEngine.SerializeField] long costValue = 0; // $costValue 消費量
		[UnityEngine.SerializeField] long mTrainingPointStatusEffectGroup = 0; // $mTrainingPointStatusEffectGroup 効果グループID
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingPointStatusLevelMasterObjectBase {
		public virtual TrainingPointStatusLevelMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingPointStatusLevelGroup => _rawData._mTrainingPointStatusLevelGroup;
		public virtual long level => _rawData._level;
		public virtual long costValue => _rawData._costValue;
		public virtual long mTrainingPointStatusEffectGroup => _rawData._mTrainingPointStatusEffectGroup;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingPointStatusLevelMasterValueObject _rawData = null;
		public TrainingPointStatusLevelMasterObjectBase( TrainingPointStatusLevelMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingPointStatusLevelMasterObject : TrainingPointStatusLevelMasterObjectBase, IMasterObject {
		public TrainingPointStatusLevelMasterObject( TrainingPointStatusLevelMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingPointStatusLevelMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Point_Status_Level;

        [UnityEngine.SerializeField]
        TrainingPointStatusLevelMasterValueObject[] m_Training_Point_Status_Level = null;
    }


}
