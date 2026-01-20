//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingEventRewardTypeDetailMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingEventRewardTypeGroup {get{ return mTrainingEventRewardTypeGroup;} set{ this.mTrainingEventRewardTypeGroup = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingEventRewardTypeGroup = 0; // $mTrainingEventRewardTypeGroup
		[UnityEngine.SerializeField] long type = 0; // $type 練習能力タイプ。クライアント側で検索に用いる番号
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingEventRewardTypeDetailMasterObjectBase {
		public virtual TrainingEventRewardTypeDetailMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingEventRewardTypeGroup => _rawData._mTrainingEventRewardTypeGroup;
		public virtual long type => _rawData._type;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingEventRewardTypeDetailMasterValueObject _rawData = null;
		public TrainingEventRewardTypeDetailMasterObjectBase( TrainingEventRewardTypeDetailMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingEventRewardTypeDetailMasterObject : TrainingEventRewardTypeDetailMasterObjectBase, IMasterObject {
		public TrainingEventRewardTypeDetailMasterObject( TrainingEventRewardTypeDetailMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingEventRewardTypeDetailMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Event_Reward_Type_Detail;

        [UnityEngine.SerializeField]
        TrainingEventRewardTypeDetailMasterValueObject[] m_Training_Event_Reward_Type_Detail = null;
    }


}
