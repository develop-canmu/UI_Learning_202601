//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class GuildPlayStyleMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // ユーザー側で使用する名称
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class GuildPlayStyleMasterObjectBase {
		public virtual GuildPlayStyleMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        GuildPlayStyleMasterValueObject _rawData = null;
		public GuildPlayStyleMasterObjectBase( GuildPlayStyleMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class GuildPlayStyleMasterObject : GuildPlayStyleMasterObjectBase, IMasterObject {
		public GuildPlayStyleMasterObject( GuildPlayStyleMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class GuildPlayStyleMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Guild_Play_Style;

        [UnityEngine.SerializeField]
        GuildPlayStyleMasterValueObject[] m_Guild_Play_Style = null;
    }


}
