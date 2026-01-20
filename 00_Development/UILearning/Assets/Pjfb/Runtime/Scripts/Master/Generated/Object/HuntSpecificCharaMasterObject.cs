//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class HuntSpecificCharaMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mHuntTimetableId {get{ return mHuntTimetableId;} set{ this.mHuntTimetableId = value;}}
		[MessagePack.Key(2)]
		public string _mCharaIdList {get{ return mCharaIdList;} set{ this.mCharaIdList = value;}}
		[MessagePack.Key(3)]
		public string _mPointIdList {get{ return mPointIdList;} set{ this.mPointIdList = value;}}
		[MessagePack.Key(4)]
		public long _rate {get{ return rate;} set{ this.rate = value;}}
		[MessagePack.Key(5)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(6)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(7)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mHuntTimetableId = 0; // $mHuntTimetableId 狩猟イベントタイムテーブルID
		[UnityEngine.SerializeField] string mCharaIdList = ""; // $mCharaIdList キャラIDの配列
		[UnityEngine.SerializeField] string mPointIdList = ""; // $mPointIdList 効果対象となるポイントIDの配列
		[UnityEngine.SerializeField] long rate = 0; // $rate 効果倍率（万分率。1.1倍にしたい場合は、1000と入力）
		[UnityEngine.SerializeField] string startAt = ""; // $startAt 効果の開始時刻
		[UnityEngine.SerializeField] string endAt = ""; // $endAt 効果の終了時刻
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class HuntSpecificCharaMasterObjectBase {
		public virtual HuntSpecificCharaMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mHuntTimetableId => _rawData._mHuntTimetableId;
		public virtual string mCharaIdList => _rawData._mCharaIdList;
		public virtual string mPointIdList => _rawData._mPointIdList;
		public virtual long rate => _rawData._rate;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        HuntSpecificCharaMasterValueObject _rawData = null;
		public HuntSpecificCharaMasterObjectBase( HuntSpecificCharaMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class HuntSpecificCharaMasterObject : HuntSpecificCharaMasterObjectBase, IMasterObject {
		public HuntSpecificCharaMasterObject( HuntSpecificCharaMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class HuntSpecificCharaMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Hunt_Specific_Chara;

        [UnityEngine.SerializeField]
        HuntSpecificCharaMasterValueObject[] m_Hunt_Specific_Chara = null;
    }


}
