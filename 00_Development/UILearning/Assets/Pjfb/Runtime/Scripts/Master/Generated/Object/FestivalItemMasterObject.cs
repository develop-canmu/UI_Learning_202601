//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class FestivalItemMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mFestivalTimetableId {get{ return mFestivalTimetableId;} set{ this.mFestivalTimetableId = value;}}
		[MessagePack.Key(2)]
		public long _mPointId {get{ return mPointId;} set{ this.mPointId = value;}}
		[MessagePack.Key(3)]
		public long _amount {get{ return amount;} set{ this.amount = value;}}
		[MessagePack.Key(4)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mFestivalTimetableId = 0; // $mFestivalTimetableId 特殊効果を発生させる対象となるイベントタイムテーブルID。同じタイムテーブルIDを持つ複数のレコードを作成すると、複数の手段で効果を発生させることが可能
		[UnityEngine.SerializeField] long mPointId = 0; // $mPointId 特殊効果を手動で発生させる際に消費するポイントのID
		[UnityEngine.SerializeField] long amount = 0; // $amount 特殊効果を手動で発生させる際のポイント消費量
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class FestivalItemMasterObjectBase {
		public virtual FestivalItemMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mFestivalTimetableId => _rawData._mFestivalTimetableId;
		public virtual long mPointId => _rawData._mPointId;
		public virtual long amount => _rawData._amount;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        FestivalItemMasterValueObject _rawData = null;
		public FestivalItemMasterObjectBase( FestivalItemMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class FestivalItemMasterObject : FestivalItemMasterObjectBase, IMasterObject {
		public FestivalItemMasterObject( FestivalItemMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class FestivalItemMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Festival_Item;

        [UnityEngine.SerializeField]
        FestivalItemMasterValueObject[] m_Festival_Item = null;
    }


}
