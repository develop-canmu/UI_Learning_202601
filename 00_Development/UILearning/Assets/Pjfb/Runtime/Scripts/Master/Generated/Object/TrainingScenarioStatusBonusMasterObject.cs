//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingScenarioStatusBonusMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public long _enhanceRate {get{ return enhanceRate;} set{ this.enhanceRate = value;}}
		[MessagePack.Key(4)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(5)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId シナリオID
		[UnityEngine.SerializeField] long type = 0; // $type 1=>mTrainingCardLevelに対するベースステータスアップ、2=>mTrainingEventRewardに対するベースステータスアップ
		[UnityEngine.SerializeField] long enhanceRate = 0; // $enhanceRate 強化倍率
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 開始日時
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 終了日時
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingScenarioStatusBonusMasterObjectBase {
		public virtual TrainingScenarioStatusBonusMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual long type => _rawData._type;
		public virtual long enhanceRate => _rawData._enhanceRate;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingScenarioStatusBonusMasterValueObject _rawData = null;
		public TrainingScenarioStatusBonusMasterObjectBase( TrainingScenarioStatusBonusMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingScenarioStatusBonusMasterObject : TrainingScenarioStatusBonusMasterObjectBase, IMasterObject {
		public TrainingScenarioStatusBonusMasterObject( TrainingScenarioStatusBonusMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingScenarioStatusBonusMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Scenario_Status_Bonus;

        [UnityEngine.SerializeField]
        TrainingScenarioStatusBonusMasterValueObject[] m_Training_Scenario_Status_Bonus = null;
    }


}
