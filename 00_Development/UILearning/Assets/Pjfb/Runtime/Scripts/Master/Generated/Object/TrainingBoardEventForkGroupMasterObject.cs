//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingBoardEventForkGroupMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(3)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理表示名
		[UnityEngine.SerializeField] long rate = 0; // $rate 確率重み。この分岐グループが抽選される確率を指定する
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingBoardEventForkGroupMasterObjectBase {
		public virtual TrainingBoardEventForkGroupMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual string adminName => _rawData._adminName;
		public virtual long rate => _rawData._rate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingBoardEventForkGroupMasterValueObject _rawData = null;
		public TrainingBoardEventForkGroupMasterObjectBase( TrainingBoardEventForkGroupMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingBoardEventForkGroupMasterObject : TrainingBoardEventForkGroupMasterObjectBase, IMasterObject {
		public TrainingBoardEventForkGroupMasterObject( TrainingBoardEventForkGroupMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingBoardEventForkGroupMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Board_Event_Fork_Group;

        [UnityEngine.SerializeField]
        TrainingBoardEventForkGroupMasterValueObject[] m_Training_Board_Event_Fork_Group = null;
    }


}
