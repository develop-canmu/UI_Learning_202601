//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingEventRewardTypeGroupMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingEventRewardTypeGroup {get{ return mTrainingEventRewardTypeGroup;} set{ this.mTrainingEventRewardTypeGroup = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public long _iconId {get{ return iconId;} set{ this.iconId = value;}}
		[MessagePack.Key(4)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(5)]
		public bool _isChangeVisible {get{ return isChangeVisible;} set{ this.isChangeVisible = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingEventRewardTypeGroup = 0; // $mTrainingEventRewardTypeGroup
		[UnityEngine.SerializeField] string name = ""; // $name 名前
		[UnityEngine.SerializeField] long iconId = 0; // $iconId アイコンの画像ID
		[UnityEngine.SerializeField] long priority = 0; // $priority 優先度
		[UnityEngine.SerializeField] bool isChangeVisible = false; // $isChangeVisible 変化がある時のみ表示するか。1=>変化がある時のみ、2=>常に表示
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingEventRewardTypeGroupMasterObjectBase {
		public virtual TrainingEventRewardTypeGroupMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingEventRewardTypeGroup => _rawData._mTrainingEventRewardTypeGroup;
		public virtual string name => _rawData._name;
		public virtual long iconId => _rawData._iconId;
		public virtual long priority => _rawData._priority;
		public virtual bool isChangeVisible => _rawData._isChangeVisible;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingEventRewardTypeGroupMasterValueObject _rawData = null;
		public TrainingEventRewardTypeGroupMasterObjectBase( TrainingEventRewardTypeGroupMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingEventRewardTypeGroupMasterObject : TrainingEventRewardTypeGroupMasterObjectBase, IMasterObject {
		public TrainingEventRewardTypeGroupMasterObject( TrainingEventRewardTypeGroupMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingEventRewardTypeGroupMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Event_Reward_Type_Group;

        [UnityEngine.SerializeField]
        TrainingEventRewardTypeGroupMasterValueObject[] m_Training_Event_Reward_Type_Group = null;
    }


}
