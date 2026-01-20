//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CostPointEscalationMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCostPointEscalationGroupId {get{ return mCostPointEscalationGroupId;} set{ this.mCostPointEscalationGroupId = value;}}
		[MessagePack.Key(2)]
		public long _timesMax {get{ return timesMax;} set{ this.timesMax = value;}}
		[MessagePack.Key(3)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(4)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(5)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCostPointEscalationGroupId = 0; // $mCostPointEscalationGroupId 回数変動式ポイントコストグループのID（親テーブルは存在しない）
		[UnityEngine.SerializeField] long timesMax = 0; // $timesMax 何回目までこの設定が適用されるかの最大値。5の場合、5回目までこの設定が有効。より小さい数の値がある場合、そっちが優先される。グループ内で最も大きい回数を超えてしまった場合、これ以上実施はできない
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId コストに使用するポイントID
		[UnityEngine.SerializeField] long value = 0; // $value コストに使用する量
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CostPointEscalationMasterObjectBase {
		public virtual CostPointEscalationMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCostPointEscalationGroupId => _rawData._mCostPointEscalationGroupId;
		public virtual long timesMax => _rawData._timesMax;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CostPointEscalationMasterValueObject _rawData = null;
		public CostPointEscalationMasterObjectBase( CostPointEscalationMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CostPointEscalationMasterObject : CostPointEscalationMasterObjectBase, IMasterObject {
		public CostPointEscalationMasterObject( CostPointEscalationMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CostPointEscalationMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Cost_Point_Escalation;

        [UnityEngine.SerializeField]
        CostPointEscalationMasterValueObject[] m_Cost_Point_Escalation = null;
    }


}
