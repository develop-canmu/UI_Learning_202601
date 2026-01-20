//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class GuildEmblemMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string adminName = ""; // 管理側で使用する名称
		[UnityEngine.SerializeField] string name = ""; // ユーザ側で使用する名称
		[UnityEngine.SerializeField] long type = 0; // エンブレムの種類。1 => 一般,  2 => 特別（一般は付与等を行わずに最初から使用でき、特別は付与を必要とする）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class GuildEmblemMasterObjectBase {
		public virtual GuildEmblemMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string adminName => _rawData._adminName;
		public virtual string name => _rawData._name;
		public virtual long type => _rawData._type;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        GuildEmblemMasterValueObject _rawData = null;
		public GuildEmblemMasterObjectBase( GuildEmblemMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class GuildEmblemMasterObject : GuildEmblemMasterObjectBase, IMasterObject {
		public GuildEmblemMasterObject( GuildEmblemMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class GuildEmblemMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Guild_Emblem;

        [UnityEngine.SerializeField]
        GuildEmblemMasterValueObject[] m_Guild_Emblem = null;
    }


}
