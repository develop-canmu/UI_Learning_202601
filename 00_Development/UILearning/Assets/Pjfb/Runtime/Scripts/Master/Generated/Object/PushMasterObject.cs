//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class PushMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public string _keyName {get{ return keyName;} set{ this.keyName = value;}}
		[MessagePack.Key(4)]
		public string _titleText {get{ return titleText;} set{ this.titleText = value;}}
		[MessagePack.Key(5)]
		public string _bodyBaseText {get{ return bodyBaseText;} set{ this.bodyBaseText = value;}}
		[MessagePack.Key(6)]
		public bool _sentDefault {get{ return sentDefault;} set{ this.sentDefault = value;}}
		[MessagePack.Key(7)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(8)]
		public PushPushOptionJson _optionJson {get{ return optionJson;} set{ this.optionJson = value;}}
		[MessagePack.Key(9)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id ID
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理名称
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
		[UnityEngine.SerializeField] string keyName = ""; // $keyName 検索する時に使う名前
		[UnityEngine.SerializeField] string titleText = ""; // $titleText 送信されるプッシュのタイトル文字列
		[UnityEngine.SerializeField] string bodyBaseText = ""; // $bodyBaseText 送信されるプッシュの本文のベース文字列
		[UnityEngine.SerializeField] bool sentDefault = false; // $sentDefault デフォルトでプッシュが送信されるか。1:送信する、2:送信しない
		[UnityEngine.SerializeField] long priority = 0; // $priority 表示優先度。0なら表示しない
		[UnityEngine.SerializeField] PushPushOptionJson optionJson = null; // 一部のPUSH区分で使用する、特殊パラメーター（主に、ネイティブタイトルのローカル通知で使用する）。全PUSHで運用する共通値ではなく基本的には個別の組み込みとセットのため、使えるだろうと憶測で使用するのはやめる。特に設定しない場合は空JSON{}を指定。空の場合クライアントにはnullで渡る
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class PushMasterObjectBase {
		public virtual PushMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string adminName => _rawData._adminName;
		public virtual string name => _rawData._name;
		public virtual string keyName => _rawData._keyName;
		public virtual string titleText => _rawData._titleText;
		public virtual string bodyBaseText => _rawData._bodyBaseText;
		public virtual bool sentDefault => _rawData._sentDefault;
		public virtual long priority => _rawData._priority;
		public virtual PushPushOptionJson optionJson => _rawData._optionJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        PushMasterValueObject _rawData = null;
		public PushMasterObjectBase( PushMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class PushMasterObject : PushMasterObjectBase, IMasterObject {
		public PushMasterObject( PushMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class PushMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Push;

        [UnityEngine.SerializeField]
        PushMasterValueObject[] m_Push = null;
    }


}
