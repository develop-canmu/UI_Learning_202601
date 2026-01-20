//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntTimetableMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mHuntId {get{ return mHuntId;} set{ this.mHuntId = value;}}
		[MessagePack.Key(2)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(3)]
		public long _type {get{ return type;} set{ this.type = value;}}
		[MessagePack.Key(4)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(5)]
		public string _viewEndAt {get{ return viewEndAt;} set{ this.viewEndAt = value;}}
		[MessagePack.Key(6)]
		public long _keyMPointId {get{ return keyMPointId;} set{ this.keyMPointId = value;}}
		[MessagePack.Key(7)]
		public long _mCommonStoreCategoryId {get{ return mCommonStoreCategoryId;} set{ this.mCommonStoreCategoryId = value;}}
		[MessagePack.Key(8)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(9)]
		public long _priority {get{ return priority;} set{ this.priority = value;}}
		[MessagePack.Key(10)]
		public string _descriptionUrl {get{ return descriptionUrl;} set{ this.descriptionUrl = value;}}
		[MessagePack.Key(11)]
		public long _playCountType {get{ return playCountType;} set{ this.playCountType = value;}}
		[MessagePack.Key(12)]
		public long _dailyPlayCount {get{ return dailyPlayCount;} set{ this.dailyPlayCount = value;}}
		[MessagePack.Key(13)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mHuntId = 0; // $mHuntId 狩猟イベントID
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 開始日
		[UnityEngine.SerializeField] long type = 0; // 区分（サーバー的には区別しないが、クライアント・運用側で明確に扱いを変えたいコンテンツとかがある場合、ここに別な値を入れて区別する）
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 終了日（バトルをこれ以降行えない）
		[UnityEngine.SerializeField] string viewEndAt = ""; // $viewEndAt 終了日（バトル期間終了後の、表示等をいつまで行うか）
		[UnityEngine.SerializeField] long keyMPointId = 0; // $keyMPointId m_hunt.lotteryType = 4 の際、必要なポイントのIDを設定する。0ならば必要なし
		[UnityEngine.SerializeField] long mCommonStoreCategoryId = 0; // $mCommonStoreCategoryId 連動する交換所（なければ0。m_hunt側に移動する可能性も）
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId イベントポイント（なければ0。指定不要になる可能性も）
		[UnityEngine.SerializeField] long priority = 0; // $priority 数値が大きいものを先に表示する。
		[UnityEngine.SerializeField] string descriptionUrl = ""; // $descriptionUrl お知らせ等を入れているURL
		[UnityEngine.SerializeField] long playCountType = 0; // $playCountType 1: 勝利時にdailyPlayCountを消費、2: 挑戦時にdailyPlayCountを消費
		[UnityEngine.SerializeField] long dailyPlayCount = 0; // $dailyPlayCount 日毎のプレイ可能回数。0の場合は無制限
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntTimetableMasterObjectBase {
		public virtual HuntTimetableMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mHuntId => _rawData._mHuntId;
		public virtual string startAt => _rawData._startAt;
		public virtual long type => _rawData._type;
		public virtual string endAt => _rawData._endAt;
		public virtual string viewEndAt => _rawData._viewEndAt;
		public virtual long keyMPointId => _rawData._keyMPointId;
		public virtual long mCommonStoreCategoryId => _rawData._mCommonStoreCategoryId;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long priority => _rawData._priority;
		public virtual string descriptionUrl => _rawData._descriptionUrl;
		public virtual long playCountType => _rawData._playCountType;
		public virtual long dailyPlayCount => _rawData._dailyPlayCount;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntTimetableMasterValueObject _rawData = null;
		public HuntTimetableMasterObjectBase( HuntTimetableMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntTimetableMasterObject : HuntTimetableMasterObjectBase, IMasterObject {
		public HuntTimetableMasterObject( HuntTimetableMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntTimetableMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt_Timetable;

        [UnityEngine.SerializeField]
        HuntTimetableMasterValueObject[] m_Hunt_Timetable = null;
    }


}
