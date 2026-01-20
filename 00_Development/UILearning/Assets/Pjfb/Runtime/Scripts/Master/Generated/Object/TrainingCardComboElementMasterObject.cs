//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardComboElementMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingCardComboId {get{ return mTrainingCardComboId;} set{ this.mTrainingCardComboId = value;}}
		[MessagePack.Key(2)]
		public long _mTrainingCardId {get{ return mTrainingCardId;} set{ this.mTrainingCardId = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingCardComboId = 0; // $mTrainingCardComboId m_training_card_comboのid
		[UnityEngine.SerializeField] long mTrainingCardId = 0; // $mTrainingCardId m_training_cardのid
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardComboElementMasterObjectBase {
		public virtual TrainingCardComboElementMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingCardComboId => _rawData._mTrainingCardComboId;
		public virtual long mTrainingCardId => _rawData._mTrainingCardId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardComboElementMasterValueObject _rawData = null;
		public TrainingCardComboElementMasterObjectBase( TrainingCardComboElementMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardComboElementMasterObject : TrainingCardComboElementMasterObjectBase, IMasterObject {
		public TrainingCardComboElementMasterObject( TrainingCardComboElementMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardComboElementMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card_Combo_Element;

        [UnityEngine.SerializeField]
        TrainingCardComboElementMasterValueObject[] m_Training_Card_Combo_Element = null;
    }


}
