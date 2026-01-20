//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaVariableConditionElementMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaVariableConditionId {get{ return mCharaVariableConditionId;} set{ this.mCharaVariableConditionId = value;}}
		[MessagePack.Key(2)]
		public string _targetColumn {get{ return targetColumn;} set{ this.targetColumn = value;}}
		[MessagePack.Key(3)]
		public string _operatorType {get{ return operatorType;} set{ this.operatorType = value;}}
		[MessagePack.Key(4)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaVariableConditionId = 0; // $mCharaVariableConditionId この要素を条件として含む条件ID
		[UnityEngine.SerializeField] string targetColumn = ""; // $targetColumn 条件対象カラム
		[UnityEngine.SerializeField] string operatorType = ""; // $operatorType 比較演算子種別（EQ, GE, LE, BETWEENなど）
		[UnityEngine.SerializeField] long value = 0; // $value 比較値
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaVariableConditionElementMasterObjectBase {
		public virtual CharaVariableConditionElementMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaVariableConditionId => _rawData._mCharaVariableConditionId;
		public virtual string targetColumn => _rawData._targetColumn;
		public virtual string operatorType => _rawData._operatorType;
		public virtual long value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaVariableConditionElementMasterValueObject _rawData = null;
		public CharaVariableConditionElementMasterObjectBase( CharaVariableConditionElementMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaVariableConditionElementMasterObject : CharaVariableConditionElementMasterObjectBase, IMasterObject {
		public CharaVariableConditionElementMasterObject( CharaVariableConditionElementMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaVariableConditionElementMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Variable_Condition_Element;

        [UnityEngine.SerializeField]
        CharaVariableConditionElementMasterValueObject[] m_Chara_Variable_Condition_Element = null;
    }


}
