//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class LangDictionaryItMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _key {get{ return key;} set{ this.key = value;}}
		[MessagePack.Key(2)]
		public string _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string key = ""; // 辞書キー
		[UnityEngine.SerializeField] string value = ""; // 文言
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class LangDictionaryItMasterObjectBase {
		public virtual LangDictionaryItMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string key => _rawData._key;
		public virtual string value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        LangDictionaryItMasterValueObject _rawData = null;
		public LangDictionaryItMasterObjectBase( LangDictionaryItMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class LangDictionaryItMasterObject : LangDictionaryItMasterObjectBase, IMasterObject {
		public LangDictionaryItMasterObject( LangDictionaryItMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class LangDictionaryItMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Lang_Dictionary_It;

        [UnityEngine.SerializeField]
        LangDictionaryItMasterValueObject[] m_Lang_Dictionary_It = null;
    }


}
