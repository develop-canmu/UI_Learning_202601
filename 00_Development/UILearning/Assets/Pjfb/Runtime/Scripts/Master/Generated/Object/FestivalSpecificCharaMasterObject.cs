//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class FestivalSpecificCharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mFestivalId {get{ return mFestivalId;} set{ this.mFestivalId = value;}}
		[MessagePack.Key(2)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(3)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(4)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(5)]
		public long _mCharaId {get{ return mCharaId;} set{ this.mCharaId = value;}}
		[MessagePack.Key(6)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mFestivalId = 0; // $mFestivalId m_festival の ID
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId 個人トレーニングのシナリオID
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 開始時刻
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 終了時刻
		[UnityEngine.SerializeField] long mCharaId = 0; // $mCharaId m_chara の ID
		[UnityEngine.SerializeField] long rate = 0; // $rate イベントポイントボーナス倍率（万分率）
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class FestivalSpecificCharaMasterObjectBase {
		public virtual FestivalSpecificCharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mFestivalId => _rawData._mFestivalId;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual long mCharaId => _rawData._mCharaId;
		public virtual long rate => _rawData._rate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        FestivalSpecificCharaMasterValueObject _rawData = null;
		public FestivalSpecificCharaMasterObjectBase( FestivalSpecificCharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class FestivalSpecificCharaMasterObject : FestivalSpecificCharaMasterObjectBase, IMasterObject {
		public FestivalSpecificCharaMasterObject( FestivalSpecificCharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class FestivalSpecificCharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Festival_Specific_Chara;

        [UnityEngine.SerializeField]
        FestivalSpecificCharaMasterValueObject[] m_Festival_Specific_Chara = null;
    }


}
