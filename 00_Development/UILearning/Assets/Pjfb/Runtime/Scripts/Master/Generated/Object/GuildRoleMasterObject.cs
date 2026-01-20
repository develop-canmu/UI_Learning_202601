//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class GuildRoleMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _accessLevel {get{ return accessLevel;} set{ this.accessLevel = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // ユーザー側で使用する名称
		[UnityEngine.SerializeField] long accessLevel = 0; // アクセスレベル 0=>ギルド無所属, 1=>ギルマス, 2=>サブリーダー, 3=>無職
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class GuildRoleMasterObjectBase {
		public virtual GuildRoleMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long accessLevel => _rawData._accessLevel;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        GuildRoleMasterValueObject _rawData = null;
		public GuildRoleMasterObjectBase( GuildRoleMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class GuildRoleMasterObject : GuildRoleMasterObjectBase, IMasterObject {
		public GuildRoleMasterObject( GuildRoleMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class GuildRoleMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Guild_Role;

        [UnityEngine.SerializeField]
        GuildRoleMasterValueObject[] m_Guild_Role = null;
    }


}
