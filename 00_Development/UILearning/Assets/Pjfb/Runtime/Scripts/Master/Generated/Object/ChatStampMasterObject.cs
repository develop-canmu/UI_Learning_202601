//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class ChatStampMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _isFreeFlg {get{ return isFreeFlg;} set{ this.isFreeFlg = value;}}
		[MessagePack.Key(3)]
		public long _sortNumber {get{ return sortNumber;} set{ this.sortNumber = value;}}
		[MessagePack.Key(4)]
		public string _imageName {get{ return imageName;} set{ this.imageName = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] long isFreeFlg = 0; // 無料なのか。1 => 無料、2 => 課金必要
		[UnityEngine.SerializeField] long sortNumber = 0; // ソート順
		[UnityEngine.SerializeField] string imageName = ""; // 画像名
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class ChatStampMasterObjectBase {
		public virtual ChatStampMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long isFreeFlg => _rawData._isFreeFlg;
		public virtual long sortNumber => _rawData._sortNumber;
		public virtual string imageName => _rawData._imageName;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        ChatStampMasterValueObject _rawData = null;
		public ChatStampMasterObjectBase( ChatStampMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class ChatStampMasterObject : ChatStampMasterObjectBase, IMasterObject {
		public ChatStampMasterObject( ChatStampMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class ChatStampMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chat_Stamp;

        [UnityEngine.SerializeField]
        ChatStampMasterValueObject[] m_Chat_Stamp = null;
    }


}
