//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckUnitRoleMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _imageId {get{ return imageId;} set{ this.imageId = value;}}
		[MessagePack.Key(3)]
		public string _colorCode {get{ return colorCode;} set{ this.colorCode = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name ロール名
		[UnityEngine.SerializeField] long imageId = 0; // $imageId ユニット役割の画像ID。クライアント側での表示に使用
		[UnityEngine.SerializeField] string colorCode = ""; // $colorCode ユニット役割の表示用カラーコード。クライアント側での表示に使用
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckUnitRoleMasterObjectBase {
		public virtual DeckUnitRoleMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long imageId => _rawData._imageId;
		public virtual string colorCode => _rawData._colorCode;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckUnitRoleMasterValueObject _rawData = null;
		public DeckUnitRoleMasterObjectBase( DeckUnitRoleMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckUnitRoleMasterObject : DeckUnitRoleMasterObjectBase, IMasterObject {
		public DeckUnitRoleMasterObject( DeckUnitRoleMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckUnitRoleMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Unit_Role;

        [UnityEngine.SerializeField]
        DeckUnitRoleMasterValueObject[] m_Deck_Unit_Role = null;
    }


}
