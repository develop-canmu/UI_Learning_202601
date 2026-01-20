//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntStageMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mHuntId {get{ return mHuntId;} set{ this.mHuntId = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public string _subName {get{ return subName;} set{ this.subName = value;}}
		[MessagePack.Key(4)]
		public long _progressMin {get{ return progressMin;} set{ this.progressMin = value;}}
		[MessagePack.Key(5)]
		public long _progressMax {get{ return progressMax;} set{ this.progressMax = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mHuntId = 0; // $mHuntId 狩猟イベントID
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] string subName = ""; // $subName サブ名称
		[UnityEngine.SerializeField] long progressMin = 0; // $progressMin 該当の狩猟イベントの、どこからがこのステージに属するか
		[UnityEngine.SerializeField] long progressMax = 0; // $progressMax 該当の狩猟イベントの、どこまでがこのステージに属するか
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntStageMasterObjectBase {
		public virtual HuntStageMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mHuntId => _rawData._mHuntId;
		public virtual string name => _rawData._name;
		public virtual string subName => _rawData._subName;
		public virtual long progressMin => _rawData._progressMin;
		public virtual long progressMax => _rawData._progressMax;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntStageMasterValueObject _rawData = null;
		public HuntStageMasterObjectBase( HuntStageMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntStageMasterObject : HuntStageMasterObjectBase, IMasterObject {
		public HuntStageMasterObject( HuntStageMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntStageMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt_Stage;

        [UnityEngine.SerializeField]
        HuntStageMasterValueObject[] m_Hunt_Stage = null;
    }


}
