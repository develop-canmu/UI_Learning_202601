//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckUnitRoleOperationMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mDeckUnitRoleId {get{ return mDeckUnitRoleId;} set{ this.mDeckUnitRoleId = value;}}
		[MessagePack.Key(2)]
		public bool _isDefault {get{ return isDefault;} set{ this.isDefault = value;}}
		[MessagePack.Key(3)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(4)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mDeckUnitRoleId = 0; // $mDeckUnitRoleId ユニット役割ID
		[UnityEngine.SerializeField] bool isDefault = false; // $isDefault そのユニット役割におけるデフォルトの指示であるかどうか。各ユニット役割に対してデフォルトのものはただ1つ存在する。 1 => デフォルト、2 => 通常（デフォルトでない）
		[UnityEngine.SerializeField] string name = ""; // $name 名前
		[UnityEngine.SerializeField] string description = ""; // $description 説明
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckUnitRoleOperationMasterObjectBase {
		public virtual DeckUnitRoleOperationMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mDeckUnitRoleId => _rawData._mDeckUnitRoleId;
		public virtual bool isDefault => _rawData._isDefault;
		public virtual string name => _rawData._name;
		public virtual string description => _rawData._description;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckUnitRoleOperationMasterValueObject _rawData = null;
		public DeckUnitRoleOperationMasterObjectBase( DeckUnitRoleOperationMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckUnitRoleOperationMasterObject : DeckUnitRoleOperationMasterObjectBase, IMasterObject {
		public DeckUnitRoleOperationMasterObject( DeckUnitRoleOperationMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckUnitRoleOperationMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Unit_Role_Operation;

        [UnityEngine.SerializeField]
        DeckUnitRoleOperationMasterValueObject[] m_Deck_Unit_Role_Operation = null;
    }


}
