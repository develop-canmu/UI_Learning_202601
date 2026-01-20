//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class FestivalTimetableMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mFestivalId {get{ return mFestivalId;} set{ this.mFestivalId = value;}}
		[MessagePack.Key(2)]
		public string _adminName {get{ return adminName;} set{ this.adminName = value;}}
		[MessagePack.Key(3)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(4)]
		public long _mHuntTimetableId {get{ return mHuntTimetableId;} set{ this.mHuntTimetableId = value;}}
		[MessagePack.Key(5)]
		public long[] _mDailyMissionCategoryIdList {get{ return mDailyMissionCategoryIdList;} set{ this.mDailyMissionCategoryIdList = value;}}
		[MessagePack.Key(6)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(7)]
		public string _deadlineAt {get{ return deadlineAt;} set{ this.deadlineAt = value;}}
		[MessagePack.Key(8)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(9)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mFestivalId = 0; // $mFestivalId m_training_festival の ID
		[UnityEngine.SerializeField] string adminName = ""; // $adminName 管理表示名
		[UnityEngine.SerializeField] string name = ""; // $name 表示名
		[UnityEngine.SerializeField] long mHuntTimetableId = 0; // $mHuntTimetableId イベントストーリー用の狩猟タイムテーブルID
		[UnityEngine.SerializeField] long[] mDailyMissionCategoryIdList = null; // $mDailyMissionCategoryIdList 関連づけられたミッションカテゴリIDのリスト。1つ目の要素にイベントポイントの獲得により達成できるミッションのみで構成されたカテゴリ、2つ目の要素に任意のミッションカテゴリを入れる。例：[201,202]
		[UnityEngine.SerializeField] string startAt = ""; // $startAt イベント期間の開始日時
		[UnityEngine.SerializeField] string deadlineAt = ""; // $deadlineAt イベントポイント獲得可能期間の終了日時
		[UnityEngine.SerializeField] string endAt = ""; // $endAt イベント期間の終了日時
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class FestivalTimetableMasterObjectBase {
		public virtual FestivalTimetableMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mFestivalId => _rawData._mFestivalId;
		public virtual string adminName => _rawData._adminName;
		public virtual string name => _rawData._name;
		public virtual long mHuntTimetableId => _rawData._mHuntTimetableId;
		public virtual long[] mDailyMissionCategoryIdList => _rawData._mDailyMissionCategoryIdList;
		public virtual string startAt => _rawData._startAt;
		public virtual string deadlineAt => _rawData._deadlineAt;
		public virtual string endAt => _rawData._endAt;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        FestivalTimetableMasterValueObject _rawData = null;
		public FestivalTimetableMasterObjectBase( FestivalTimetableMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class FestivalTimetableMasterObject : FestivalTimetableMasterObjectBase, IMasterObject {
		public FestivalTimetableMasterObject( FestivalTimetableMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class FestivalTimetableMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Festival_Timetable;

        [UnityEngine.SerializeField]
        FestivalTimetableMasterValueObject[] m_Festival_Timetable = null;
    }


}
