//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PlayerEnhanceMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(2)]
		public long _contentType {get{ return contentType;} set{ this.contentType = value;}}
		[MessagePack.Key(3)]
		public long _mEnhanceId {get{ return mEnhanceId;} set{ this.mEnhanceId = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理名
		[UnityEngine.SerializeField] long contentType = 0; // $contentType 種別
		[UnityEngine.SerializeField] long mEnhanceId = 0; // $mEnhanceId m_enhanceのid
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PlayerEnhanceMasterObjectBase {
		public virtual PlayerEnhanceMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string adminName => _rawData._adminName;
		public virtual long contentType => _rawData._contentType;
		public virtual long mEnhanceId => _rawData._mEnhanceId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PlayerEnhanceMasterValueObject _rawData = null;
		public PlayerEnhanceMasterObjectBase( PlayerEnhanceMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PlayerEnhanceMasterObject : PlayerEnhanceMasterObjectBase, IMasterObject {
		public PlayerEnhanceMasterObject( PlayerEnhanceMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PlayerEnhanceMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Player_Enhance;

        [UnityEngine.SerializeField]
        PlayerEnhanceMasterValueObject[] m_Player_Enhance = null;
    }


}
