//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingBattleAdditionalRewardMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(2)]
		public string _conditionParam {get{ return conditionParam;} set{ this.conditionParam = value;}}
		[MessagePack.Key(3)]
		public string _operationType {get{ return operationType;} set{ this.operationType = value;}}
		[MessagePack.Key(4)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(5)]
		public string _paramEnhanceMap {get{ return paramEnhanceMap;} set{ this.paramEnhanceMap = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId 対象のシナリオID
		[UnityEngine.SerializeField] string conditionParam = ""; // $conditionParam バトル結果に含まれるパラメータ名
		[UnityEngine.SerializeField] string operationType = ""; // $operationType 比較演算子種別（EQ, GE, LE, BETWEENなど）
		[UnityEngine.SerializeField] long value = 0; // $value 比較値
		[UnityEngine.SerializeField] string paramEnhanceMap = ""; // $paramEnhanceMap 獲得ステータスにかけられるボーナス倍率（万分率）。 {"spd":2000} とすれば spd に20%ぶんボーナスがかかる
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingBattleAdditionalRewardMasterObjectBase {
		public virtual TrainingBattleAdditionalRewardMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual string conditionParam => _rawData._conditionParam;
		public virtual string operationType => _rawData._operationType;
		public virtual long value => _rawData._value;
		public virtual string paramEnhanceMap => _rawData._paramEnhanceMap;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingBattleAdditionalRewardMasterValueObject _rawData = null;
		public TrainingBattleAdditionalRewardMasterObjectBase( TrainingBattleAdditionalRewardMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingBattleAdditionalRewardMasterObject : TrainingBattleAdditionalRewardMasterObjectBase, IMasterObject {
		public TrainingBattleAdditionalRewardMasterObject( TrainingBattleAdditionalRewardMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingBattleAdditionalRewardMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Battle_Additional_Reward;

        [UnityEngine.SerializeField]
        TrainingBattleAdditionalRewardMasterValueObject[] m_Training_Battle_Additional_Reward = null;
    }


}
