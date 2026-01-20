//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ProfilePartMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(2)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long type = 0; // 1=>m_chara_library_voice、2=>m_profile_frame、3=>m_profile_chara、4=>m_profile_background、5=>m_emblem
		[UnityEngine.SerializeField] long imageId = 0; // 画像ID
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ProfilePartMasterObjectBase {
		public virtual ProfilePartMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long type => _rawData._type;
		public virtual long imageId => _rawData._imageId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ProfilePartMasterValueObject _rawData = null;
		public ProfilePartMasterObjectBase( ProfilePartMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ProfilePartMasterObject : ProfilePartMasterObjectBase, IMasterObject {
		public ProfilePartMasterObject( ProfilePartMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ProfilePartMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Profile_Part;

        [UnityEngine.SerializeField]
        ProfilePartMasterValueObject[] m_Profile_Part = null;
    }


}
