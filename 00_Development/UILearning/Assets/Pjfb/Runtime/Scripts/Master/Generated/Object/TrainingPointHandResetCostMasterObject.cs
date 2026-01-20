//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingPointHandResetCostMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingPointHandResetCostGroup {get{ return mTrainingPointHandResetCostGroup;} set{ this.mTrainingPointHandResetCostGroup = value;}}
		[MessagePack.Key(2)]
		public long _count {get{ return count;} set{ this.count = value;}}
		[MessagePack.Key(3)]
		public long _costValue {get{ return costValue;} set{ this.costValue = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingPointHandResetCostGroup = 0; // $mTrainingPointHandResetCostGroup グループID
		[UnityEngine.SerializeField] long count = 0; // $count 何回目の実行か
		[UnityEngine.SerializeField] long costValue = 0; // $costValue 消費量
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingPointHandResetCostMasterObjectBase {
		public virtual TrainingPointHandResetCostMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingPointHandResetCostGroup => _rawData._mTrainingPointHandResetCostGroup;
		public virtual long count => _rawData._count;
		public virtual long costValue => _rawData._costValue;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingPointHandResetCostMasterValueObject _rawData = null;
		public TrainingPointHandResetCostMasterObjectBase( TrainingPointHandResetCostMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingPointHandResetCostMasterObject : TrainingPointHandResetCostMasterObjectBase, IMasterObject {
		public TrainingPointHandResetCostMasterObject( TrainingPointHandResetCostMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingPointHandResetCostMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Point_Hand_Reset_Cost;

        [UnityEngine.SerializeField]
        TrainingPointHandResetCostMasterValueObject[] m_Training_Point_Hand_Reset_Cost = null;
    }


}
