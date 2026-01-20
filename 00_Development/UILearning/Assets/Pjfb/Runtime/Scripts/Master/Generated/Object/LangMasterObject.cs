//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class LangMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _lang {get{ return lang;} set{ this.lang = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public string _dateTimeFormat {get{ return dateTimeFormat;} set{ this.dateTimeFormat = value;}}
		[MessagePack.Key(4)]
		public bool _isDev {get{ return isDev;} set{ this.isDev = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string lang = ""; // 言語を意味するキー。en,jp など。
		[UnityEngine.SerializeField] string name = ""; // ゲーム画面上で表示する言語名。
		[UnityEngine.SerializeField] string dateTimeFormat = ""; // 言語ごとの日時表記のフォーマット。PHP の DateTime::format() に渡す値。
		[UnityEngine.SerializeField] bool isDev = false; // 開発中（一般ユーザには表示しない）かどうか。1: 表示しない、2: 表示する。
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class LangMasterObjectBase {
		public virtual LangMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string lang => _rawData._lang;
		public virtual string name => _rawData._name;
		public virtual string dateTimeFormat => _rawData._dateTimeFormat;
		public virtual bool isDev => _rawData._isDev;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        LangMasterValueObject _rawData = null;
		public LangMasterObjectBase( LangMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class LangMasterObject : LangMasterObjectBase, IMasterObject {
		public LangMasterObject( LangMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class LangMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Lang;

        [UnityEngine.SerializeField]
        LangMasterValueObject[] m_Lang = null;
    }


}
