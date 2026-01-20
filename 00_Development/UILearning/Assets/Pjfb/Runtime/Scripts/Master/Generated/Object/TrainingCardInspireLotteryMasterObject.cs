//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardInspireLotteryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingCardInspireLotteryGroup {get{ return mTrainingCardInspireLotteryGroup;} set{ this.mTrainingCardInspireLotteryGroup = value;}}
		[MessagePack.Key(2)]
		public long _mTrainingCardInspireId {get{ return mTrainingCardInspireId;} set{ this.mTrainingCardInspireId = value;}}
		[MessagePack.Key(3)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingCardInspireLotteryGroup = 0; // $mTrainingCardInspireLotteryGroup グループID
		[UnityEngine.SerializeField] long mTrainingCardInspireId = 0; // $mTrainingCardInspireId m_training_card_inspireのid
		[UnityEngine.SerializeField] long rate = 0; // $rate 抽選確率（万分率）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardInspireLotteryMasterObjectBase {
		public virtual TrainingCardInspireLotteryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingCardInspireLotteryGroup => _rawData._mTrainingCardInspireLotteryGroup;
		public virtual long mTrainingCardInspireId => _rawData._mTrainingCardInspireId;
		public virtual long rate => _rawData._rate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardInspireLotteryMasterValueObject _rawData = null;
		public TrainingCardInspireLotteryMasterObjectBase( TrainingCardInspireLotteryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardInspireLotteryMasterObject : TrainingCardInspireLotteryMasterObjectBase, IMasterObject {
		public TrainingCardInspireLotteryMasterObject( TrainingCardInspireLotteryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardInspireLotteryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card_Inspire_Lottery;

        [UnityEngine.SerializeField]
        TrainingCardInspireLotteryMasterValueObject[] m_Training_Card_Inspire_Lottery = null;
    }


}
