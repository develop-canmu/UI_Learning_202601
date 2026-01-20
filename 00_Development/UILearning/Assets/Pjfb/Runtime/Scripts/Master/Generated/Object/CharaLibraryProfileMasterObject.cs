//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaLibraryProfileMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _masterType {get{ return masterType;} set{ this.masterType = value;}}
		[MessagePack.Key(2)]
		public long _masterId {get{ return masterId;} set{ this.masterId = value;}}
		[MessagePack.Key(3)]
		public long _useType {get{ return useType;} set{ this.useType = value;}}
		[MessagePack.Key(4)]
		public string _text {get{ return text;} set{ this.text = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long masterType = 0; // 1 => m_chara.parentMCharaId, 2 => m_chara.id
		[UnityEngine.SerializeField] long masterId = 0; // idTypeの区分に応じて、別なテーブルのIDを参照
		[UnityEngine.SerializeField] long useType = 0; // 1 => 名前, 2 => 名前（名前英語表記）, 3 => 声優名, 4 => 誕生日, 5 => 身長, 6 => 体重, 7 => 特技, 8=> 解説文
		[UnityEngine.SerializeField] string text = ""; // テキスト
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaLibraryProfileMasterObjectBase {
		public virtual CharaLibraryProfileMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long masterType => _rawData._masterType;
		public virtual long masterId => _rawData._masterId;
		public virtual long useType => _rawData._useType;
		public virtual string text => _rawData._text;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaLibraryProfileMasterValueObject _rawData = null;
		public CharaLibraryProfileMasterObjectBase( CharaLibraryProfileMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaLibraryProfileMasterObject : CharaLibraryProfileMasterObjectBase, IMasterObject {
		public CharaLibraryProfileMasterObject( CharaLibraryProfileMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaLibraryProfileMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Library_Profile;

        [UnityEngine.SerializeField]
        CharaLibraryProfileMasterValueObject[] m_Chara_Library_Profile = null;
    }


}
