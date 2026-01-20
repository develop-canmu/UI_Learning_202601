//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingConcentrationEffectMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingConcentrationEffectNumber {get{ return mTrainingConcentrationEffectNumber;} set{ this.mTrainingConcentrationEffectNumber = value;}}
		[MessagePack.Key(2)]
		public string _text {get{ return text;} set{ this.text = value;}}
		[MessagePack.Key(3)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingConcentrationEffectNumber = 0; // $mTrainingConcentrationEffectNumber m_training_concentrationのeffectNumber
		[UnityEngine.SerializeField] string text = ""; // $text 表示テキスト
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingConcentrationEffectMasterObjectBase {
		public virtual TrainingConcentrationEffectMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingConcentrationEffectNumber => _rawData._mTrainingConcentrationEffectNumber;
		public virtual string text => _rawData._text;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingConcentrationEffectMasterValueObject _rawData = null;
		public TrainingConcentrationEffectMasterObjectBase( TrainingConcentrationEffectMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingConcentrationEffectMasterObject : TrainingConcentrationEffectMasterObjectBase, IMasterObject {
		public TrainingConcentrationEffectMasterObject( TrainingConcentrationEffectMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingConcentrationEffectMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Concentration_Effect;

        [UnityEngine.SerializeField]
        TrainingConcentrationEffectMasterValueObject[] m_Training_Concentration_Effect = null;
    }


}
