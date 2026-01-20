//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class FestivalPointMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mFestivalId {get{ return mFestivalId;} set{ this.mFestivalId = value;}}
		[MessagePack.Key(2)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(3)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(4)]
		public long _thresholdMin {get{ return thresholdMin;} set{ this.thresholdMin = value;}}
		[MessagePack.Key(5)]
		public long _value {get{ return value;} set{ this.value = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mFestivalId = 0; // $mFestivalId m_festival の ID
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId 個人トレーニングのシナリオID
		[UnityEngine.SerializeField] long type = 0; // $type ポイント獲得方法。1: 育成キャラの最終ランクの rankNumber、2: トレーニング中のバトル勝利数
		[UnityEngine.SerializeField] long thresholdMin = 0; // $thresholdMin rankNumber またはバトル勝利数の必要最小値
		[UnityEngine.SerializeField] long value = 0; // $value 獲得ポイント量
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class FestivalPointMasterObjectBase {
		public virtual FestivalPointMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mFestivalId => _rawData._mFestivalId;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual long type => _rawData._type;
		public virtual long thresholdMin => _rawData._thresholdMin;
		public virtual long value => _rawData._value;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        FestivalPointMasterValueObject _rawData = null;
		public FestivalPointMasterObjectBase( FestivalPointMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class FestivalPointMasterObject : FestivalPointMasterObjectBase, IMasterObject {
		public FestivalPointMasterObject( FestivalPointMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class FestivalPointMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Festival_Point;

        [UnityEngine.SerializeField]
        FestivalPointMasterValueObject[] m_Festival_Point = null;
    }


}
