//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingConditionTierMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(2)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(3)]
		public long _tier {get{ return tier;} set{ this.tier = value;}}
		[MessagePack.Key(4)]
		public long _min {get{ return min;} set{ this.min = value;}}
		[MessagePack.Key(5)]
		public long _max {get{ return max;} set{ this.max = value;}}
		[MessagePack.Key(6)]
		public long _triggerValue {get{ return triggerValue;} set{ this.triggerValue = value;}}
		[MessagePack.Key(7)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId トレーニングシナリオID
		[UnityEngine.SerializeField] string name = ""; // $name 名前
		[UnityEngine.SerializeField] long tier = 0; // $tier このコンディション帯の番号。0がもっとも低いコンディション
		[UnityEngine.SerializeField] long min = 0; // $min このコンディション帯におけるコンディション値の最小値
		[UnityEngine.SerializeField] long max = 0; // $max このコンディション帯におけるコンディション値の最大値
		[UnityEngine.SerializeField] long triggerValue = 0; // $triggerValue このコンディション帯になるために到達すべきコンディション値
		[UnityEngine.SerializeField] long type = 0; // $type 種別。1=>通常、2=>特殊条件でのみ発動
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingConditionTierMasterObjectBase {
		public virtual TrainingConditionTierMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual string name => _rawData._name;
		public virtual long tier => _rawData._tier;
		public virtual long min => _rawData._min;
		public virtual long max => _rawData._max;
		public virtual long triggerValue => _rawData._triggerValue;
		public virtual long type => _rawData._type;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingConditionTierMasterValueObject _rawData = null;
		public TrainingConditionTierMasterObjectBase( TrainingConditionTierMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingConditionTierMasterObject : TrainingConditionTierMasterObjectBase, IMasterObject {
		public TrainingConditionTierMasterObject( TrainingConditionTierMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingConditionTierMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Condition_Tier;

        [UnityEngine.SerializeField]
        TrainingConditionTierMasterValueObject[] m_Training_Condition_Tier = null;
    }


}
