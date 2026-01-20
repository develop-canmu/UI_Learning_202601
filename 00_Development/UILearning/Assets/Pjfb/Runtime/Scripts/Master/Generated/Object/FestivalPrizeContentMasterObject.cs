//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class FestivalPrizeContentMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mFestivalPrizeContentGroup {get{ return mFestivalPrizeContentGroup;} set{ this.mFestivalPrizeContentGroup = value;}}
		[MessagePack.Key(2)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(3)]
		public long _samePrizeContentId {get{ return samePrizeContentId;} set{ this.samePrizeContentId = value;}}
		[MessagePack.Key(4)]
		public long _maxObtainCount {get{ return maxObtainCount;} set{ this.maxObtainCount = value;}}
		[MessagePack.Key(5)]
		public long _festivalPoint {get{ return festivalPoint;} set{ this.festivalPoint = value;}}
		[MessagePack.Key(6)]
		public string _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(7)]
		public bool _isFeatured {get{ return isFeatured;} set{ this.isFeatured = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mFestivalPrizeContentGroup = 0; // $mFestivalPrizeContentGroup m_festival_prize_setting との紐付け
		[UnityEngine.SerializeField] long rate = 0; // $rate 発生抽選時の抽選重み（万分率）
		[UnityEngine.SerializeField] long samePrizeContentId = 0; // $samePrizeContentId 同一として扱う追加報酬内容のID
		[UnityEngine.SerializeField] long maxObtainCount = 0; // $maxObtainCount 最大獲得回数。無制限の場合は0を指定する
		[UnityEngine.SerializeField] long festivalPoint = 0; // $festivalPoint 追加イベントポイント量。m_festival.type: 2の場合は使用されない
		[UnityEngine.SerializeField] string prizeJson = ""; // $prizeJson 追加報酬内容
		[UnityEngine.SerializeField] bool isFeatured = false; // $isFeatured 目玉報酬かどうか
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class FestivalPrizeContentMasterObjectBase {
		public virtual FestivalPrizeContentMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mFestivalPrizeContentGroup => _rawData._mFestivalPrizeContentGroup;
		public virtual long rate => _rawData._rate;
		public virtual long samePrizeContentId => _rawData._samePrizeContentId;
		public virtual long maxObtainCount => _rawData._maxObtainCount;
		public virtual long festivalPoint => _rawData._festivalPoint;
		public virtual string prizeJson => _rawData._prizeJson;
		public virtual bool isFeatured => _rawData._isFeatured;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        FestivalPrizeContentMasterValueObject _rawData = null;
		public FestivalPrizeContentMasterObjectBase( FestivalPrizeContentMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class FestivalPrizeContentMasterObject : FestivalPrizeContentMasterObjectBase, IMasterObject {
		public FestivalPrizeContentMasterObject( FestivalPrizeContentMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class FestivalPrizeContentMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Festival_Prize_Content;

        [UnityEngine.SerializeField]
        FestivalPrizeContentMasterValueObject[] m_Festival_Prize_Content = null;
    }


}
