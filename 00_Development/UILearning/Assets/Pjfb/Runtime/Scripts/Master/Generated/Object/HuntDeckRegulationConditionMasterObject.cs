//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntDeckRegulationConditionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mHuntDeckRegulationId {get{ return mHuntDeckRegulationId;} set{ this.mHuntDeckRegulationId = value;}}
		[MessagePack.Key(2)]
		public long _conditionLogicType {get{ return conditionLogicType;} set{ this.conditionLogicType = value;}}
		[MessagePack.Key(3)]
		public string _operatorType {get{ return operatorType;} set{ this.operatorType = value;}}
		[MessagePack.Key(4)]
		public long _compareValue {get{ return compareValue;} set{ this.compareValue = value;}}
		[MessagePack.Key(5)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mHuntDeckRegulationId = 0; // 対象の編成制限のID（m_hunt_deck_regulationのID）
		[UnityEngine.SerializeField] long conditionLogicType = 0; // 条件処理種別
		[UnityEngine.SerializeField] string operatorType = ""; // 比較演算子種別（EQ, GE, LE, BETWEENなど）
		[UnityEngine.SerializeField] long compareValue = 0; // 比較値
		[UnityEngine.SerializeField] long value = 0; // conditionLogicTypeによって自由に使用可能な値
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntDeckRegulationConditionMasterObjectBase {
		public virtual HuntDeckRegulationConditionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mHuntDeckRegulationId => _rawData._mHuntDeckRegulationId;
		public virtual long conditionLogicType => _rawData._conditionLogicType;
		public virtual string operatorType => _rawData._operatorType;
		public virtual long compareValue => _rawData._compareValue;
		public virtual long value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntDeckRegulationConditionMasterValueObject _rawData = null;
		public HuntDeckRegulationConditionMasterObjectBase( HuntDeckRegulationConditionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntDeckRegulationConditionMasterObject : HuntDeckRegulationConditionMasterObjectBase, IMasterObject {
		public HuntDeckRegulationConditionMasterObject( HuntDeckRegulationConditionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntDeckRegulationConditionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt_Deck_Regulation_Condition;

        [UnityEngine.SerializeField]
        HuntDeckRegulationConditionMasterValueObject[] m_Hunt_Deck_Regulation_Condition = null;
    }


}
