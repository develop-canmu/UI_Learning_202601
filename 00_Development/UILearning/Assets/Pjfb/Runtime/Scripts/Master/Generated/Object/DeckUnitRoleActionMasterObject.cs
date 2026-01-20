//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckUnitRoleActionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mDeckUnitRoleOperationId {get{ return mDeckUnitRoleOperationId;} set{ this.mDeckUnitRoleOperationId = value;}}
		[MessagePack.Key(2)]
		public long _actionType {get{ return actionType;} set{ this.actionType = value;}}
		[MessagePack.Key(3)]
		public long _actionValue {get{ return actionValue;} set{ this.actionValue = value;}}
		[MessagePack.Key(4)]
		public string _invokeCondition {get{ return invokeCondition;} set{ this.invokeCondition = value;}}
		[MessagePack.Key(5)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mDeckUnitRoleOperationId = 0; // $mDeckUnitRoleOperationId 指示ID。どの指示に紐づいているか
		[UnityEngine.SerializeField] long actionType = 0; // $actionType 行動内容
		[UnityEngine.SerializeField] long actionValue = 0; // $actionValue 行動内容のvalue
		[UnityEngine.SerializeField] string invokeCondition = ""; // $invokeCondition 行動条件を指定するjson形式文字列
		[UnityEngine.SerializeField] long priority = 0; // $priority 優先度
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckUnitRoleActionMasterObjectBase {
		public virtual DeckUnitRoleActionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mDeckUnitRoleOperationId => _rawData._mDeckUnitRoleOperationId;
		public virtual long actionType => _rawData._actionType;
		public virtual long actionValue => _rawData._actionValue;
		public virtual string invokeCondition => _rawData._invokeCondition;
		public virtual long priority => _rawData._priority;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckUnitRoleActionMasterValueObject _rawData = null;
		public DeckUnitRoleActionMasterObjectBase( DeckUnitRoleActionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckUnitRoleActionMasterObject : DeckUnitRoleActionMasterObjectBase, IMasterObject {
		public DeckUnitRoleActionMasterObject( DeckUnitRoleActionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckUnitRoleActionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Unit_Role_Action;

        [UnityEngine.SerializeField]
        DeckUnitRoleActionMasterValueObject[] m_Deck_Unit_Role_Action = null;
    }


}
