//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingConcentrationLotteryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(2)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(3)]
		public long _mTrainingConcentrationId {get{ return mTrainingConcentrationId;} set{ this.mTrainingConcentrationId = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId シナリオID
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId キャラの指定。指定しない場合は0
		[UnityEngine.SerializeField] long mTrainingConcentrationId = 0; // $mTrainingConcentrationId コンセントレーションゾーンID
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingConcentrationLotteryMasterObjectBase {
		public virtual TrainingConcentrationLotteryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long mTrainingConcentrationId => _rawData._mTrainingConcentrationId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingConcentrationLotteryMasterValueObject _rawData = null;
		public TrainingConcentrationLotteryMasterObjectBase( TrainingConcentrationLotteryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingConcentrationLotteryMasterObject : TrainingConcentrationLotteryMasterObjectBase, IMasterObject {
		public TrainingConcentrationLotteryMasterObject( TrainingConcentrationLotteryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingConcentrationLotteryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Concentration_Lottery;

        [UnityEngine.SerializeField]
        TrainingConcentrationLotteryMasterValueObject[] m_Training_Concentration_Lottery = null;
    }


}
