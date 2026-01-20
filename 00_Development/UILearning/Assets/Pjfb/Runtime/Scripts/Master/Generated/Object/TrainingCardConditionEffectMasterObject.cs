//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingCardConditionEffectMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mCharaVariableConditionId {get{ return mCharaVariableConditionId;} set{ this.mCharaVariableConditionId = value;}}
		[MessagePack.Key(2)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(3)]
		public long _minCondition {get{ return minCondition;} set{ this.minCondition = value;}}
		[MessagePack.Key(4)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(5)]
		public long _effectRate {get{ return effectRate;} set{ this.effectRate = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mCharaVariableConditionId = 0; // $mCharaVariableConditionId 可変キャラのパラメータ条件。この条件を使用しないシナリオや、どの条件も満たさない場合用に全て0を指定したセットが必要
		[UnityEngine.SerializeField] long type = 0; // $type 種別。1=>通常ボーナス、2=>追加ボーナス
		[UnityEngine.SerializeField] long minCondition = 0; // $minCondition コンディション範囲の最低値
		[UnityEngine.SerializeField] long rate = 0; // $rate 当該コンディション値範囲において、effectRate がこの値となる確率
		[UnityEngine.SerializeField] long effectRate = 0; // $effectRate 練習効果倍率。万分率で指定し、等倍なら10000とする
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingCardConditionEffectMasterObjectBase {
		public virtual TrainingCardConditionEffectMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mCharaVariableConditionId => _rawData._mCharaVariableConditionId;
		public virtual long type => _rawData._type;
		public virtual long minCondition => _rawData._minCondition;
		public virtual long rate => _rawData._rate;
		public virtual long effectRate => _rawData._effectRate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingCardConditionEffectMasterValueObject _rawData = null;
		public TrainingCardConditionEffectMasterObjectBase( TrainingCardConditionEffectMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingCardConditionEffectMasterObject : TrainingCardConditionEffectMasterObjectBase, IMasterObject {
		public TrainingCardConditionEffectMasterObject( TrainingCardConditionEffectMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingCardConditionEffectMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Card_Condition_Effect;

        [UnityEngine.SerializeField]
        TrainingCardConditionEffectMasterValueObject[] m_Training_Card_Condition_Effect = null;
    }


}
