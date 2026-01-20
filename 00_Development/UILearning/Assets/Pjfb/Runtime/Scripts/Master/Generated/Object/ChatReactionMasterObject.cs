//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ChatReactionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public bool _isFreeFlg {get{ return isFreeFlg;} set{ this.isFreeFlg = value;}}
		[MessagePack.Key(3)]
		public long _sortNumber {get{ return sortNumber;} set{ this.sortNumber = value;}}
		[MessagePack.Key(4)]
		public string _imageName {get{ return imageName;} set{ this.imageName = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name ユーザ側で使用する名称
		[UnityEngine.SerializeField] bool isFreeFlg = false; // $isFreeFlg 無料なのか。1 => 無料、2 => 課金必要
		[UnityEngine.SerializeField] long sortNumber = 0; // $sortNumber 並び順、降順
		[UnityEngine.SerializeField] string imageName = ""; // $imageName 画像名
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ChatReactionMasterObjectBase {
		public virtual ChatReactionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual bool isFreeFlg => _rawData._isFreeFlg;
		public virtual long sortNumber => _rawData._sortNumber;
		public virtual string imageName => _rawData._imageName;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ChatReactionMasterValueObject _rawData = null;
		public ChatReactionMasterObjectBase( ChatReactionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ChatReactionMasterObject : ChatReactionMasterObjectBase, IMasterObject {
		public ChatReactionMasterObject( ChatReactionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ChatReactionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chat_Reaction;

        [UnityEngine.SerializeField]
        ChatReactionMasterValueObject[] m_Chat_Reaction = null;
    }


}
