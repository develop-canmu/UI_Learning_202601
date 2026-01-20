//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class TrainingIntentionalEventMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(2)]
		public long _mTrainingEventId {get{ return mTrainingEventId;} set{ this.mTrainingEventId = value;}}
		[MessagePack.Key(3)]
		public long _combatPowerMin {get{ return combatPowerMin;} set{ this.combatPowerMin = value;}}
		[MessagePack.Key(4)]
		public long _combatPowerMax {get{ return combatPowerMax;} set{ this.combatPowerMax = value;}}
		[MessagePack.Key(5)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(6)]
		public long _rarity {get{ return rarity;} set{ this.rarity = value;}}
		[MessagePack.Key(7)]
		public long _expectedMTrainingEventRewardId {get{ return expectedMTrainingEventRewardId;} set{ this.expectedMTrainingEventRewardId = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId トレーニングシナリオID
		[UnityEngine.SerializeField] long mTrainingEventId = 0; // $mTrainingEventId 発生するイベントのID
		[UnityEngine.SerializeField] long combatPowerMin = 0; // $combatPowerMin この任意イベントが発生する条件となる最小の総合力（育成対象キャラクター）
		[UnityEngine.SerializeField] long combatPowerMax = 0; // $combatPowerMax この任意イベントが発生する条件となる最大の総合力（育成対象キャラクター）
		[UnityEngine.SerializeField] long rate = 0; // $rate 実行可能なイベントを抽選する際の確率重み
		[UnityEngine.SerializeField] long rarity = 0; // $rarity 発生のレア度
		[UnityEngine.SerializeField] long expectedMTrainingEventRewardId = 0; // $expectedMTrainingEventRewardId このイベントが発生することで得られることが期待されるイベント報酬のID。m_training_event.choicePrizeJson で設定されたものが実際に得られる報酬となるが、バトルイベント等で後続のイベント等に報酬の設定をしている場合があり、その場合はサーバ上でイベントに紐づく報酬の取得が困難なため、このカラムで表示上の報酬を設定する
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class TrainingIntentionalEventMasterObjectBase {
		public virtual TrainingIntentionalEventMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual long mTrainingEventId => _rawData._mTrainingEventId;
		public virtual long combatPowerMin => _rawData._combatPowerMin;
		public virtual long combatPowerMax => _rawData._combatPowerMax;
		public virtual long rate => _rawData._rate;
		public virtual long rarity => _rawData._rarity;
		public virtual long expectedMTrainingEventRewardId => _rawData._expectedMTrainingEventRewardId;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        TrainingIntentionalEventMasterValueObject _rawData = null;
		public TrainingIntentionalEventMasterObjectBase( TrainingIntentionalEventMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class TrainingIntentionalEventMasterObject : TrainingIntentionalEventMasterObjectBase, IMasterObject {
		public TrainingIntentionalEventMasterObject( TrainingIntentionalEventMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class TrainingIntentionalEventMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Training_Intentional_Event;

        [UnityEngine.SerializeField]
        TrainingIntentionalEventMasterValueObject[] m_Training_Intentional_Event = null;
    }


}
