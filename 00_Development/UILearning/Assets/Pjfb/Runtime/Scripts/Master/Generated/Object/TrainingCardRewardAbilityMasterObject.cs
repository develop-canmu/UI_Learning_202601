//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardRewardAbilityMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingCardId {get{ return mTrainingCardId;} set{ this.mTrainingCardId = value;}}
		[MessagePack.Key(2)]
		public long _level {get{ return level;} set{ this.level = value;}}
		[MessagePack.Key(3)]
		public long _abilityId {get{ return abilityId;} set{ this.abilityId = value;}}
		[MessagePack.Key(4)]
		public long _abilityLevel {get{ return abilityLevel;} set{ this.abilityLevel = value;}}
		[MessagePack.Key(5)]
		public long _abilityRate {get{ return abilityRate;} set{ this.abilityRate = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingCardId = 0; // $mTrainingCardId 練習カードID
		[UnityEngine.SerializeField] long level = 0; // $level カードレベル
		[UnityEngine.SerializeField] long abilityId = 0; // $abilityId この練習カードによる練習で得られるスキル
		[UnityEngine.SerializeField] long abilityLevel = 0; // $abilityLevel この練習カードによる練習で得られるスキルのレベル
		[UnityEngine.SerializeField] long abilityRate = 0; // $abilityRate スキル取得確率（万分率）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardRewardAbilityMasterObjectBase {
		public virtual TrainingCardRewardAbilityMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingCardId => _rawData._mTrainingCardId;
		public virtual long level => _rawData._level;
		public virtual long abilityId => _rawData._abilityId;
		public virtual long abilityLevel => _rawData._abilityLevel;
		public virtual long abilityRate => _rawData._abilityRate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardRewardAbilityMasterValueObject _rawData = null;
		public TrainingCardRewardAbilityMasterObjectBase( TrainingCardRewardAbilityMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardRewardAbilityMasterObject : TrainingCardRewardAbilityMasterObjectBase, IMasterObject {
		public TrainingCardRewardAbilityMasterObject( TrainingCardRewardAbilityMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardRewardAbilityMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card_Reward_Ability;

        [UnityEngine.SerializeField]
        TrainingCardRewardAbilityMasterValueObject[] m_Training_Card_Reward_Ability = null;
    }


}
