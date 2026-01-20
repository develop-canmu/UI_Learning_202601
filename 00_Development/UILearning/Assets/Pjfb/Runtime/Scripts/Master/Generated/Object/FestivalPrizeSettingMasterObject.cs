//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class FestivalPrizeSettingMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mFestivalId {get{ return mFestivalId;} set{ this.mFestivalId = value;}}
		[MessagePack.Key(2)]
		public long _mTrainingScenarioId {get{ return mTrainingScenarioId;} set{ this.mTrainingScenarioId = value;}}
		[MessagePack.Key(3)]
		public long _battleCount {get{ return battleCount;} set{ this.battleCount = value;}}
		[MessagePack.Key(4)]
		public long _countMin {get{ return countMin;} set{ this.countMin = value;}}
		[MessagePack.Key(5)]
		public long _countMax {get{ return countMax;} set{ this.countMax = value;}}
		[MessagePack.Key(6)]
		public string _lotteryOrder {get{ return lotteryOrder;} set{ this.lotteryOrder = value;}}
		[MessagePack.Key(7)]
		public long _mFestivalPrizeContentGroup {get{ return mFestivalPrizeContentGroup;} set{ this.mFestivalPrizeContentGroup = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mFestivalId = 0; // $mFestivalId イベントID
		[UnityEngine.SerializeField] long mTrainingScenarioId = 0; // $mTrainingScenarioId シナリオID
		[UnityEngine.SerializeField] long battleCount = 0; // $battleCount シナリオ起因のバトル通過回数、すなわちシナリオ進行度
		[UnityEngine.SerializeField] long countMin = 0; // $countMin 最小追加報酬発生数。上記ターン区間内に発生する追加報酬の最小個数を定める
		[UnityEngine.SerializeField] long countMax = 0; // $countMax 最大追加報酬発生数。上記ターン区間内に発生する追加報酬の最大個数を定める
		[UnityEngine.SerializeField] string lotteryOrder = ""; // $lotteryOrder ターン別マス抽選順序。[5,10]のような形で設定すれば、5ターン目、10ターン目の順にマス配置抽選を優先的に行う（他は順不同）
		[UnityEngine.SerializeField] long mFestivalPrizeContentGroup = 0; // $mFestivalPrizeContentGroup m_festival_prize_matching との紐付け。獲得しうる追加報酬内容の組み合わせを指定する
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class FestivalPrizeSettingMasterObjectBase {
		public virtual FestivalPrizeSettingMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mFestivalId => _rawData._mFestivalId;
		public virtual long mTrainingScenarioId => _rawData._mTrainingScenarioId;
		public virtual long battleCount => _rawData._battleCount;
		public virtual long countMin => _rawData._countMin;
		public virtual long countMax => _rawData._countMax;
		public virtual string lotteryOrder => _rawData._lotteryOrder;
		public virtual long mFestivalPrizeContentGroup => _rawData._mFestivalPrizeContentGroup;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        FestivalPrizeSettingMasterValueObject _rawData = null;
		public FestivalPrizeSettingMasterObjectBase( FestivalPrizeSettingMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class FestivalPrizeSettingMasterObject : FestivalPrizeSettingMasterObjectBase, IMasterObject {
		public FestivalPrizeSettingMasterObject( FestivalPrizeSettingMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class FestivalPrizeSettingMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Festival_Prize_Setting;

        [UnityEngine.SerializeField]
        FestivalPrizeSettingMasterValueObject[] m_Festival_Prize_Setting = null;
    }


}
