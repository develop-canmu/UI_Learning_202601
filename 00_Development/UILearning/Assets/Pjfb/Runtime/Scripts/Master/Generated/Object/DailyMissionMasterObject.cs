//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DailyMissionMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mDailyMissionCategoryId {get{ return mDailyMissionCategoryId;} set{ this.mDailyMissionCategoryId = value;}}
		[MessagePack.Key(2)]
		public long _previousMDailyMissionId {get{ return previousMDailyMissionId;} set{ this.previousMDailyMissionId = value;}}
		[MessagePack.Key(3)]
		public long _groupNumber {get{ return groupNumber;} set{ this.groupNumber = value;}}
		[MessagePack.Key(4)]
		public long _joinType {get{ return joinType;} set{ this.joinType = value;}}
		[MessagePack.Key(5)]
		public long _compareType {get{ return compareType;} set{ this.compareType = value;}}
		[MessagePack.Key(6)]
		public long _number {get{ return number;} set{ this.number = value;}}
		[MessagePack.Key(7)]
		public long _placeNumber {get{ return placeNumber;} set{ this.placeNumber = value;}}
		[MessagePack.Key(8)]
		public long _sortNumber {get{ return sortNumber;} set{ this.sortNumber = value;}}
		[MessagePack.Key(9)]
		public string _description {get{ return description;} set{ this.description = value;}}
		[MessagePack.Key(10)]
		public long _requireCount {get{ return requireCount;} set{ this.requireCount = value;}}
		[MessagePack.Key(11)]
		public PrizeJsonWrap[] _prizeJson {get{ return prizeJson;} set{ this.prizeJson = value;}}
		[MessagePack.Key(12)]
		public string _linkEx {get{ return linkEx;} set{ this.linkEx = value;}}
		[MessagePack.Key(13)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] long mDailyMissionCategoryId = 0; // ミッションカテゴリID
		[UnityEngine.SerializeField] long previousMDailyMissionId = 0; // 前提ミッションID（詳しい説明は schema.d 参照）
		[UnityEngine.SerializeField] long groupNumber = 0; // 同じミッションに対してのグループ番号
		[UnityEngine.SerializeField] long joinType = 0; // 参加タイプ。1 => 個人での参加、2 => ギルドでの参加
		[UnityEngine.SerializeField] long compareType = 0; // 比較タイプ。u_daily_mission.passingCount（P）と m_daily_mission.requireCount（R） との比較ルールをどのように定めるか。1: 以上（P >= R）、2: ビット論理積一致（(P & R) == R）
		[UnityEngine.SerializeField] long number = 0; // 通し番号（0始まり）。同一ミッションカテゴリ内でどのミッションをクリアしたかを判別するために使用する
		[UnityEngine.SerializeField] long placeNumber = 0; // クライアント側でのみ使用。表示場所を示す番号
		[UnityEngine.SerializeField] long sortNumber = 0; // ソート番号。カテゴリのタイプが1（通常ミッション）以外の場合は、同一タイプで必ず重複をなしにする。99の場合は1から始まる連番として設定する。
		[UnityEngine.SerializeField] string description = ""; // 解説文
		[UnityEngine.SerializeField] long requireCount = 0; // 要求数
		[UnityEngine.SerializeField] PrizeJsonWrap[] prizeJson = null; // 報酬json構造
		[UnityEngine.SerializeField] string linkEx = ""; // 遷移先URL等
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DailyMissionMasterObjectBase {
		public virtual DailyMissionMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mDailyMissionCategoryId => _rawData._mDailyMissionCategoryId;
		public virtual long previousMDailyMissionId => _rawData._previousMDailyMissionId;
		public virtual long groupNumber => _rawData._groupNumber;
		public virtual long joinType => _rawData._joinType;
		public virtual long compareType => _rawData._compareType;
		public virtual long number => _rawData._number;
		public virtual long placeNumber => _rawData._placeNumber;
		public virtual long sortNumber => _rawData._sortNumber;
		public virtual string description => _rawData._description;
		public virtual long requireCount => _rawData._requireCount;
		public virtual PrizeJsonWrap[] prizeJson => _rawData._prizeJson;
		public virtual string linkEx => _rawData._linkEx;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DailyMissionMasterValueObject _rawData = null;
		public DailyMissionMasterObjectBase( DailyMissionMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DailyMissionMasterObject : DailyMissionMasterObjectBase, IMasterObject {
		public DailyMissionMasterObject( DailyMissionMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DailyMissionMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Daily_Mission;

        [UnityEngine.SerializeField]
        DailyMissionMasterValueObject[] m_Daily_Mission = null;
    }


}
